using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;

public class RecordScript : MonoBehaviour {

	public const string CSVFileName = "driving_log.csv";
	public const string DirFrames = "IMG";

	private string m_SaveLocation = "";

	[SerializeField] private Camera CenterCamera;

	private bool m_isRecording = false;
	public bool IsRecording {
		get
		{
			return m_isRecording;
		}

		set
		{
			m_isRecording = value;
			if(value == true)
			{ 
				Debug.Log("Starting to record");
				//carSamples = new Queue<CarSample>();
				//StartCoroutine(Sample());             
			} 
			else
			{
				Debug.Log("Stopping record");
				//StopCoroutine(Sample());
				//Debug.Log("Writing to disk");
				//save the cars coordinate parameters so we can reset it to this properly after capturing data
				//saved_position = transform.position;
				//saved_rotation = transform.rotation;
				//see how many samples we captured use this to show save percentage in UISystem script
				//TotalSamples = carSamples.Count;
				//isSaving = true;
				//StartCoroutine(WriteSamplesToDisk());

			};
		}
	}

	public bool checkSaveLocation()
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
		Directory.CreateDirectory (Path.Combine(m_SaveLocation, DirFrames));
	}

	// Use this for initialization
	void Start () {
		checkSaveLocation ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
