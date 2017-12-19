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
			Rect ForwardSpeedRect = new Rect (0f, 0f, 
				(2f / 6f) * Screen.width, .2f * Screen.height);
			Rect AccelRect = new Rect ((2f / 6f) * Screen.width, 0f, 
				(1f / 6f) * Screen.width, .2f * Screen.height);
			Rect AccelProportionalRect = new Rect ((3f / 6f) * Screen.width, 0f, 
				(1f / 6f) * Screen.width, .2f * Screen.height);
			Rect AccelIntegralRect = new Rect ((4f / 6f) * Screen.width, 0f, 
				(1f / 6f) * Screen.width, .2f * Screen.height);
			Rect AccelDerivativeRect = new Rect ((5f / 6f) * Screen.width, 0f, 
				(1f / 6f) * Screen.width, .2f * Screen.height);

			Rect AngularSpeedRect = new Rect (0f, .2f * Screen.height, 
				(2f / 6f) * Screen.width, .2f * Screen.height);
			Rect SteeringRect = new Rect ((2f / 6f) * Screen.width, .2f * Screen.height, 
				(1f / 6f) * Screen.width, .2f * Screen.height);
			Rect SteeringProportionalRect = new Rect ((3f / 6f) * Screen.width, .2f * Screen.height, 
				(1f / 6f) * Screen.width, .2f * Screen.height);
			Rect SteeringIntegralRect = new Rect ((4f / 6f) * Screen.width, .2f * Screen.height, 
				(1f / 6f) * Screen.width, .2f * Screen.height);
			Rect SteeringDerivativeRect = new Rect ((5f / 6f) * Screen.width, .2f * Screen.height, 
				(1f / 6f) * Screen.width, .2f * Screen.height);

			Rect FixRect = new Rect (Screen.width, .2f * Screen.height, 
				(1f / 6f) * Screen.width, .2f * Screen.height);

			if (GraphManager.Graph != null) {
				GraphManager.Graph.Plot ("ForwardSpeedGraph", m_Car.speed, Color.green, ForwardSpeedRect, -20f, 20f);
				GraphManager.Graph.Plot ("AccelGraph", m_Pid.accelResult, Color.red, AccelRect, -1f, 1f);
				GraphManager.Graph.Plot ("AccelProportionalGraph", m_Pid.accelProportional, Color.red, AccelProportionalRect);
				GraphManager.Graph.Plot ("AccelIntegralGraph", m_Pid.accelIntegral, Color.red, AccelIntegralRect);
				GraphManager.Graph.Plot ("AccelDerivativeGraph", m_Pid.accelDerivative, Color.red, AccelDerivativeRect);

				GraphManager.Graph.Plot ("AngularSpeedGraph", m_Car.angularSpeed, Color.green, AngularSpeedRect, -1f, 1f);
				GraphManager.Graph.Plot ("SteeringGraph", m_Pid.steeringResult, Color.red, SteeringRect, -1f, 1f);
				GraphManager.Graph.Plot ("SteeringProportionalGraph", m_Pid.steeringProportional, Color.red, SteeringProportionalRect);
				GraphManager.Graph.Plot ("SteeringIntegralGraph", m_Pid.steeringIntegral, Color.red, SteeringIntegralRect);
				GraphManager.Graph.Plot ("SteeringDerivativeGraph", m_Pid.steeringDerivative, Color.red, SteeringDerivativeRect);

				GraphManager.Graph.Plot ("fix", 0, Color.red, FixRect);
			}
		}
	}
}
