using UnityEngine;
using SocketIO;
using System.Collections.Generic;
using System;

public class CommandServer : MonoBehaviour {

    [SerializeField] private Camera m_FrontCamera;

	private SocketIOComponent m_Socket;
    private bool m_IsOpen = false;
    private CarController m_Car;
	private PidController m_Pid;

    private void Awake()
    {
        GameObject lazik = GameObject.FindWithTag("Player");
        m_Car = lazik.GetComponent<CarController>();
        m_Pid = lazik.GetComponent<PidController>();
        m_Socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
    }

    void Start () {
		m_Socket.On("open", OnOpen);
		m_Socket.On("steer", OnSteer);
	}
	
	void OnOpen(SocketIOEvent obj)
	{
		Debug.Log("Connection Open");
        m_IsOpen = true;
	}

	void OnSteer(SocketIOEvent obj)
	{
		JSONObject jsonObject = obj.data;

		float accel = float.Parse(jsonObject.GetField("accel").str);
		float steering = float.Parse(jsonObject.GetField("steering").str);
		m_Pid.Move (accel, steering);
	}

    public byte[] CaptureFrame(Camera camera)
    {
        RenderTexture targetTexture = camera.targetTexture;
        RenderTexture.active = targetTexture;
        Texture2D texture2D = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);
        texture2D.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0);
        texture2D.Apply();
        byte[] image = texture2D.EncodeToJPG();
        DestroyImmediate(texture2D); // Required to prevent leaking the texture
        return image;
    }

    private void Update()
    {
        if (m_IsOpen)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["speed"] = m_Car.speed.ToString("N4");
            data["angularSpeed"] = m_Car.angularSpeed.ToString("N4");
            data["image"] = Convert.ToBase64String(CaptureFrame(m_FrontCamera));
            m_Socket.Emit("telemetry", new JSONObject(data));
        }
    }
}
