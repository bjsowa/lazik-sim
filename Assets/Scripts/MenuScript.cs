using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class MenuScript : MonoBehaviour {

    public InputField IPField;
    public InputField PortField;
    public InputField WidthField;
    public InputField HeightField;
    public Slider TopSpeedSlider;
    public Text CurrentTopSpeed;
    public Slider FPSSlider;
    public Text CurrentFPS;

	public void PlayTraining() { SceneManager.LoadScene (1); }
	public void PlayAutonomous() { SceneManager.LoadScene (2); }
    public void ChangeTopSpeed(float value) { CurrentTopSpeed.text = value.ToString(); }
    public void ChangeFPS(float value) { CurrentFPS.text = value.ToString(); }

    public void ApplySettings()
    {
        try
        {
            SettingsManager.Instance.Settings.Width = Int32.Parse(WidthField.text);
            SettingsManager.Instance.Settings.Height = Int32.Parse(HeightField.text);
            SettingsManager.Instance.Settings.TopSpeed = (int)TopSpeedSlider.value;
            SettingsManager.Instance.Settings.FPS = (int)FPSSlider.value;
            SettingsManager.Instance.Settings.IP = IPField.text;
            SettingsManager.Instance.Settings.Port = Int32.Parse(PortField.text);
        }
        catch (OverflowException)
        {
            SettingsManager.Instance.LoadSettings();
        }

        SettingsManager.Instance.SaveSettings();
    }

    public void RevertSettings()
    {
        Settings settings = SettingsManager.Instance.Settings;
        WidthField.text = settings.Width.ToString();
        HeightField.text = settings.Height.ToString();
        TopSpeedSlider.value = settings.TopSpeed;
        CurrentTopSpeed.text = settings.TopSpeed.ToString();
        FPSSlider.value = settings.FPS;
        CurrentFPS.text = settings.FPS.ToString();
        IPField.text = settings.IP;
        PortField.text = settings.Port.ToString();
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
