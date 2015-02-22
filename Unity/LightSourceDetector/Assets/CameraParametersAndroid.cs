using UnityEngine;

#if UNITY_ANDROID
public class CameraParametersAndroid
{
	public static float HorizontalViewAngle { get; protected set; }
	public static float VerticalViewAngle { get; protected set; }
	public static int numCameras { get; protected set; }
	static AndroidJavaClass cameraClass = new AndroidJavaClass("android.hardware.Camera"); 
	
	static CameraParametersAndroid()
	{
		//cameraClass = new AndroidJavaClass("android.hardware.Camera");
		
		numCameras = cameraClass.CallStatic<int>("getNumberOfCameras");
		
		// This is an ugly hack to make Unity
		// generate Camera permisions
		WebCamDevice[] devices = WebCamTexture.devices;
		
		// Camera.open gets back-facing camera by default
		// you should check for exceptions
		int camID = 0;
		AndroidJavaObject camera = cameraClass.CallStatic<AndroidJavaObject>("open", camID);
		
		// I'm pretty sure camera will never be null at this point
		// It will either be a valid object or Camera.open would throw an exception
		if (camera != null)
		{
			AndroidJavaObject cameraParameters = camera.Call<AndroidJavaObject>("getParameters");
			HorizontalViewAngle = cameraParameters.Call<float>("getHorizontalViewAngle");
			VerticalViewAngle = cameraParameters.Call<float>("getVerticalViewAngle");
			
		}
		else
		{
			Debug.LogError("[CameraParametersAndroid] Camera not available");
		}
	}
}

#elif UNITY_EDITOR
public class CameraParametersAndroid
{
	public static float HorizontalViewAngle { get; protected set; }
	public static float VerticalViewAngle { get; protected set; }
	public static int numCameras { get; protected set; }
	
	static CameraParametersAndroid()
	{
		HorizontalViewAngle = 0;
		VerticalViewAngle = 0;
		numCameras = 0;
	}
}
#endif
