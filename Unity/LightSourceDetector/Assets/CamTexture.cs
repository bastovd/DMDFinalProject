using UnityEngine;
using System.Collections;

public class CamTexture : MonoBehaviour {

	/*------------------------*/
	/// <summary>
	/// experimental AR vars
	/// </summary>
	/*public float m_frustumLength = 3.0f; // HACK : Chase
	public Material m_screenMaterial;
	
	private Texture2D m_texture;
	private float m_frustumHeight = 0.0f;
	private float m_frustumWidth = 0.0f;
	private Vector3[] m_frustumPoints;
	private Mesh m_mesh;
	private MeshFilter m_meshFilter;
	//private TangoApplication m_tangoApplication;
	private bool m_readyToDraw = false;
	
	/// <summary>
	/// Gets the frustum points of AR Screen.
	/// </summary>
	/// <returns>Retursn the frustum points of AR Screen.</returns>
	public Vector3[] GetFrustumPoints()
	{
		return m_frustumPoints;
	}
	/*-------------------*/

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

	/*private void Awake()
	{
		///////object init//////////////////
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
		//////////////////////////

		m_frustumPoints = new Vector3[5];
		
		m_meshFilter = GetComponent<MeshFilter>();
		m_mesh = new Mesh();
		ResizeScreen();
		
		transform.rotation = Quaternion.identity;
		
		m_texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
		
		m_texture.Apply();
		if(m_screenMaterial != null)
		{
			m_screenMaterial.mainTexture = m_texture;
			m_meshFilter.renderer.material = m_screenMaterial;
			//VideoOverlayProvider.ExperimentalConnectTexture(TangoEnums.TangoCameraId.TANGO_CAMERA_COLOR, m_texture.GetNativeTextureID(), _OnUnityFrameAvailable);
			Debug.Log("Texture set!");
		}
		
		//m_tangoApplication = FindObjectOfType<Tango.TangoApplication>();
		
		/*if(m_tangoApplication != null)
		{
			m_tangoApplication.RegisterPermissionsCallback(_OnTangoApplicationPermissionsEvent);
		}*/
	/*}*/
	
	/// <summary>
	/// Creates a plane and sizes it for the AR Screen.
	/// </summary>
	/*public void ResizeScreen()
	{
		m_frustumHeight = 2.0f * m_frustumLength * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
		m_frustumWidth = m_frustumHeight * camera.aspect;
		
		m_frustumPoints[0] = Vector3.zero;
		m_frustumPoints[1] = new Vector3(-m_frustumWidth, -m_frustumHeight, m_frustumLength);
		m_frustumPoints[2] = new Vector3(-m_frustumWidth, m_frustumHeight, m_frustumLength);
		m_frustumPoints[3] = new Vector3(m_frustumWidth, -m_frustumHeight, m_frustumLength);
		m_frustumPoints[4] = new Vector3(m_frustumWidth, m_frustumHeight, m_frustumLength);
		
		transform.position = new Vector3(0.0f, 0.0f, m_frustumLength);
		
		// verts
		Vector3[] verts = new Vector3[6];
		verts[0] = m_frustumPoints[1] + transform.position;
		verts[1] = m_frustumPoints[2] + transform.position;
		verts[2] = m_frustumPoints[3] + transform.position;
		verts[3] = m_frustumPoints[3] + transform.position;
		verts[4] = m_frustumPoints[2] + transform.position;
		verts[5] = m_frustumPoints[4] + transform.position;
		
		// indices
		int[] indices = new int[6];
		indices[0] = 0;
		indices[1] = 1;
		indices[2] = 2;
		indices[3] = 3;
		indices[4] = 4;
		indices[5] = 5;
		
		// uvs
		Vector2[] uvs = new Vector2[6];
		uvs[0] = new Vector2(0, 0);
		uvs[1] = new Vector2(0, 1f);
		uvs[2] = new Vector2(1f, 0);
		uvs[3] = new Vector2(1f, 0);
		uvs[4] = new Vector2(0, 1f);
		uvs[5] = new Vector2(1f, 1f);
		
		m_mesh.Clear();
		m_mesh.vertices = verts;
		m_mesh.triangles = indices;
		m_mesh.uv = uvs;
		m_meshFilter.mesh = m_mesh;
		m_mesh.RecalculateNormals();
	}
	
	/// <summary>
	/// Event handler for tango application permissions event.
	/// </summary>
	/// <param name="permissionsGranted">If set to <c>true</c> permissions granted.</param>
	private void _OnTangoApplicationPermissionsEvent(bool permissionsGranted)
	{
		m_readyToDraw = true;
	}*/
	
	/// <summary>
	/// Event handler for unity frame available.
	/// </summary>
	/// <param name="callbackContext">Callback context.</param>
	/// <param name="cameraId">Camera identifier.</param>
	private void _OnUnityFrameAvailable(System.IntPtr callbackContext, Tango.TangoEnums.TangoCameraId cameraId)
	{
		// Do fun stuff here!
	}
	
	/// <summary>
	/// Raises the pre render event.
	/// </summary>
	/*void OnPreRender()
	{
		if(m_readyToDraw)
		{
			VideoOverlayProvider.RenderLatestFrame(TangoEnums.TangoCameraId.TANGO_CAMERA_COLOR);
			GL.InvalidateState();
		}
	}*/

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

		//m_frustumHeight = 2.0f * m_frustumLength * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
		//m_frustumWidth = m_frustumHeight * camera.aspect;
		float W = Screen.width;
		float H = Screen.height;
		float InsetX = -(W / 2);
		float InsetY = -(H / 2);
		guiTexture.pixelInset = new Rect (InsetX, InsetY, W, H);

		devices = WebCamTexture.devices;
		m_numCameras = devices.Length;
		deviceName = devices[0].name;
		WebCamTexture wct = new WebCamTexture(deviceName);
		guiTexture.texture = wct;
		wct.Play();
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
