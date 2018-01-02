using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIScript : MonoBehaviour {

    [SerializeField] private Text m_SpeedText;
    [SerializeField] private Text m_AngularSpeedText;
    [SerializeField] private Text m_TargetSpeedText;
    [SerializeField] private Text m_TargetAngularSpeedText;

    private RecordScript m_Record;
    private CarController m_Car;
    private PidController m_Pid;

    private void Awake()
    {
        GameObject lazik = GameObject.FindWithTag("Player");
        m_Record = lazik.GetComponent<RecordScript>();
        m_Car = lazik.GetComponent<CarController>();
        m_Pid = lazik.GetComponent<PidController>();
    }

    private void Start()
    {

    }

    public void ToggleRecord()
    {
        if( !m_Record.IsRecording )
            m_Record.IsRecording = true;
        else
            m_Record.IsRecording = false;
    }

    // Update is called once per frame
    private void Update() {
        if (Input.GetButtonDown("Cancel"))
            SceneManager.LoadScene (0);

        if (Input.GetButtonDown("Record"))
            ToggleRecord();

        m_SpeedText.text = m_Car.speed.ToString();
        m_AngularSpeedText.text = m_Car.angularSpeed.ToString();
        m_TargetSpeedText.text = m_Pid.targetSpeed.ToString();
        m_TargetAngularSpeedText.text = m_Pid.targetAngularSpeed.ToString();
    }
}
