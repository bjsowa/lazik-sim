using UnityEngine;
using System;
using System.IO;
using System.IO.IsolatedStorage;

public class Settings
{
    public int width;
    public int height;
    public int fps;
    public string ip;
    public int port;

    public Settings()
    {
        width = 320;
        height = 160;
        fps = 15;
        ip = "127.0.0.1";
        port = 4567;
    }
}

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    public Settings settings;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
            settings = JsonUtility.FromJson<Settings>(jsonData);
        }
        catch (Exception ex)
        {
            if (ex is FileNotFoundException || ex is IsolatedStorageException)
            {
                settings = new Settings();
                SaveSettings();
                return;
            }
            throw;
        }
    }

    public void SaveSettings()
    {
        string jsonData = JsonUtility.ToJson(settings, true);
        File.WriteAllText(Application.dataPath + "/settings.json", jsonData);
    }
}
