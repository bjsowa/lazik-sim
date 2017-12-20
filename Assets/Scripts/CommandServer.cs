using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class CommandServer : MonoBehaviour {

	private SocketIOComponent _socket;
	private PidController _controller;

	// Use this for initialization
	void Start () {
		_controller = GameObject.Find ("łazik").GetComponent<PidController> ();

		_socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
		_socket.On("open", OnOpen);
		_socket.On("steer", OnSteer);
	}
	
	void OnOpen(SocketIOEvent obj)
	{
		Debug.Log("Connection Open");
		_socket.Emit ("test");
	}

	void OnSteer(SocketIOEvent obj)
	{
		JSONObject jsonObject = obj.data;
		//    print(float.Parse(jsonObject.GetField("steering_angle").str));
		float accel = float.Parse(jsonObject.GetField("accel").str);
		float steering = float.Parse(jsonObject.GetField("steering").str);
		_controller.Move (accel, steering);
	}
}
