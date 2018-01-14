using UnityEngine;
using System.Collections;


[RequireComponent(typeof(PidController))]
public class CarUserControl : MonoBehaviour
{
    [SerializeField] private bool m_Autonomous;

    private PidController m_Controller;

    public enum Mode { Manual, Autonomous };
    public Mode currentMode { get; private set; }

    private void Awake() {
        m_Controller = GetComponent<PidController> ();
        if (m_Autonomous)
            currentMode = Mode.Autonomous;
        else
            currentMode = Mode.Manual;
    }

    private void Update()
    {
        if (m_Autonomous && Input.GetButtonDown("Control"))
        {
            if (currentMode == Mode.Autonomous)
                currentMode = Mode.Manual;
            else
                currentMode = Mode.Autonomous;
        }
    }

    private void FixedUpdate()
    {
        if (currentMode == Mode.Manual)
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");
            m_Controller.Move(v, h);
        }
    }
}
