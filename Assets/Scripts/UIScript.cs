using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour {

    private RecordScript m_Record;

    private void Awake()
    {
        GameObject lazik = GameObject.FindWithTag("Player");
        m_Record = lazik.GetComponent<RecordScript>();
    }

    public void ToggleRecord()
    {
        if( !m_Record.IsRecording)
        {
            m_Record.IsRecording = true;
        }
        else
        {
            m_Record.IsRecording = false;
        }
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Cancel"))
            SceneManager.LoadScene (0);

        if (Input.GetButtonDown("Record"))
            ToggleRecord();
    }
}
