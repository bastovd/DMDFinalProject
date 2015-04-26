

using UnityEngine;
using System.Collections;

public class CamTexture : MonoBehaviour {

	private int m_numCameras = 0;
	public string deviceName;
	WebCamDevice[] devices;
	private string m_message = "default";
	private GameObject m_player;
	private Vector3 m_posState = new Vector3(0, 0, 0);
	private float m_lightPosRadius = 5.0f;
	private float m_lightAngle = 45.0f;
	private float m_pi = 3.1415926f;
	private Quaternion m_rotState = new Quaternion();
	private string[] m_luminosityValues = new string[7];
	public bool m_isReady = false;
	
	GameObject lightGameObjectL;
	GameObject lightGameObjectR;
	GameObject backLight;

	void Awake() {
		/*Vector3 prev_rot = new Vector3(
			Camera.main.transform.rotation[0],
			Camera.main.transform.rotation[1],
			Camera.main.transform.rotation[2]);
		Camera.main.transform.rotation =  
			new Quaternion (prev_rot[0] - 0.1f, 
			                prev_rot[1], 
			                prev_rot[2],
			                Camera.main.transform.rotation[3]);*/
	}
	// Use this for initialization
	void Start () {

		//setAutoExposure ();

		m_player = GameObject.Find ("Player");

		lightGameObjectL = new GameObject ("The Light1");
		lightGameObjectL.AddComponent<Light> ();
		lightGameObjectL.light.type = LightType.Directional;
		lightGameObjectL.light.intensity = 0f;

		//lightGameObjectL.light.range = 30.0f;
		lightGameObjectR = new GameObject ("The Light2");
		lightGameObjectR.AddComponent<Light> ();
		lightGameObjectR.light.type = LightType.Directional;
		lightGameObjectR.light.intensity = 0f;
		//lightGameObjectR.light.range = 30.0f;
		
		backLight = new GameObject ("Back Light");
		backLight.AddComponent<Light> ();
		backLight.light.type = LightType.Spot;
		backLight.light.intensity = 0f;
		backLight.light.range = 36f;
		
		float W = Screen.width;
		float H = Screen.height;
		float InsetX = -(W / 2);
		float InsetY = -(H / 2);
		guiTexture.pixelInset = new Rect (InsetX, InsetY, W, H);

		//******test
		//testLights ();

		//***** end test

		//devices = WebCamTexture.devices;
		//m_numCameras = devices.Length;
		//deviceName = devices[0].name;
		//WebCamTexture wct = new WebCamTexture(deviceName);
		//guiTexture.texture = wct;
		//wct.Play();
	}

	void testLights() {
		float angleH1 = (float) 25;
		angleH1 = m_pi - angleH1 * m_pi / 180.0f  + m_pi / 2f;
		float angleV1 = (float)0;
		angleV1 = m_pi + (float) angleV1 * m_pi / 180f + m_pi / 2f;
		//angleV1 = angleV1 * m_pi / 180.0f;
		Vector3 lightPos1 = new Vector3(
			m_player.transform.position.x + m_lightPosRadius * Mathf.Cos(angleH1) * Mathf.Sin (angleV1),
			m_player.transform.position.y + m_lightPosRadius * Mathf.Cos (angleV1),
			m_player.transform.position.z + m_lightPosRadius * Mathf.Sin(angleH1) * Mathf.Sin (angleV1));
		lightGameObjectR.transform.position = lightPos1;
		lightGameObjectR.transform.rotation = new Quaternion ();
		lightGameObjectR.transform.LookAt (m_player.transform);
		lightGameObjectR.light.intensity = 0.5f;
		
		float angleH2 = (float) -130;
		angleH2 = m_pi - angleH2 * m_pi / 180.0f + m_pi / 2;
		float angleV2 = (float) 20;
		angleV2 = m_pi + (float) angleV2 * m_pi / 180f + m_pi / 2f;
		Vector3 lightPos2 = new Vector3(
			m_player.transform.position.x + m_lightPosRadius * Mathf.Cos(angleH2) * Mathf.Sin (angleV2),
			m_player.transform.position.y + m_lightPosRadius * Mathf.Cos (angleV2),
			m_player.transform.position.z + m_lightPosRadius * Mathf.Sin(angleH2) * Mathf.Sin (angleV2));
		lightGameObjectL.transform.position = lightPos2;
		lightGameObjectL.transform.rotation = new Quaternion ();
		lightGameObjectL.transform.LookAt (m_player.transform);
		lightGameObjectL.light.intensity = 0.5f;
	}

	public void receiveMessage(string message) {
		m_message = message;
	}

	public void saveCurrPosRotState(string message) {
		int mId = int.Parse (message);
		if (mId == 1) {
			m_posState = m_player.transform.position;
			m_rotState = m_player.transform.rotation;
		}
	}

	// nose : 3 values, left - middle - right
	// forehead: 2 values, bottom - top
	// cheeks: 2 values, left - right
	// light colors: 6 values, 3 RGB for left, 3 RGB for right
	// overall min ambient: 1 value
	// cheeks min ambient: 2 valuea, left - right
	// back light pos and color: 5 values, 2 for pos, 3 for RGB
	public void receiveLuminosityVals (string message) {
		m_message = message;
		m_luminosityValues = message.Split (',');
		int leftN = int.Parse (m_luminosityValues [0]);
		int midN = int.Parse (m_luminosityValues [1]);
		int rightN = int.Parse (m_luminosityValues [2]);
		int botF = int.Parse (m_luminosityValues [3]);
		int topF = int.Parse (m_luminosityValues [4]);
		int leftC = int.Parse (m_luminosityValues [5]);
		int rightC = int.Parse (m_luminosityValues [6]);
		int rL = int.Parse (m_luminosityValues [7]);
		int gL = int.Parse (m_luminosityValues [8]);
		int bL = int.Parse (m_luminosityValues [9]);
		int rR = int.Parse (m_luminosityValues [10]);
		int gR = int.Parse (m_luminosityValues [11]);
		int bR = int.Parse (m_luminosityValues [12]);
		int ambient = int.Parse (m_luminosityValues [13]);
		int ambL = int.Parse (m_luminosityValues [14]);
		int ambR = int.Parse (m_luminosityValues [15]);
		int posX = int.Parse (m_luminosityValues [16]);
		int posY = int.Parse (m_luminosityValues [17]);
		int backR = int.Parse (m_luminosityValues [18]);
		int backG = int.Parse (m_luminosityValues [19]);
		int backB = int.Parse (m_luminosityValues [20]);
		int backCount = int.Parse (m_luminosityValues [21]);
		int azimuth1 = int.Parse (m_luminosityValues [22]);
		int elevation1 = int.Parse (m_luminosityValues [23]);
		int azimuth2 = int.Parse (m_luminosityValues [24]);
		int elevation2 = int.Parse (m_luminosityValues [25]);


		if (posX > 0 && posY > 0) {
			float spotA = (float) backCount / 432f / 768f * 180f * 10f;
			if (spotA < 1) spotA = 1f;
			if (spotA > 179) spotA = 179f;
			backLight.light.spotAngle = (int) spotA;
			backLight.light.color = new Color((float) backR / 255f,
			                                  (float) backG / 255f,
			                                  (float) backB / 255f);
			backLight.light.intensity = (float) (backR + backG + backB) / 3f / 255f;
			float angleH = (float) posX / 768f * m_pi;
			float angleV = (float) (posY - 432/2) / 432f * m_pi;
			Vector3 lightPos = new Vector3(
				//m_player.transform.position.x - m_lightPosRadius * Mathf.Cos(angleH) * Mathf.Sin (angleV),
				//m_player.transform.position.y + m_lightPosRadius * Mathf.Cos (angleV),
				//m_player.transform.position.z + m_lightPosRadius * Mathf.Sin(angleH) * Mathf.Sin (angleV));
				m_player.transform.position.x,
				m_player.transform.position.y + 2,
				m_player.transform.position.z - m_lightPosRadius);
			backLight.transform.position = lightPos;
			backLight.transform.rotation = new Quaternion ();
			backLight.transform.LookAt (m_player.transform);
		}

		bool secondLight = false;
		if (ambL > ambient && ambR > ambient) {
			secondLight = true;
		}

		GameObject light1;
		GameObject light2;
		if (leftC > rightC) {
			float ratio = 1f;
			if (!secondLight) {
				//ratio =(float) leftC / rightC - 1;
				ratio =(float) leftN / ambL - 1;
			} else {
				ratio =(float) leftN / ambL - 1;
			}
			float angle = m_lightAngle * ratio;
			angle = angle * m_pi / 180.0f + m_pi / 2f;
			Vector3 lightPos = new Vector3(
				m_player.transform.position.x - m_lightPosRadius * Mathf.Cos(angle),
				m_player.transform.position.y + 5,
				m_player.transform.position.z + m_lightPosRadius * Mathf.Sin(angle));
			lightGameObjectL.transform.position = lightPos;
			lightGameObjectL.transform.rotation = new Quaternion ();
			lightGameObjectL.transform.LookAt (m_player.transform);

			float rf = (float) rL / 255.0f;
			float gf = (float) gL / 255.0f;
			float bf = (float) bL / 255.0f;
			lightGameObjectL.light.color = new Color(rf, gf, bf);
			lightGameObjectL.light.intensity = (rf + gf + bf)/3.0f;

			light1 = lightGameObjectL;
			light2 = lightGameObjectR;
			if (secondLight) {
				ratio =(float) rightN / ambR - 1;
				angle = m_lightAngle * ratio;
				angle = angle * m_pi / 180.0f + m_pi / 2f;
				lightPos = new Vector3(
				m_player.transform.position.x + m_lightPosRadius * Mathf.Cos(angle),
				m_player.transform.position.y + 5,
				m_player.transform.position.z + m_lightPosRadius * Mathf.Sin(angle));
				lightGameObjectR.transform.position = lightPos;

				lightGameObjectR.transform.rotation = new Quaternion ();
				lightGameObjectR.transform.LookAt (m_player.transform);
				
				rf = (float) rR / 255.0f;
				gf = (float) gR / 255.0f;
				bf = (float) bR / 255.0f;
				lightGameObjectR.light.color = new Color(rf, gf, bf);
				lightGameObjectR.light.intensity = (rf + gf + bf)/3.0f;
			}
		} else {
			float ratio = 1f;
			if (!secondLight) {
				//ratio =(float) rightC / leftC - 1;
				ratio =(float) rightN / ambR - 1;
			} else {
				ratio =(float) rightN / ambR - 1;
			}
			float angle = m_lightAngle * ratio;
			angle = angle * m_pi / 180.0f  + m_pi / 2f;
			Vector3 lightPos = new Vector3(
				m_player.transform.position.x + m_lightPosRadius * Mathf.Cos(angle),
				m_player.transform.position.y + 5,
				m_player.transform.position.z + m_lightPosRadius * Mathf.Sin(angle));
			lightGameObjectR.transform.position = lightPos;
			lightGameObjectR.transform.rotation = new Quaternion ();
			lightGameObjectR.transform.LookAt (m_player.transform);

			float rf = (float) rR / 255.0f;
			float gf = (float) gR / 255.0f;
			float bf = (float) bR / 255.0f;
			lightGameObjectR.light.color = new Color(rf, gf, bf);
			lightGameObjectR.light.intensity = (rf + gf + bf)/3.0f;

			light1 = lightGameObjectR;
			light2 = lightGameObjectL;
			if (secondLight) {
				ratio =(float) leftN / ambL - 1;
				angle = m_lightAngle * ratio;
				angle = angle * m_pi / 180.0f + m_pi / 2f;
				lightPos = new Vector3(
					m_player.transform.position.x - m_lightPosRadius * Mathf.Cos(angle),
					m_player.transform.position.y + 5,
					m_player.transform.position.z + m_lightPosRadius * Mathf.Sin(angle));
				lightGameObjectL.transform.position = lightPos;

				lightGameObjectL.transform.rotation = new Quaternion ();
				lightGameObjectL.transform.LookAt (m_player.transform);
				
				rf = (float) rL / 255.0f;
				gf = (float) gL / 255.0f;
				bf = (float) bL / 255.0f;
				lightGameObjectL.light.color = new Color(rf, gf, bf);
				lightGameObjectL.light.intensity = (rf + gf + bf)/3.0f;
			}
		}

		//Lights from Masks, EXPERIMENTAL
		// ************************** //
		float angleH1 = (float) azimuth1;
		angleH1 = m_pi - angleH1 * m_pi / 180.0f  + m_pi / 2f;
		float angleV1 = (float)elevation1;
		angleV1 = m_pi + (float) angleV1 * m_pi / 180f + m_pi / 2f;
		//angleV1 = angleV1 * m_pi / 180.0f;
		Vector3 lightPos1 = new Vector3(
			m_player.transform.position.x + m_lightPosRadius * Mathf.Cos(angleH1) * Mathf.Sin (angleV1),
			m_player.transform.position.y + m_lightPosRadius * Mathf.Cos (angleV1),
			m_player.transform.position.z + m_lightPosRadius * Mathf.Sin(angleH1) * Mathf.Sin (angleV1));
		light1.transform.position = lightPos1;
		light1.transform.rotation = new Quaternion ();
		light1.transform.LookAt (m_player.transform);
		
		float angleH2 = (float) azimuth2;
		angleH2 = m_pi - angleH2 * m_pi / 180.0f + m_pi / 2;
		float angleV2 = (float) elevation2;
		angleV2 = m_pi + (float) angleV2 * m_pi / 180f + m_pi / 2f;
		Vector3 lightPos2 = new Vector3(
			m_player.transform.position.x + m_lightPosRadius * Mathf.Cos(angleH2) * Mathf.Sin (angleV2),
			m_player.transform.position.y + m_lightPosRadius * Mathf.Cos (angleV2),
			m_player.transform.position.z + m_lightPosRadius * Mathf.Sin(angleH2) * Mathf.Sin (angleV2));
		light2.transform.position = lightPos2;
		light2.transform.rotation = new Quaternion ();
		light2.transform.LookAt (m_player.transform);
		// ************************* // END EXPERIMENTAL

		// ambient light
		float af = (float) ambient / 255.0f;
		RenderSettings.ambientLight = new Color (af, af, af);
		Debug.Log (RenderSettings.ambientLight.ToString());
		Debug.Log (lightGameObjectL.light.intensity.ToString());

		if ((lightGameObjectL.light.intensity + lightGameObjectR.light.intensity +
		     backLight.light.intensity + af) > 1f) {
			float lightIntensity = 1f - af;
			float leftLight = 
				lightGameObjectL.light.intensity * lightIntensity /
					(lightGameObjectL.light.intensity + lightGameObjectR.light.intensity + 
					 backLight.light.intensity + af);
			float rightLight = 
				lightGameObjectR.light.intensity * lightIntensity /
					(lightGameObjectL.light.intensity + lightGameObjectR.light.intensity + 
					 backLight.light.intensity + af);
			float bLight = 
				backLight.light.intensity * lightIntensity /
					(lightGameObjectL.light.intensity + lightGameObjectR.light.intensity + 
					 backLight.light.intensity + af);
			lightGameObjectL.light.intensity = leftLight;
			lightGameObjectR.light.intensity = rightLight;
			backLight.light.intensity = bLight;
		}
	}

	private void normalizeLight(GameObject lightSource1, float ambient) {
		Color lightSource1Color = lightSource1.light.color;
		float l1Lum = (float)(lightSource1Color.r + 
						lightSource1Color.g +
						lightSource1Color.b) / 3.0f;
		float lumSum = l1Lum + ambient;
		l1Lum = l1Lum / lumSum;
		ambient = ambient / lumSum;

		RenderSettings.ambientLight = new Color (ambient, ambient, ambient);
	}

	public void setAutoExposure() {
		AndroidJavaClass cam = new AndroidJavaClass ("android.hardware.Camera");
		//AndroidJavaClass parameters = new AndroidJavaClass ("android.hardware.Camera.Parameters");
		AndroidJavaObject camInstance = cam.CallStatic<AndroidJavaObject> ("open", 0);
		AndroidJavaObject camParams = camInstance.Call<AndroidJavaObject> ("getParameters");
		camParams.Call ("setAutoExposureLock", true);
		camInstance.Call ("setParameters", camParams);
	}

	// Update is called once per frame
	void Update () {
		if (m_isReady) {

		}
	}

	void OnGUI() {
		//GUI.Label(new Rect(10, 10, 100, 20), m_numCameras.ToString());
		//GUI.Label(new Rect(10, 40, 100, 20), deviceName);
		//GUI.Label(new Rect(10, 100, 100, 20), wct.isPlaying.ToString());
		if(GUI.Button(new Rect(0,0,200,200),"Start Video")) {
			devices = WebCamTexture.devices;
			m_numCameras = devices.Length;
			deviceName = devices[0].name;
			WebCamTexture wct = new WebCamTexture(deviceName);
			guiTexture.texture = wct;
			wct.Play();
		}
		GUI.Label(new Rect(10, 220, 1000, 200), RenderSettings.ambientLight.ToString());
		GUI.Label(new Rect(10, 250, 1000, 200), lightGameObjectL.light.color.ToString());
		GUI.Label(new Rect(10, 280, 1000, 200), m_message);
	}
}
