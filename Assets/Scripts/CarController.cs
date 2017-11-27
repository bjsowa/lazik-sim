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
    [SerializeField] private Vector3 m_CentreOfMassOffset;
    [SerializeField] private float m_MaximumSteerAngle;
    [Range (0, 1)] [SerializeField] private float m_SteerHelper;
    // 0 is raw physics , 1 the car will grip in the direction it is facing
    [Range (0, 1)] [SerializeField] private float m_TractionControl;
    // 0 is no traction control, 1 is full interference
    [SerializeField] private float m_FullTorqueOverAllWheels;
    [SerializeField] private float m_ReverseTorque;
    [SerializeField] private float m_Downforce = 100f;
    [SerializeField] private float m_Topspeed = 200;
    [SerializeField] private float m_SlipLimit;
    [SerializeField] private float m_BrakeTorque;

    private Quaternion[] m_WheelMeshLocalRotations;
    private Vector3 m_Prevpos, m_Pos;
    private float m_SteerAngle;
    private int m_GearNum;
    private float m_GearFactor;
    private float m_OldRotation;
    private float m_CurrentTorque;
    private Rigidbody m_Rigidbody;
    private const float k_ReversingThreshold = 0.01f;
	private int TotalSamples;
	private bool isSaving;
	private Vector3 saved_position;
	private Quaternion saved_rotation;

    public float CurrentSteerAngle {
        get { return m_SteerAngle; }
        set { m_SteerAngle = value; }
    }

    public float CurrentSpeed{ get { return m_Rigidbody.velocity.magnitude * 2.23693629f; } }

    public float MaxSpeed{ get { return m_Topspeed; } }

    public float Revs { get; private set; }

    public float AccelInput { get; set; }

    // Use this for initialization
    private void Start ()
    {
        m_WheelMeshLocalRotations = new Quaternion[4];
        for (int i = 0; i < 4; i++) {
            m_WheelMeshLocalRotations [i] = m_WheelMeshes [i].transform.localRotation;
        }
        m_WheelColliders [0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;

        m_Rigidbody = GetComponent<Rigidbody> ();
        m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl * m_FullTorqueOverAllWheels);
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
        AccelInput = accel = Mathf.Clamp (accel, 0, 1);
        //BrakeInput = footbrake = -1 * Mathf.Clamp (footbrake, -1, 0);
        //handbrake = Mathf.Clamp (handbrake, 0, 1);

        //Set the steer on the front wheels.
        //Assuming that wheels 0 and 1 are the front wheels.
        m_SteerAngle = steering * m_MaximumSteerAngle;
        m_WheelColliders [0].steerAngle = m_SteerAngle;
        m_WheelColliders [1].steerAngle = m_SteerAngle;



        SteerHelper ();
        ApplyDrive (accel, steering);

        AddDownForce ();
        TractionControl ();
    }


    private void ApplyDrive (float accel, float steering)
    {

        float thrustTorque;

        thrustTorque = accel * (m_CurrentTorque / 4f);
        for (int i = 0; i < 4; i++) {
            m_WheelColliders [i].motorTorque = thrustTorque;
        }


    }


    private void SteerHelper ()
    {
        for (int i = 0; i < 4; i++) {
            WheelHit wheelhit;
            m_WheelColliders [i].GetGroundHit (out wheelhit);
            if (wheelhit.normal == Vector3.zero)
                return; // wheels arent on the ground so dont realign the rigidbody velocity
        }

        // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
        if (Mathf.Abs (m_OldRotation - transform.eulerAngles.y) < 10f) {
            var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
            Quaternion velRotation = Quaternion.AngleAxis (turnadjust, Vector3.up);
            m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
        }
        m_OldRotation = transform.eulerAngles.y;
    }


    // this is used to add more grip in relation to speed
    private void AddDownForce ()
    {
        m_WheelColliders [0].attachedRigidbody.AddForce (-transform.up * m_Downforce *
        m_WheelColliders [0].attachedRigidbody.velocity.magnitude);
    }



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
}