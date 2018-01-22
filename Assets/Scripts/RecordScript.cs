using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;

[Serializable]
public class MyCamera
{
    public string Name;
    public Camera Camera;
}

internal class CarSample
{
    public Quaternion Rotation;
    public Vector3 Position;
    public float Accel;
    public float Steering;
    public float Speed;
    public string TimeStamp;
    public char Mode;
}

public class RecordScript : MonoBehaviour
{
    [SerializeField] private MyCamera[] m_Cameras;

    private const string m_CSVFileName = "driving_log.csv";
	private const string m_DirFrames = "IMG";

	private Rigidbody m_Rigidbody;
	private CarController m_Car;
	private PidController m_Pid;
    private CarUserControl m_Control;

	private string m_SaveLocation = "";
	private Queue<CarSample> m_CarSamples;
	private Vector3 m_SavedPosition;
	private Quaternion m_SavedRotation;
    private Vector3 m_SavedVelocity;
	private bool m_IsRecording = false;

    private RenderTexture m_TargetTexture;
    private float m_FrameDelay;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Car = GetComponent<CarController>();
        m_Pid = GetComponent<PidController>();
        m_Control = GetComponent<CarUserControl>();

        int width, height, fps;
        if (SettingsManager.Instance == null)
        {
            width = 320;
            height = 160;
            fps = 15;
        }
        else
        {
            width = SettingsManager.Instance.Settings.Width;
            height = SettingsManager.Instance.Settings.Height;
            fps = SettingsManager.Instance.Settings.FPS;
        }

        m_TargetTexture = new RenderTexture(width,height, 24, RenderTextureFormat.ARGB32);
        foreach (MyCamera cam in m_Cameras)
            cam.Camera.targetTexture = m_TargetTexture;

        m_FrameDelay = 1 / ((float)fps);
    }

    public int TotalSamples { get; private set; }
    public bool IsSaving { get; private set; }

    public int GetSavePercent()
    {
        float part = (float)(TotalSamples - m_CarSamples.Count) / TotalSamples;
        return (int)Math.Round(100f * part);
    }

    public bool IsRecording {
		get {
			return m_IsRecording;
		}
		set {
            if (m_IsRecording != value)
            {
                if (value == true)
                {
                    if (checkSaveLocation())
                    {
                        Debug.Log("Starting to record");
                        m_IsRecording = true;
                        m_CarSamples = new Queue<CarSample>();
                        StartCoroutine(Sample());
                    }
                }
                else
                {
                    Debug.Log("Stopping record");
                    m_IsRecording = false;
                    StopCoroutine(Sample());

                    Debug.Log("Writing to disk");
                    m_SavedPosition = transform.position;
                    m_SavedRotation = transform.rotation;
                    m_SavedVelocity = m_Rigidbody.velocity;
                    TotalSamples = m_CarSamples.Count;
                    IsSaving = true;
                    StartCoroutine(WriteSamplesToDisk());
                }
            }
		}
	}
		
	private bool checkSaveLocation()
	{
        if (m_SaveLocation != "")
            return true;
        else
        {
            FileBrowser.ShowSaveDialog
                (OpenFolder, null, true, null, "Select Output Folder", "Select");
        }
		return false;
	}

	private void OpenFolder(string location)
	{
		m_SaveLocation = location;
		Directory.CreateDirectory (Path.Combine(m_SaveLocation, m_DirFrames));
	}

	private IEnumerator WriteSamplesToDisk()
	{
        //retrieve as fast as we can but still allow communication of main thread to screen and UISystem
        yield return new WaitForSeconds(0.000f); 
		if (m_CarSamples.Count > 0) {
			//pull off a sample from the queue
			CarSample sample = m_CarSamples.Dequeue();

			//pysically moving the car to get the right camera position
			transform.position = sample.Position;
			transform.rotation = sample.Rotation;

            // Capture and Persist Image
            string paths = "";
            foreach (MyCamera cam in m_Cameras)
                paths += WriteImage(cam.Camera, cam.Name, sample.TimeStamp) + ",";

			string row = string.Format ("{0}{1},{2},{3},{4}\n", 
                paths, sample.Accel, sample.Steering, sample.Speed, sample.Mode);
			File.AppendAllText (Path.Combine (m_SaveLocation, m_CSVFileName), row);
		}
		if (m_CarSamples.Count > 0) {
			//request if there are more samples to pull
			StartCoroutine(WriteSamplesToDisk()); 
		}
		else 
		{
			//all samples have been pulled
			StopCoroutine(WriteSamplesToDisk());
			IsSaving = false;

			//need to reset the car back to its position before ending recording
			transform.position = m_SavedPosition;
			transform.rotation = m_SavedRotation;
            m_Rigidbody.velocity = m_SavedVelocity;

		}
	}

	private IEnumerator Sample()
	{
		// Start the Coroutine to Capture Data.
		// Persist that Information to a CSV and Perist the Camera Frame
		yield return new WaitForSeconds(m_FrameDelay);

		if (m_SaveLocation != "")
		{
			CarSample sample = new CarSample();

			sample.TimeStamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff");
            sample.Mode = m_Control.CurrentMode == CarUserControl.Mode.Autonomous ? 'A' : 'M';
			sample.Accel = m_Pid.Accel;
			sample.Steering = m_Pid.Steering;
			sample.Speed = m_Car.Speed;
			sample.Position = transform.position;
			sample.Rotation = transform.rotation;

			m_CarSamples.Enqueue(sample);

			sample = null;
		}

		// Only reschedule if the button hasn't been toggled
		if (IsRecording)
			StartCoroutine(Sample());

	}

	private string WriteImage (Camera camera, string prepend, string timestamp)
	{
		//force camera update 
		camera.Render();

		RenderTexture targetTexture = camera.targetTexture;
		RenderTexture.active = targetTexture;

		Texture2D texture2D = new Texture2D (
            targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);
		texture2D.ReadPixels (new Rect (0, 0, targetTexture.width, targetTexture.height), 0, 0);
		texture2D.Apply ();

		byte[] image = texture2D.EncodeToJPG ();
		DestroyImmediate (texture2D);

        string filename = Path.Combine(m_DirFrames, prepend + "_" + timestamp + ".jpg");
        string path = Path.Combine(m_SaveLocation, filename);

        File.WriteAllBytes (path, image);
		image = null;

		return filename;
	}
}