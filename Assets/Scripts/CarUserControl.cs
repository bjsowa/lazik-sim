using UnityEngine;
using System.Collections;


[RequireComponent(typeof(PidController))]
public class CarUserControl : MonoBehaviour
{
    private PidController m_Controller;

    private void Awake() {
        m_Controller = GetComponent<PidController> ();
    }

    private void FixedUpdate()
    {
		float v = Input.GetAxis ("Vertical");
		float h = Input.GetAxis ("Horizontal");
		m_Controller.Move(v, h);
    }
}
