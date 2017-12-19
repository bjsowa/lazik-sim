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

	private float previousForwardError = 0f;
	private float currentForwardError = 0f;

	private float previousAngularError = 0f;
	private float currentAngularError = 0f;

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

	void Awake() {
		m_Car = GetComponent<CarController> ();
	}
		
	// Update is called once per frame
	public void Move (float accelUser, float steeringUser) {

		accel = accelUser;
		steering = steeringUser;
		
		float carForwardSpeed = m_Car.speed;
		if (Mathf.Abs (carForwardSpeed) < m_ForwardSpeedThreshold) {
			carForwardSpeed = 0f;
			accelIntegral = 0f;
		}
			
		float targetForwardSpeed = accel * m_TopForwardSpeed;
		currentForwardError = targetForwardSpeed - carForwardSpeed;

		accelProportional = m_PForward * currentForwardError / m_TopForwardSpeed;
		accelIntegral += (m_IForward * (currentForwardError + previousForwardError) / 2f * Time.deltaTime) / m_TopForwardSpeed;
		accelDerivative = (m_DForward * (currentForwardError - previousForwardError) / Time.deltaTime) / m_TopForwardSpeed;

		accelIntegral = Mathf.Clamp (accelIntegral, -1, 1);

		accelResult = Mathf.Clamp(accelProportional + accelIntegral + accelDerivative, -1, 1);


		float carAngularSpeed = m_Car.angularSpeed;
		if (Mathf.Abs (carAngularSpeed) < m_AngularSpeedThreshold) {
			carAngularSpeed = 0f;
			steeringIntegral = 0f;
		}

		float targetAngularSpeed = steering * m_TopAngularSpeed;
		currentAngularError = targetAngularSpeed - carAngularSpeed;

		steeringProportional = m_PAngular * currentAngularError / m_TopAngularSpeed;
		steeringIntegral += (m_IAngular * (currentAngularError + previousAngularError) / 2f * Time.deltaTime) / m_TopAngularSpeed;
		steeringDerivative = (m_DAngular * (currentAngularError - previousAngularError) / Time.deltaTime) / m_TopAngularSpeed;

		steeringIntegral = Mathf.Clamp (steeringIntegral, -1, 1);

		steeringResult = Mathf.Clamp(steeringProportional + steeringIntegral + steeringDerivative, -1, 1);


		//Debug.Log ("target speed: " + targetSpeed);
		//Debug.Log ("accel: " + accel);

		m_Car.Move (accelResult, steeringResult);

		previousForwardError = currentForwardError;
		previousAngularError = currentAngularError;
	}
}
