using UnityEngine;
using System.Collections;
using System.IO;
using System.IO.IsolatedStorage;
using System;

public class Settings
{
    public string ip;
    public string port;

    public Settings()
    {
        ip = "127.0.0.1";
        port = "4567";
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
