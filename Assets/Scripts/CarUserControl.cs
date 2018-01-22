using UnityEngine;
using System.Collections;


[RequireComponent(typeof(PidController))]
public class CarUserControl : MonoBehaviour
{
    [SerializeField] private bool m_Autonomous;

    private PidController m_Controller;

    public enum Mode { Manual, Autonomous };
    public Mode CurrentMode { get; private set; }

    private void Awake() {
        m_Controller = GetComponent<PidController> ();
        if (m_Autonomous)
            CurrentMode = Mode.Autonomous;
        else
            CurrentMode = Mode.Manual;
    }

    private void Update()
    {
        if (m_Autonomous && Input.GetButtonDown("Control"))
        {
            if (CurrentMode == Mode.Autonomous)
                CurrentMode = Mode.Manual;
            else
                CurrentMode = Mode.Autonomous;
        }

        if (CurrentMode == Mode.Manual)
        {
            float accel = Input.GetAxis("Accel");
            float steering = Input.GetAxis("Steering");
            m_Controller.Move(accel, steering);
        }
    }
}
