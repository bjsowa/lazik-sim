using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class GraphController : MonoBehaviour
{
	private bool isOn = false;

	private CarController m_Car;
	private GraphManager m_Graph;

	void Awake() {
		m_Car = GetComponent<CarController> ();
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
			Rect SpeedRect = new Rect (0.5f * Screen.width, 0f, 
				0.5f * Screen.width, .2f * Screen.height);

			if (GraphManager.Graph != null) {
				GraphManager.Graph.Plot ("SpeedGraph", m_Car.speed, Color.green, SpeedRect);
			}
		}
	}
}
