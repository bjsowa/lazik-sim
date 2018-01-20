using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

    public InputField IPField;
    public InputField PortField;

	public void PlayTraining()
	{
		SceneManager.LoadScene (1);
	}

	public void PlayAutonomous() 
	{
		SceneManager.LoadScene (2);
	}

    public void Exit()
    {
        Application.Quit();
    }

    public void ApplySettings()
    {
        SettingsManager.Instance.settings.ip = IPField.text;
        SettingsManager.Instance.settings.port = PortField.text;
        SettingsManager.Instance.SaveSettings();
    }

    public void RevertSettings()
    {
        Settings settings = SettingsManager.Instance.settings;
        IPField.text = settings.ip;
        PortField.text = settings.port;
    }

    private void Start()
    {
        RevertSettings();
    }

    void Update() {
        if (Input.GetButtonDown("Cancel"))
            Exit();
    }
}
