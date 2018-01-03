using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class CommandServer : MonoBehaviour {

	private SocketIOComponent m_Socket;
	private PidController m_Controller;

	// Use this for initialization
	void Start () {
		m_Controller = GameObject.FindWithTag ("Player").GetComponent<PidController> ();

		m_Socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
		m_Socket.On("open", OnOpen);
		m_Socket.On("steer", OnSteer);
	}
	
	void OnOpen(SocketIOEvent obj)
	{
		Debug.Log("Connection Open");
		m_Socket.Emit ("test");
	}

	void OnSteer(SocketIOEvent obj)
	{
		JSONObject jsonObject = obj.data;
		//    print(float.Parse(jsonObject.GetField("steering_angle").str));
		float accel = float.Parse(jsonObject.GetField("accel").str);
		float steering = float.Parse(jsonObject.GetField("steering").str);
		m_Controller.Move (accel, steering);
	}
}
