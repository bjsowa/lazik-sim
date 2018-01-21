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

    // Input Values
    public float Accel { get; private set; }
    public float Steering { get; private set; }

    // Output Values
    public float[] Proportion { get; private set; }
	public float[] Integral { get; private set; }
    public float[] Derivative { get; private set; }
    public float[] Errors { get; private set; }
    public float[] Result { get; private set; }

	private void Awake()
    {
		m_Car = GetComponent<CarController> ();
	}

    private void Start()
    {
        Proportion = new float[] { 0f, 0f, 0f, 0f };
        Integral = new float[] { 0f, 0f, 0f, 0f };
        Derivative = new float[] { 0f, 0f, 0f, 0f };
        Errors = new float[] { 0f, 0f, 0f, 0f };
        Result = new float[] { 0f, 0f, 0f, 0f };
        Accel = 0f;
        Steering = 0f;
    }

    public void Move (float accelUser, float steeringUser) {
		Accel = Mathf.Clamp(accelUser, -1f, 1f);
		Steering = Mathf.Clamp(steeringUser, -1f, 1f);
	}

	private void Update() 
	{
        for (int i = 0; i < 4; i++)
        {
            float speed = m_Car.WheelSpeed[i];

            if (Mathf.Abs(speed) < m_SpeedThreshold)
            {
                speed = 0f;
                Integral[i] = 0f;
            }

            float targetAccel;
            if (i == 0 || i == 2)
                targetAccel = Mathf.Clamp(Accel - Steering, -1f, 1f);
            else
                targetAccel = Mathf.Clamp(Accel + Steering, -1f, 1f);

            float targetSpeed = targetAccel * m_TopSpeed;
            float newError = targetSpeed - speed;

            Proportion[i] = m_P * newError / m_TopSpeed;
            Integral[i] = (m_I * (newError + Errors[i]) / 2f * Time.deltaTime) / m_TopSpeed;
            Derivative[i] = (m_D * (newError - Errors[i]) / Time.deltaTime) / m_TopSpeed;

            // prevent
            Integral[i] = Mathf.Clamp(Integral[i], -1, 1);

            Result[i] = Mathf.Clamp(Proportion[i] + Integral[i] + Derivative[i], -1, 1);

            m_Car.ApplyTorque(i, Result[i]);
        }
	}
}
