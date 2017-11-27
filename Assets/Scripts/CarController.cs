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
    [SerializeField] private float m_DownForce = 100f;

    private Rigidbody m_Rigidbody;
	private float m_MaxRPM;

    // Use this for initialization
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

		Debug.Log ("speed: " + m_Rigidbody.velocity.magnitude * 3.6f);
		//Debug.Log(accel + " " + steering);
    }


    private void ApplyDrive (float accel, float steering)
    {

        float thrustTorque;

		thrustTorque = (accel * m_StallTorque);
        for (int i = 0; i < 4; i++) {
			
			m_WheelColliders [i].motorTorque = thrustTorque * (m_MaxRPM - m_WheelColliders[i].rpm) / m_MaxRPM;
        }


		m_WheelColliders [0].motorTorque -= steering * m_StallTorque;
		m_WheelColliders [1].motorTorque += steering * m_StallTorque;
		m_WheelColliders [2].motorTorque -= steering * m_StallTorque;
		m_WheelColliders [3].motorTorque += steering * m_StallTorque;
    }




    // this is used to add more grip in relation to speed
    private void AddDownForce ()
    {
        m_WheelColliders [0].attachedRigidbody.AddForce (-transform.up * m_DownForce *
        m_WheelColliders [0].attachedRigidbody.velocity.magnitude);
    }


	/*
    // crude traction control that reduces the power to wheel if the car is wheel spinning too much
    private void TractionControl ()
    {
        WheelHit wheelHit;

        for (int i = 0; i < 4; i++) {
            m_WheelColliders [i].GetGroundHit (out wheelHit);

            AdjustTorque (wheelHit.forwardSlip);
        }

    }


    private void AdjustTorque (float forwardSlip)
    {
        if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0) {
            m_CurrentTorque -= 10 * m_TractionControl;
        } else {
            m_CurrentTorque += 10 * m_TractionControl;
            if (m_CurrentTorque > m_FullTorqueOverAllWheels) {
                m_CurrentTorque = m_FullTorqueOverAllWheels;
            }
        }
    }
	*/
}