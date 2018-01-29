using UnityEngine;
using SocketIO;
using System.Collections.Generic;
using System;
using System.Globalization;

public class CommandServer : MonoBehaviour {

    [SerializeField] private MyCamera[] m_Cameras;

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

    private void EmitTelemetry()
    {
        DateTime date1 = DateTime.Now;

        Dictionary<string, string> data = new Dictionary<string, string>();
        data["speed"] = m_Car.Speed.ToString("N4", CultureInfo.InstalledUICulture);

        foreach( MyCamera cam in m_Cameras )
            data[cam.Name] = Convert.ToBase64String(cam.CaptureFrame());

        DateTime date2 = DateTime.Now;
        long elapsed = date2.Ticks - date1.Ticks;
        TimeSpan elapsedSpan = new TimeSpan(elapsed);

        Debug.Log("capturing: " + elapsedSpan.TotalSeconds.ToString("N3"));

        m_Socket.Emit("telemetry", new JSONObject(data));

        DateTime date3 = DateTime.Now;
        elapsed = date3.Ticks - date2.Ticks;
        elapsedSpan = new TimeSpan(elapsed);

        Debug.Log("emitting: " + elapsedSpan.TotalSeconds.ToString("N3"));

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
