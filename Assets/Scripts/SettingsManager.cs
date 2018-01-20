using UnityEngine;
using System.Collections;
using System.IO;

public class Settings
{
    public int v1;
    public int v2;

    public Settings()
    {
        v1 = 1;
        v2 = 2;
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
        catch (FileNotFoundException)
        {
            settings = new Settings();
            SaveSettings();
        }

        Debug.Log(settings.v1.ToString() + " " + settings.v2.ToString());
    }

    public void SaveSettings()
    {
        string jsonData = JsonUtility.ToJson(settings, true);
        File.WriteAllText(Application.dataPath + "/settings.json", jsonData);
    }
}
