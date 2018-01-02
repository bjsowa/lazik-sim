using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class PidController : MonoBehaviour 
{
	private CarController m_Car;
	[SerializeField] private float m_TopForwardSpeed = 30f;
	[SerializeField] private float m_PForward = 1f;
	[SerializeField] private float m_IForward = 1f;
	[SerializeField] private float m_DForward = 1f;
	[SerializeField] private float m_ForwardSpeedThreshold = 1f;

	[SerializeField] private float m_TopAngularSpeed = 30f;
	[SerializeField] private float m_PAngular = 1f;
	[SerializeField] private float m_IAngular = 1f;
	[SerializeField] private float m_DAngular = 1f;
	[SerializeField] private float m_AngularSpeedThreshold = 1f;

	private float m_PreviousForwardError = 0f;
	private float m_CurrentForwardError = 0f;

	private float m_PreviousAngularError = 0f;
	private float m_CurrentAngularError = 0f;

	[HideInInspector] public float accelProportional = 0f;
	[HideInInspector] public float accelIntegral = 0f;
	[HideInInspector] public float accelDerivative = 0f;
	[HideInInspector] public float accelResult = 0f;

	[HideInInspector] public float steeringProportional = 0f;
	[HideInInspector] public float steeringIntegral = 0f;
	[HideInInspector] public float steeringDerivative = 0f;
	[HideInInspector] public float steeringResult = 0f;

	public float accel { get; private set; }
	public float steering { get; private set; }
    public float targetSpeed { get; private set; }
    public float targetAngularSpeed { get; private set; }

	private void Awake() {
		m_Car = GetComponent<CarController> ();
	}

	public void Move (float accelUser, float steeringUser) {
		accel = accelUser;
		steering = steeringUser;
	}

	private void FixedUpdate() 
	{
		float carForwardSpeed = m_Car.speed;
		if (Mathf.Abs (carForwardSpeed) < m_ForwardSpeedThreshold) {
			carForwardSpeed = 0f;
			accelIntegral = 0f;
		}
			
		targetSpeed = accel * m_TopForwardSpeed;
		m_CurrentForwardError = targetSpeed - carForwardSpeed;

		accelProportional = m_PForward * m_CurrentForwardError / m_TopForwardSpeed;
		accelIntegral += (m_IForward * (m_CurrentForwardError + m_PreviousForwardError) / 2f * Time.deltaTime) / m_TopForwardSpeed;
		accelDerivative = (m_DForward * (m_CurrentForwardError - m_PreviousForwardError) / Time.deltaTime) / m_TopForwardSpeed;

		accelIntegral = Mathf.Clamp (accelIntegral, -1, 1);

		accelResult = Mathf.Clamp(accelProportional + accelIntegral + accelDerivative, -1, 1);


		float carAngularSpeed = m_Car.angularSpeed;
		if (Mathf.Abs (carAngularSpeed) < m_AngularSpeedThreshold) {
			carAngularSpeed = 0f;
			steeringIntegral = 0f;
		}

		targetAngularSpeed = steering * m_TopAngularSpeed;
		m_CurrentAngularError = targetAngularSpeed - carAngularSpeed;

		steeringProportional = m_PAngular * m_CurrentAngularError / m_TopAngularSpeed;
		steeringIntegral += (m_IAngular * (m_CurrentAngularError + m_PreviousAngularError) / 2f * Time.deltaTime) / m_TopAngularSpeed;
		steeringDerivative = (m_DAngular * (m_CurrentAngularError - m_PreviousAngularError) / Time.deltaTime) / m_TopAngularSpeed;

		steeringIntegral = Mathf.Clamp (steeringIntegral, -1, 1);

		steeringResult = Mathf.Clamp(steeringProportional + steeringIntegral + steeringDerivative, -1, 1);


		//Debug.Log ("target speed: " + targetSpeed);
		//Debug.Log ("accel: " + accel);

		m_Car.Move (accelResult, steeringResult);

		m_PreviousForwardError = m_CurrentForwardError;
		m_PreviousAngularError = m_CurrentAngularError;
	}
}
