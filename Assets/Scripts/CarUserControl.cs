using UnityEngine;
using System.Collections;


[RequireComponent(typeof(CarController))]
public class CarUserControl : MonoBehaviour
{
    private CarController m_Car;

    private void Awake()
    {
        m_Car = GetComponent<CarController>();
    }

    private void FixedUpdate()
    {
		float v = Input.GetAxis ("Vertical");
		float h = Input.GetAxis ("Horizontal");
		m_Car.Move(v, h);
    }
}
