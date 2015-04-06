using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	private int m_numCameras = 0;
	public string deviceName;
	WebCamDevice[] devices;
	private string m_message = "";

	// Use this for initialization
	void Start () {
		m_numCameras = CameraParametersAndroid.numCameras;
		print (m_numCameras);

		devices = WebCamTexture.devices;
		deviceName = devices[0].name;
	}
	
	// Update is called once per frame
	void Update () {
		m_numCameras = CameraParametersAndroid.numCameras;
	}

	public void receiveMessage(string message) {
		m_message = message;
	}

	void OnGUI() {
		GUI.Label(new Rect(10, 10, 100, 20), m_numCameras.ToString());
		GUI.Label(new Rect(10, 40, 100, 20), deviceName);
		GUI.Label(new Rect(10, 70, 100, 20), devices[0].isFrontFacing.ToString());
		GUI.Label(new Rect(10, 100, 100, 20), m_message);
		//GUI.Label(new Rect(10, 100, 100, 20), wct.isPlaying.ToString());
	}
}
