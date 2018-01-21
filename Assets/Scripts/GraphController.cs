using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]
[RequireComponent(typeof(PidController))]
public class GraphController : MonoBehaviour
{
	private bool m_IsOn = false;

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
			if (m_IsOn) {
				m_IsOn = false;
				m_Graph.stopGraph ();
			} else {
				m_IsOn = true;
				m_Graph.startGraph ();
			}
		}

		if (m_IsOn) {
			Rect FLSpeedRect = new Rect (
                0f, 0f, 
				(2f / 6f) * Screen.width, .2f * Screen.height);
			Rect FLAccelRect = new Rect (
                (2f / 6f) * Screen.width, 0f, 
				(1f / 6f) * Screen.width, .2f * Screen.height);
            Rect FRSpeedRect = new Rect(
                (3f / 6f) * Screen.width, 0f,
                (2f / 6f) * Screen.width, .2f * Screen.height);
            Rect FRAccelRect = new Rect(
                (5f / 6f) * Screen.width, 0f,
                (1f / 6f) * Screen.width, .2f * Screen.height);
            Rect RLSpeedRect = new Rect(
                0f, .2f * Screen.height,
                (2f / 6f) * Screen.width, .2f * Screen.height);
            Rect RLAccelRect = new Rect(
                (2f / 6f) * Screen.width, .2f * Screen.height,
                (1f / 6f) * Screen.width, .2f * Screen.height);
            Rect RRSpeedRect = new Rect(
                (3f / 6f) * Screen.width, .2f * Screen.height,
                (2f / 6f) * Screen.width, .2f * Screen.height);
            Rect RRAccelRect = new Rect(
                (5f / 6f) * Screen.width, .2f * Screen.height,
                (1f / 6f) * Screen.width, .2f * Screen.height);

            Rect FixRect = new Rect (Screen.width, .2f * Screen.height, 
				(1f / 6f) * Screen.width, .2f * Screen.height);

			if (GraphManager.Graph != null)
            { 
				GraphManager.Graph.Plot("FLSpeedGraph", m_Car.WheelSpeed[1], Color.green, FLSpeedRect, -12f, 12f);
                GraphManager.Graph.Plot("FLAccelGraph", m_Pid.Result[1], Color.red, FLAccelRect, -1f, 1f);
                GraphManager.Graph.Plot("FRSpeedGraph", m_Car.WheelSpeed[0], Color.green, FRSpeedRect, -12f, 12f);
                GraphManager.Graph.Plot("FRAccelGraph", m_Pid.Result[0], Color.red, FRAccelRect, -1f, 1f);
                GraphManager.Graph.Plot("RLSpeedGraph", m_Car.WheelSpeed[3], Color.green, RLSpeedRect, -12f, 12f);
                GraphManager.Graph.Plot("RLAccelGraph", m_Pid.Result[3], Color.red, RLAccelRect, -1f, 1f);
                GraphManager.Graph.Plot("RRSpeedGraph", m_Car.WheelSpeed[2], Color.green, RRSpeedRect, -12f, 12f);
                GraphManager.Graph.Plot("RRAccelGraph", m_Pid.Result[2], Color.red, RRAccelRect, -1f, 1f);

                GraphManager.Graph.Plot ("fix", 0, Color.red, FixRect);
			}
		}
	}
}
