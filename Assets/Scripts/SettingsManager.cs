using UnityEngine;
using System;
using System.IO;
using System.IO.IsolatedStorage;

public class Settings
{
    public int Width;
    public int Height;
    public int TopSpeed;
    public int FPS;
    public string IP;
    public int Port;

    public Settings()
    {
        Width = 320;
        Height = 160;
        TopSpeed = 10;
        FPS = 15;
        IP = "127.0.0.1";
        Port = 4567;
    }
}

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    public Settings Settings;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            //Disable Vsync
            QualitySettings.vSyncCount = 0;
        }
        else
        {
            Destroy(gameObject);
        }

        Instance.LoadSettings();
    }

    public void LoadSettings()
    {
        try
        {
            string jsonData = File.ReadAllText(Application.dataPath + "/settings.json");
            Settings = JsonUtility.FromJson<Settings>(jsonData);
        }
        catch (Exception ex)
        {
            if (ex is FileNotFoundException || ex is IsolatedStorageException)
            {
                Settings = new Settings();
                SaveSettings();
                return;
            }
            throw;
        }
    }

    public void SaveSettings()
    {
        string jsonData = JsonUtility.ToJson(Settings, true);
        File.WriteAllText(Application.dataPath + "/settings.json", jsonData);
    }
}
