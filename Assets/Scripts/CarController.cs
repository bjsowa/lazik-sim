using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CarController : MonoBehaviour
{
    [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
    [SerializeField] private GameObject[] m_WheelMeshes = new GameObject[4];
	[SerializeField] private Vector3 m_CentreOfMassOffset = new Vector3(0,0,0);

	[SerializeField] private float m_StallTorque = 100f;
	[SerializeField] private float m_NoLoadSpeed = 100f;
	[SerializeField] private float m_BreakTorque = 100f;
    [SerializeField] private float m_DownForce = 100f;

    private Rigidbody m_Rigidbody;
    private float m_WheelCircumference;
	private float m_MaxRPM;

    public float Speed { get; private set; }
    public float[] WheelSpeed { get; private set; }

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        m_Rigidbody.centerOfMass = m_CentreOfMassOffset;

        m_WheelCircumference = m_WheelColliders[0].radius * Mathf.PI * 2f;
        m_MaxRPM = (m_NoLoadSpeed / m_WheelCircumference) * 1000f / 60f;

        WheelSpeed = new float[] { 0f, 0f, 0f, 0f };
        Speed = 0f;
    }

    private void Update()
    {
        // transform meshes
        for (int i = 0; i < 4; i++)
        {
            Quaternion quat;
            Vector3 position;
            m_WheelColliders[i].GetWorldPose(out position, out quat);
            m_WheelMeshes[i].transform.position = position;
            m_WheelMeshes[i].transform.rotation = quat;
        }

        // calculate speed 
        WheelSpeed[0] = m_WheelColliders[0].rpm * m_WheelCircumference * 3.6f / 60f;
        WheelSpeed[1] = m_WheelColliders[1].rpm * m_WheelCircumference * 3.6f / 60f;
        WheelSpeed[2] = m_WheelColliders[2].rpm * m_WheelCircumference * 3.6f / 60f;
        WheelSpeed[3] = m_WheelColliders[3].rpm * m_WheelCircumference * 3.6f / 60f;

        Speed = (WheelSpeed[0] + WheelSpeed[1] + WheelSpeed[2] + WheelSpeed[3]) / 4f;

        // add more grip in relation to speed
        AddDownForce();
    }

	public void ApplyTorque(int i, float accel)
	{
		float thrustTorque = (accel * m_StallTorque);
		float breakTorque = (accel * m_BreakTorque);
		if (m_WheelColliders [i].rpm * accel >= 0f) {
			m_WheelColliders [i].motorTorque = 
                thrustTorque * (m_MaxRPM - Mathf.Abs (m_WheelColliders [i].rpm)) / m_MaxRPM;
		} else {
			m_WheelColliders [i].motorTorque = 
                thrustTorque + breakTorque * Mathf.Abs (m_WheelColliders [i].rpm) / m_MaxRPM;
		}
	}

    private void AddDownForce ()
    {
        m_Rigidbody.AddForce (-transform.up * m_DownForce *
        	m_Rigidbody.velocity.magnitude);
    }
		
}