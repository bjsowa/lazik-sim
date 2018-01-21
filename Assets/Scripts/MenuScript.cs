using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class MenuScript : MonoBehaviour {

    public InputField IPField;
    public InputField PortField;
    public InputField WidthField;
    public InputField HeightField;
    public Slider FPSSlider;
    public Text CurrentFPS;

	public void PlayTraining() { SceneManager.LoadScene (1); }
	public void PlayAutonomous() { SceneManager.LoadScene (2); }
    public void ChangeFPS(float value) { CurrentFPS.text = value.ToString(); }

    public void ApplySettings()
    {
        try
        {
            SettingsManager.Instance.settings.width = Int32.Parse(WidthField.text);
            SettingsManager.Instance.settings.height = Int32.Parse(HeightField.text);
            SettingsManager.Instance.settings.fps = (int) FPSSlider.value;
            SettingsManager.Instance.settings.ip = IPField.text;
            SettingsManager.Instance.settings.port = Int32.Parse(PortField.text);
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
        WidthField.text = settings.width.ToString();
        HeightField.text = settings.height.ToString();
        FPSSlider.value = settings.fps;
        CurrentFPS.text = settings.fps.ToString();
        IPField.text = settings.ip;
        PortField.text = settings.port.ToString();
    }

    private void Start()
    {
        RevertSettings();
    }

    void Update() {
        if (Input.GetButtonDown("Cancel"))
            Application.Quit();
    }
}
