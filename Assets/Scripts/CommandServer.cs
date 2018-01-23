using UnityEngine;
using SocketIO;
using System.Collections.Generic;
using System;
using System.Globalization;

public class CommandServer : MonoBehaviour {

    [SerializeField] private Camera m_FrontCamera;

	private SocketIOComponent m_Socket;
    private bool m_IsOpen = false;
    private bool m_Request = true;
    private CarController m_Car;
	private PidController m_Pid;
    private CarUserControl m_Control;

    private void Awake()
    {
        GameObject lazik = GameObject.FindWithTag("Player");
        m_Car = lazik.GetComponent<CarController>();
        m_Pid = lazik.GetComponent<PidController>();
        m_Control = lazik.GetComponent<CarUserControl>();
        m_Socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
    }

    void Start () {
		m_Socket.On("open", OnOpen);
		m_Socket.On("steer", OnSteer);
        m_Socket.On("request_telemetry", OnRequest);
	}
	
	void OnOpen(SocketIOEvent obj)
	{
		Debug.Log("Connection Open");
        m_IsOpen = true;
	}

	void OnSteer(SocketIOEvent obj)
	{
        if (m_Control.CurrentMode == CarUserControl.Mode.Autonomous)
        {
            JSONObject jsonObject = obj.data;

            float accel = float.Parse(jsonObject.GetField("accel").str, CultureInfo.InvariantCulture);
            float steering = float.Parse(jsonObject.GetField("steering").str, CultureInfo.InvariantCulture);
            m_Pid.Move(accel, steering);
        }
	}

    void OnRequest(SocketIOEvent obj)
    {
        //Debug.Log("Requested Telemetry");
        m_Request = true;
    }

    public byte[] CaptureFrame(Camera camera)
    {
        //force camera update 
        camera.Render();

        RenderTexture targetTexture = camera.targetTexture;
        RenderTexture.active = targetTexture;
        Texture2D texture2D = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);
        texture2D.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0);
        texture2D.Apply();
        byte[] image = texture2D.EncodeToJPG();
        DestroyImmediate(texture2D); // Required to prevent leaking the texture
        return image;
    }

    private void EmitTelemetry()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["speed"] = m_Car.Speed.ToString("N4", CultureInfo.InstalledUICulture);
        data["image"] = Convert.ToBase64String(CaptureFrame(m_FrontCamera));
        m_Socket.Emit("telemetry", new JSONObject(data));
    }

    private void Update()
    {
        if (m_IsOpen && m_Request)
        {
            EmitTelemetry();
            m_Request = false;
        }
    }
}
