using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]
[RequireComponent(typeof(PidController))]
public class GraphController : MonoBehaviour
{
	private bool isOn = false;

	private CarController m_Car;
	private PidController m_Pid;
	private GraphManager m_Graph;

	void Awake() {
		m_Car = GetComponent<CarController> ();
		m_Pid = GetComponent<PidController> ();
		GameObject camera = GameObject.FindWithTag ("MainCamera");
		m_Graph = camera.GetComponent<GraphManager> ();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Space) ) {
			if (isOn) {
				isOn = false;
				m_Graph.stopGraph ();
			} else {
				isOn = true;
				m_Graph.startGraph ();
			}
		}

		if (isOn) {
			Rect SpeedRect = new Rect (0f, 0f, 
				(2f / 6f) * Screen.width, .2f * Screen.height);
			Rect AccelRect = new Rect ((2f / 6f) * Screen.width, 0f, 
				(1f / 6f) * Screen.width, .2f * Screen.height);
			Rect ProportionalRect = new Rect ((3f / 6f) * Screen.width, 0f, 
				(1f / 6f) * Screen.width, .2f * Screen.height);
			Rect IntegralRect = new Rect ((4f / 6f) * Screen.width, 0f, 
				(1f / 6f) * Screen.width, .2f * Screen.height);
			Rect DerivativeRect = new Rect ((5f / 6f) * Screen.width, 0f, 
				(1f / 6f) * Screen.width, .2f * Screen.height);

			if (GraphManager.Graph != null) {
				GraphManager.Graph.Plot ("SpeedGraph", m_Car.speed, Color.green, SpeedRect, -20f, 20f);
				GraphManager.Graph.Plot ("AccelGraph", m_Pid.accel, Color.red, AccelRect, -1f, 1f);
				GraphManager.Graph.Plot ("ProportionalGraph", m_Pid.proportional, Color.red, ProportionalRect);
				GraphManager.Graph.Plot ("IntegralGraph", m_Pid.integral, Color.red, IntegralRect);
				GraphManager.Graph.Plot ("DerivativeGraph", m_Pid.derivative, Color.red, DerivativeRect);
			}
		}
	}
}
