using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class PidController : MonoBehaviour 
{
	private CarController m_Car;
	[SerializeField] private float m_TopSpeed = 30f;
	[SerializeField] private float m_P = 1f;
	[SerializeField] private float m_I = 1f;
	[SerializeField] private float m_D = 1f;
	[SerializeField] private float m_SpeedThreshold = 1f;

	private float previousError = 0f;
	private float currentError = 0f;

	[HideInInspector] public float proportional = 0f;
	[HideInInspector] public float integral = 0f;
	[HideInInspector] public float derivative = 0f;
	[HideInInspector] public float accel = 0f;

	void Awake() {
		m_Car = GetComponent<CarController> ();
	}

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	public void Move (float speedFactor, float steering) {
		float carSpeed = m_Car.speed;
		if (Mathf.Abs (carSpeed) < m_SpeedThreshold) {
			carSpeed = 0f;
			integral = 0f;
		}
			
		float targetSpeed = speedFactor * m_TopSpeed;
		currentError = targetSpeed - carSpeed;

		proportional = m_P * currentError / m_TopSpeed;
		integral += (m_I * (currentError + previousError) / 2f * Time.deltaTime) / m_TopSpeed;
		derivative = (m_D * (currentError - previousError) / Time.deltaTime) / m_TopSpeed;

		accel = Mathf.Clamp(proportional + integral + derivative, -1, 1);

		m_Car.Move (accel, steering);

		//Debug.Log ("target speed: " + targetSpeed);
		//Debug.Log ("accel: " + accel);

		previousError = currentError;
	}
}
