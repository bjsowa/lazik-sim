﻿using System;
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
    public char mode;
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

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Car = GetComponent<CarController>();
        m_Pid = GetComponent<PidController>();
        m_Control = GetComponent<CarUserControl>();

        int width, height;
        if (SettingsManager.Instance == null)
        {
            width = 320;
            height = 160;
        }
        else
        {
            width = SettingsManager.Instance.settings.width;
            height = SettingsManager.Instance.settings.height;
        }

        m_TargetTexture = new RenderTexture(width,height, 24, RenderTextureFormat.ARGB32);
        foreach( MyCamera cam in m_Cameras )
            cam.camera.targetTexture = m_TargetTexture;
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
                    totalSamples = m_CarSamples.Count;
                    isSaving = true;
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
			transform.position = sample.position;
			transform.rotation = sample.rotation;

            // Capture and Persist Image
            string paths = "";
            foreach (MyCamera cam in m_Cameras)
                paths += WriteImage(cam.camera, cam.name, sample.timeStamp) + ",";

			string row = string.Format ("{0}{1},{2},{3},{4},{5}\n", 
                paths, sample.accel, sample.steering, sample.speed, sample.angularSpeed, sample.mode);
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

			//need to reset the car back to its position before ending recording
			transform.position = m_SavedPosition;
			transform.rotation = m_SavedRotation;
            m_Rigidbody.velocity = m_SavedVelocity;

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

			sample.timeStamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff");
            sample.mode = m_Control.currentMode == CarUserControl.Mode.Autonomous ? 'A' : 'M';
			sample.accel = m_Pid.accel;
			sample.steering = m_Pid.steering;
			sample.speed = m_Car.speed;
			sample.angularSpeed = m_Car.angularSpeed;
			sample.position = transform.position;
			sample.rotation = transform.rotation;

			m_CarSamples.Enqueue(sample);

			sample = null;
		}

		// Only reschedule if the button hasn't been toggled
		if (isRecording)
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