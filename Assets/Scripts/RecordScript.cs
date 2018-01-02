using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;

[Serializable]
public class MyCamera
{
    public string name;
    public Camera camera;
}

internal class CarSample
{
    public Quaternion rotation;
    public Vector3 position;
    public float accel;
    public float steering;
    public float speed;
    public float angularSpeed;
    public string timeStamp;
}

public class RecordScript : MonoBehaviour {

    //[SerializeField] private Camera m_CenterCamera = new Camera();
    [SerializeField] private MyCamera[] m_Cameras;

    private const string m_CSVFileName = "driving_log.csv";
	private const string m_DirFrames = "IMG";

	private Rigidbody m_Rigidbody;
	private CarController m_Car;
	private PidController m_Pid;

	private string m_SaveLocation = "";
	private Queue<CarSample> m_CarSamples;
	private Vector3 m_SavedPosition;
	private Quaternion m_SavedRotation;
	private bool m_IsRecording = false;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Car = GetComponent<CarController>();
        m_Pid = GetComponent<PidController>();
    }

    public int totalSamples { get; private set; }
    public bool isSaving { get; private set; }

    public int getSavePercent()
    {
        float part = (float)(totalSamples - m_CarSamples.Count) / totalSamples;
        return (int)Math.Round(100f * part);
    }

    public bool isRecording {
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
                        m_IsRecording = true;
                        Debug.Log("Starting to record");
                        m_CarSamples = new Queue<CarSample>();
                        StartCoroutine(Sample());
                    }
                }
                else
                {
                    m_IsRecording = false;
                    Debug.Log("Stopping record");
                    StopCoroutine(Sample());
                    Debug.Log("Writing to disk");
                    //save the cars coordinate parameters so we can reset it to this properly after capturing data
                    m_SavedPosition = transform.position;
                    m_SavedRotation = transform.rotation;
                    //see how many samples we captured use this to show save percentage in UISystem script
                    totalSamples = m_CarSamples.Count;
                    isSaving = true;
                    StartCoroutine(WriteSamplesToDisk());
                }
            }
		}
	}
		
	private bool checkSaveLocation()
	{
		if (m_SaveLocation != "") {
			return true;
		} else {
			FileBrowser.ShowSaveDialog (OpenFolder, null, true, null, "Select Output Folder", "Select");
		}
		return false;
	}

	private void OpenFolder(string location)
	{
		m_SaveLocation = location;
		Directory.CreateDirectory (Path.Combine(m_SaveLocation, m_DirFrames));
	}

	//Changed the WriteSamplesToDisk to a IEnumerator method that plays back recording along with percent status from UISystem script 
	//instead of showing frozen screen until all data is recorded
	private IEnumerator WriteSamplesToDisk()
	{
		yield return new WaitForSeconds(0.000f); //retrieve as fast as we can but still allow communication of main thread to screen and UISystem
		if (m_CarSamples.Count > 0) {
			//pull off a sample from the que
			CarSample sample = m_CarSamples.Dequeue();

			//pysically moving the car to get the right camera position
			transform.position = sample.position;
			transform.rotation = sample.rotation;

            // Capture and Persist Image
            string paths = "";
            foreach (MyCamera cam in m_Cameras)
                paths += WriteImage(cam.camera, cam.name, sample.timeStamp) + ",";

			string row = string.Format ("{0}{1},{2},{3},{4}\n", paths, sample.accel, sample.steering, sample.speed, sample.angularSpeed);
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
			isSaving = false;

			//need to reset the car back to its position before ending recording, otherwise sometimes the car ended up in strange areas
			transform.position = m_SavedPosition;
			transform.rotation = m_SavedRotation;
			m_Rigidbody.velocity = new Vector3(0f,-10f,0f);
			//Move(0f, 0f, 0f, 0f);

		}
	}

	private IEnumerator Sample()
	{
		// Start the Coroutine to Capture Data Every Second.
		// Persist that Information to a CSV and Perist the Camera Frame
		yield return new WaitForSeconds(0.0666666666666667f);

		if (m_SaveLocation != "")
		{
			CarSample sample = new CarSample();

			sample.timeStamp = System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff");
			sample.accel = m_Pid.accel;
			sample.steering = m_Pid.steering;
			sample.speed = m_Car.speed;
			sample.angularSpeed = m_Car.angularSpeed;
			sample.position = transform.position;
			sample.rotation = transform.rotation;

			m_CarSamples.Enqueue(sample);

			sample = null;
			//may or may not be needed
		}

		// Only reschedule if the button hasn't toggled
		if (isRecording){
			StartCoroutine(Sample());
		}

	}

	private string WriteImage (Camera camera, string prepend, string timestamp)
	{
		//needed to force camera update 
		camera.Render();
		RenderTexture targetTexture = camera.targetTexture;
		RenderTexture.active = targetTexture;
		Texture2D texture2D = new Texture2D (targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);
		texture2D.ReadPixels (new Rect (0, 0, targetTexture.width, targetTexture.height), 0, 0);
		texture2D.Apply ();
		byte[] image = texture2D.EncodeToJPG ();
		UnityEngine.Object.DestroyImmediate (texture2D);
		string directory = Path.Combine(m_SaveLocation, m_DirFrames);
		string path = Path.Combine(directory, prepend + "_" + timestamp + ".jpg");
		File.WriteAllBytes (path, image);
		image = null;
		return path;
	}
}