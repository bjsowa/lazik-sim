using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIScript : MonoBehaviour {

    [SerializeField] private Sprite m_RecordSprite;
    [SerializeField] private Sprite m_StopSprite;

    [SerializeField] private Text m_SpeedText;
    [SerializeField] private Text m_AngularSpeedText;
    [SerializeField] private Text m_TargetSpeedText;
    [SerializeField] private Text m_TargetAngularSpeedText;
    [SerializeField] private Text m_CollectingDataText;

    private RecordScript m_Record;
    private CarController m_Car;
    private PidController m_Pid;
    private Image m_RecordImage;

    private string m_BaseSpeed;
    private string m_BaseAngularSpeed;
    private string m_BaseTargetSpeed;
    private string m_BaseTargetAngularSpeed;
    private string m_BaseCollectingData;

    private void Awake()
    {
        GameObject lazik = GameObject.FindWithTag("Player");
        m_Record = lazik.GetComponent<RecordScript>();
        m_Car = lazik.GetComponent<CarController>();
        m_Pid = lazik.GetComponent<PidController>();
        m_RecordImage = GameObject.Find("RecordButton").GetComponent<Image>();
    }

    private void Start()
    {
        m_BaseSpeed = m_SpeedText.text;
        m_BaseAngularSpeed = m_AngularSpeedText.text;
        m_BaseTargetSpeed = m_TargetSpeedText.text;
        m_BaseTargetAngularSpeed = m_TargetAngularSpeedText.text;
        m_BaseCollectingData = m_CollectingDataText.text;
    }

    public void ToggleRecord()
    {
        if (m_Record.isSaving)
            return;

        if (!m_Record.isRecording)
        {
            m_Record.isRecording = true;
            if (m_Record.isRecording)
                m_RecordImage.sprite = m_StopSprite;
        }
        else
        {
            m_Record.isRecording = false;
            m_RecordImage.sprite = m_RecordSprite;
        }
    }

    // Update is called once per frame
    private void Update() {
        if (Input.GetButtonDown("Cancel"))
            SceneManager.LoadScene (0);

        if (Input.GetButtonDown("Record"))
            ToggleRecord();

        m_SpeedText.text = m_BaseSpeed + m_Car.speed.ToString("0.00") + " km/h";
        m_AngularSpeedText.text = m_BaseAngularSpeed + m_Car.angularSpeed.ToString("0.00") + " RPM";
        m_TargetSpeedText.text = m_BaseTargetSpeed + m_Pid.targetSpeed.ToString("0.00") + " km/h";
        m_TargetAngularSpeedText.text = m_BaseTargetAngularSpeed + m_Pid.targetAngularSpeed.ToString("0.00") + " RPM";

        if (m_Record.isSaving)
            m_CollectingDataText.text = m_BaseCollectingData + m_Record.getSavePercent().ToString() + " %";
        else
            m_CollectingDataText.text = "";
    }
}
