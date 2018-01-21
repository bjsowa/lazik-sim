using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class MenuScript : MonoBehaviour {

    public InputField IPField;
    public InputField PortField;
    public InputField WidthField;
    public InputField HeightField;

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
        try
        {
            SettingsManager.Instance.settings.ip = IPField.text;
            SettingsManager.Instance.settings.port = Int32.Parse(PortField.text);
            SettingsManager.Instance.settings.width = Int32.Parse(WidthField.text);
            SettingsManager.Instance.settings.height = Int32.Parse(HeightField.text);
        }
        catch (OverflowException)
        {
            SettingsManager.Instance.LoadSettings();
        }

        SettingsManager.Instance.SaveSettings();
    }

    public void RevertSettings()
    {
        Settings settings = SettingsManager.Instance.settings;
        IPField.text = settings.ip;
        PortField.text = settings.port.ToString();
        WidthField.text = settings.width.ToString();
        HeightField.text = settings.height.ToString();
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
