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
	private float m_MaxRPM;

	[HideInInspector] public float speed = 0f;

    private void Start ()
    {
        m_WheelColliders [0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;
        m_Rigidbody = GetComponent<Rigidbody> ();

		float wheelCircumference = m_WheelColliders [0].radius * Mathf.PI * 2f;
		m_MaxRPM = (m_NoLoadSpeed / wheelCircumference) * 1000f / 60f;
    }
		

	public void Move (float accel, float steering)
    {
        for (int i = 0; i < 4; i++) {
            Quaternion quat;
            Vector3 position;
            m_WheelColliders [i].GetWorldPose (out position, out quat);
            m_WheelMeshes [i].transform.position = position;
            m_WheelMeshes [i].transform.rotation = quat;
        }

        //clamp input values
        steering = Mathf.Clamp (steering, -1, 1);
        accel = Mathf.Clamp (accel, -1, 1);
        //BrakeInput = footbrake = -1 * Mathf.Clamp (footbrake, -1, 0);
        //handbrake = Mathf.Clamp (handbrake, 0, 1);

        ApplyDrive (accel, steering);

        AddDownForce ();
        //TractionControl ();
		speed = m_Rigidbody.velocity.magnitude * 3.6f;
		Debug.Log ("speed: " + speed);
		//Debug.Log(accel + " " + steering);
    }


    private void ApplyDrive (float accel, float steering)
    {
		float leftAccel = Mathf.Clamp (accel + steering, -1, 1);
		float rightAccel = Mathf.Clamp (accel - steering, -1, 1);

		ApplyTorque (0, rightAccel);
		ApplyTorque (1, leftAccel);
		ApplyTorque (2, rightAccel);
		ApplyTorque (3, leftAccel);

		Debug.Log ("left: " + leftAccel + " right: " + rightAccel);
    }

	private void ApplyTorque(int i, float accel)
	{
		float thrustTorque = (accel * m_StallTorque);
		float breakTorque = (accel * m_BreakTorque);
		if (m_WheelColliders [i].rpm * accel >= 0f) {
			m_WheelColliders [i].motorTorque = thrustTorque * (m_MaxRPM - Mathf.Abs (m_WheelColliders [i].rpm)) / m_MaxRPM;
		} else {
			m_WheelColliders [i].motorTorque = thrustTorque + breakTorque * Mathf.Abs (m_WheelColliders [i].rpm) / m_MaxRPM;
		}
	}



    // this is used to add more grip in relation to speed
    private void AddDownForce ()
    {
        m_WheelColliders [0].attachedRigidbody.AddForce (-transform.up * m_DownForce *
        	m_WheelColliders [0].attachedRigidbody.velocity.magnitude);
    }
		
}