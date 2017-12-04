using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class GraphController : MonoBehaviour
{
	private int Count;

	private Rect SpeedRect;
	private Rect TestRect;

	private CarController m_Car;

	void Awake() {
		m_Car = GetComponent<CarController> ();
	}

	// Update is called once per frame
	void Update()
	{
		SpeedRect = new Rect(0.5f * Screen.width, 0f, 
							0.5f * Screen.width, .2f * Screen.height);

		if(GraphManager.Graph != null){
			GraphManager.Graph.Plot("Test_ScreenSpace1", m_Car.speed, Color.green, SpeedRect);
		}
	}
}
