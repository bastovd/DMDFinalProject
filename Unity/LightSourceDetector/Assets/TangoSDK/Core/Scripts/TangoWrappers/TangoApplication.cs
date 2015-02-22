/*
 * Copyright 2014 Google Inc. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Tango
{
	public delegate void PermissionsEvent(bool permissionsGranted);

    /// <summary>
    /// Entry point of Tango applications, maintain the application handler.
    /// </summary>
    public class TangoApplication : MonoBehaviour 
	{	

        public bool m_enableMotionTracking = true;
        public bool m_enableDepth = false;
        public bool m_motionTrackingAutoReset = true;
		public bool m_enableAreaLearning = false;
		public bool m_enableADFSaveLoad = false;

		private static string m_tangoServiceVersion = string.Empty;

		private const string CLASS_NAME = "TangoApplication";
        private const string ANDROID_PRO_LABEL_TEXT = "<size=30>Tango plugin requires Unity Android Pro!</size>";
        private const float ANDROID_PRO_LABEL_PERCENT_X = 0.5f;
        private const float ANDROID_PRO_LABEL_PERCENT_Y = 0.5f;
        private const float ANDROID_PRO_LABEL_WIDTH = 200.0f;
        private const float ANDROID_PRO_LABEL_HEIGHT = 200.0f;
        private const string DEFAULT_AREA_DESCRIPTION = "/sdcard/defaultArea";
		private const string MOTION_TRACKING_LOG_PREFIX = "Motion tracking mode : ";
		private const int MINIMUM_API_VERSION = 1978;

		private event PermissionsEvent m_permissionEvent;

		private static bool m_isValidTangoAPIVersion = false;
		private static bool m_hasVersionBeenChecked = false;
		private static bool m_anyPermissionDenied = false;
		private static bool m_permissionsRequested = false;
		private static bool m_permissionsGranted = false;
		private DepthProvider m_depthProvider;
		private IntPtr m_callbackContext = IntPtr.Zero;
		private bool m_isServiceConnected = false;
		private bool m_shouldReconnectService = false;
		private bool m_currentlyCheckingPermissions = false;
		private bool m_isDisconnecting = false;

		/// <summary>
		/// Gets the tango service version.
		/// </summary>
		/// <returns>The tango service version.</returns>
		public static string GetTangoServiceVersion()
		{
			if(m_tangoServiceVersion == string.Empty)
			{
				m_tangoServiceVersion = AndroidHelper.GetVersionName("com.projecttango.tango");
			}

			return m_tangoServiceVersion;
		}

		/// <summary>
		/// Determines if the application has Tango permissions.
		/// </summary>
		/// <returns><c>true</c> if has permissions; otherwise, <c>false</c>.</returns>
		public static bool HasGrantedPermissions()
		{
			return m_permissionsGranted;
		}

		/// <summary>
		/// Determines if has requested permissions.
		/// </summary>
		/// <returns><c>true</c> if has requested permissions; otherwise, <c>false</c>.</returns>
		public static bool HasRequestedPermissions()
		{
			return m_permissionsRequested;
		}

		/// <summary>
		/// Registers the permissions callback.
		/// </summary>
		/// <param name="permissionsEventHandler">Permissions event handler.</param>
		public void RegisterPermissionsCallback(PermissionsEvent permissionsEventHandler)
		{
			if(permissionsEventHandler != null)
			{
				m_permissionEvent += permissionsEventHandler;
			}
		}

		/// <summary>
		/// Removes the permissions callback.
		/// </summary>
		/// <param name="permissionsEventHandler">Permissions event handler.</param>
		public void RemovePermissionsCallback(PermissionsEvent permissionsEventHandler)
		{
			if(permissionsEventHandler != null)
			{
				m_permissionEvent -= permissionsEventHandler;
			}
		}

		
		/// <summary>
		/// Requests the necessary permissions for Tango functionality.
		/// </summary>
		/// <returns><c>true</c>, if necessary permissions were granted from the permissions prompts, <c>false</c> otherwise.</returns>
		public void RequestNecessaryPermissions()
		{
			if(!m_currentlyCheckingPermissions)
			{
				StartCoroutine(_PromptUserForPermissions());
			}
		}
		
		/// <summary>
		/// Initialize Tango Service and Config.
		/// </summary>
		public void InitApplication()
		{
			Debug.Log("-----------------------------------Initializing Tango");
			_TangoInitialize();
			TangoConfig.InitConfig(TangoEnums.TangoConfigType.TANGO_CONFIG_DEFAULT);
		}
		
		/// <summary>
		/// Initialize the providers.
		/// </summary>
		/// <param name="UUID"> UUID to be loaded, if any.</param>
		public void InitProviders(string UUID)
		{
			_InitializeMotionTracking(UUID);
			_InitializeDepth();
			_InitializeOverlay();
			_SetEventCallbacks();
			_InitializeOverlay();
		}
		
		/// <summary>
		/// Connects to Tango Service.
		/// </summary>
		public void ConnectToService()
		{
			Debug.Log("TangoApplication.ConnectToService()");
			_TangoConnect();
		}

		/// <summary>
		/// Shutdown this instance.
		/// </summary>
		public void Shutdown()
		{
			Debug.Log("Tango Shutdown");
			TangoConfig.Free();
			_TangoDisconnect();
		}

		/// <summary>
		/// Prompt user for Tango permissions.
		/// </summary>
		/// <returns>The prompt user for permissions.</returns>
		private IEnumerator _PromptUserForPermissions()
		{
			m_currentlyCheckingPermissions = true;
			m_anyPermissionDenied = false;

			if(m_isDisconnecting)
			{
				yield return null;
			}

			if(m_enableMotionTracking)
			{
				yield return StartCoroutine(_RequestPermissions(Common.TANGO_MOTION_TRACKING_PERMISSIONS));
			}
			
			if(m_enableAreaLearning || m_enableADFSaveLoad)
			{
				yield return StartCoroutine(_RequestPermissions(Common.TANGO_ADF_LOAD_SAVE_PERMISSIONS));
			}
			
			m_permissionsGranted = !m_anyPermissionDenied;
			Debug.Log("-------------------------------------Permissions were accepted = " + m_permissionsGranted); 
			m_permissionsRequested = true;

			if(m_permissionEvent != null)
			{
				m_permissionEvent(m_permissionsGranted);
			}

			yield return new WaitForSeconds(0.1f);

			m_currentlyCheckingPermissions = false;
		}

		/// <summary>
		/// Helper method that will resume the tango services on App Resume.
		/// Locks the config again and connects the service.
		/// </summary>
		private void _ResumeTangoServices()
		{
			StartCoroutine(_DelayedConnect());
		}
		
		private IEnumerator _DelayedConnect()
		{
			if(!m_currentlyCheckingPermissions)
			{
				yield return StartCoroutine(_PromptUserForPermissions());
			}
			_TangoConnect();
		}
		
		/// <summary>
		/// Helper method that will suspend the tango services on App Suspend.
		/// Unlocks the tango config and disconnects the service.
		/// </summary>
		private void _SuspendTangoServices()
		{
			Debug.Log("Suspending Tango Service");
			_TangoDisconnect();
		}
		
		/// <summary>
		/// Callback for when Unity app goes to background/foreground states.
		/// </summary>
		/// <param name="didPause">If <c>true</c> application is in the background.</param>
		private void OnApplicationPause(bool isPaused)
		{
			if(!isPaused)
			{
				Debug.Log("OnResume");
				if(m_shouldReconnectService)
				{
					_ResumeTangoServices();
				}
			}
			else
			{
				Debug.Log("OnPause");
				if(m_isServiceConnected && !m_currentlyCheckingPermissions)
				{
					m_shouldReconnectService = true;
					_SuspendTangoServices();
				}
			}
		}

		/// <summary>
		/// Request permissions for Unity functionality.
		/// </summary>
		/// <returns><c>true</c>, if request permissions was _ed, <c>false</c> otherwise.</returns>
		/// <param name="permissionType">Permission type.</param>
		private IEnumerator _RequestPermissions(string permissionType)
		{
			//#if !UNITY_EDITOR && UNITY_ANDROID
			// verify
			string syncFile = "data/data/" + AndroidHelper.GetCurrentPackageName() + "/files/activity_result";
			string result = string.Empty;
			yield return StartCoroutine(_Delete(syncFile));

			Debug.Log("Launching permission requester for : " + permissionType);

			// request
			AndroidHelper.StartTangoPermissionsActivity(permissionType);
			
			while(!File.Exists(syncFile))
			{
				// sit and spin while waiting for the permissions intent to 
				// steal focus from Unity. Once the intent returns this text
				// file will exist.
				yield return null;
			}
			
			try
			{
				StreamReader streamReader = new StreamReader(syncFile);
				result = streamReader.ReadLine();
				streamReader.Close();
			}
			catch (System.IO.FileNotFoundException e)
			{
				Debug.Log(CLASS_NAME + "._RequestPermissions : File Not Found: " + e.Message);
			}
			
			if(string.IsNullOrEmpty(result) == false)
			{
				if(result != "RESULT_OK")
				{
					m_anyPermissionDenied = true;
				}
			}

			yield return StartCoroutine(_Delete(syncFile));
			//#endif
			
			yield return null;
		}
        
		/// <summary>
		/// Set callbacks on all PoseListener objects.
		/// </summary>
		/// <param name="framePairs">Frame pairs.</param>
        private void _SetMotionTrackingCallbacks(TangoCoordinateFramePair[] framePairs)
        {
            PoseListener[] poseListeners = FindObjectsOfType<PoseListener>();

            foreach (PoseListener poseListener in poseListeners)
            {
                if (poseListener != null)
                {
                    poseListener.AutoReset = m_motionTrackingAutoReset;

                    poseListener.SetCallback(framePairs);
                }
            }
        }

		/// <summary>
		/// Set callbacks for all DepthListener objects.
		/// </summary>
        private void _SetDepthCallbacks()
        {
            DepthListener[] depthListeners = FindObjectsOfType<DepthListener>();

            foreach (DepthListener depthListener in depthListeners)
            {
                if (depthListener != null)
                {
                    depthListener.SetCallback();
                }
            }
        }

        /// <summary>
        /// Set callbacks for all TangoEventListener objects.
        /// </summary>
        private void _SetEventCallbacks()
        {
            TangoEventListener[] eventListeners = FindObjectsOfType<TangoEventListener>();

            foreach (TangoEventListener eventListener in eventListeners)
            {
                if (eventListener != null)
                {
                    eventListener.SetCallback();
                }
            }
        }

		/// <summary>
		/// Set callbacks for all VideoOverlayListener objects.
		/// </summary>
		private void _SetVideoOverlayCallbacks()
		{
			VideoOverlayListener[] videoOverlayListeners = FindObjectsOfType<VideoOverlayListener>();
			
			foreach (VideoOverlayListener videoOverlayListener in videoOverlayListeners)
			{
				if (videoOverlayListener != null)
				{
					videoOverlayListener.SetCallback(TangoEnums.TangoCameraId.TANGO_CAMERA_COLOR);
				}
			}
		}
		
		/// <summary>
		/// Initialize motion tracking.
		/// </summary>
		private void _InitializeMotionTracking(string UUID)
        {
			System.Collections.Generic.List<TangoCoordinateFramePair> framePairs = new System.Collections.Generic.List<TangoCoordinateFramePair>();
			if (TangoConfig.SetBool(TangoConfig.Keys.ENABLE_AREA_LEARNING_BOOL, m_enableAreaLearning) && m_enableAreaLearning)
			{
				Debug.Log("Area Learning is enabled.");
                if(!string.IsNullOrEmpty(UUID))
                {
                        TangoConfig.SetString(TangoConfig.Keys.LOAD_AREA_DESCRIPTION_UUID_STRING, UUID);
                }
                
                TangoCoordinateFramePair areaDescription;
                areaDescription.baseFrame = TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_AREA_DESCRIPTION;
                areaDescription.targetFrame = TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_DEVICE;
                
                TangoCoordinateFramePair startToADF;
                startToADF.baseFrame = TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_AREA_DESCRIPTION;
                startToADF.targetFrame = TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_START_OF_SERVICE;
                
                framePairs.Add(areaDescription);
                framePairs.Add(startToADF);
			}

            
            if (TangoConfig.SetBool(TangoConfig.Keys.ENABLE_MOTION_TRACKING_BOOL, m_enableMotionTracking) && m_enableMotionTracking)
            {
				TangoCoordinateFramePair motionTracking;
				motionTracking.baseFrame = TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_START_OF_SERVICE;
				motionTracking.targetFrame = TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_DEVICE;
				framePairs.Add(motionTracking);
            }

			if(framePairs.Count > 0)
			{
				_SetMotionTrackingCallbacks(framePairs.ToArray());
			}
            
			TangoConfig.SetBool(TangoConfig.Keys.ENABLE_MOTION_TRACKING_AUTO_RECOVERY_BOOL, m_motionTrackingAutoReset);
        }

        /// <summary>
        /// Initialize depth perception.
        /// </summary>
        private void _InitializeDepth()
        {
            if (TangoConfig.SetBool(TangoConfig.Keys.ENABLE_DEPTH_PERCEPTION_BOOL, m_enableDepth) && m_enableDepth)
            {
                _SetDepthCallbacks();
            }
			bool depthConfigValue = false;
			TangoConfig.GetBool(TangoConfig.Keys.ENABLE_DEPTH_PERCEPTION_BOOL, ref depthConfigValue);
			Debug.Log("TangoConfig bool for key: " + TangoConfig.Keys.ENABLE_DEPTH_PERCEPTION_BOOL
			          + " has value set of: " + depthConfigValue);
        }

        /// <summary>
        /// Initialize the RGB overlay.
        /// </summary>
        private void _InitializeOverlay()
        {
			_SetVideoOverlayCallbacks();
        }
        
		/// <summary>
		/// Initialize the Tango Service.
		/// </summary>
        private void _TangoInitialize()
        {
			if(_IsValidTangoAPIVersion())
			{
				int status = TangoServiceAPI.TangoService_initialize( IntPtr.Zero, IntPtr.Zero);
	            if (status != Common.ErrorType.TANGO_SUCCESS)
	            {
					Debug.Log("-------------------Tango initialize status : " + status);
					Debug.Log(CLASS_NAME + ".Initialize() The service has not been initialized!");
	            }
	            else
	            {
					Debug.Log(CLASS_NAME + ".Initialize() Tango was initialized!");
	            }
			}
			else
			{
				Debug.Log(CLASS_NAME + ".Initialize() Invalid API version. please update to minimul API version.");
			}
        }
        
		/// <summary>
		/// Connect to the Tango Service.
		/// </summary>
        private void _TangoConnect()
        {
			if(!m_isServiceConnected)
			{
				m_isServiceConnected = true;
				if (TangoServiceAPI.TangoService_connect(m_callbackContext, TangoConfig.GetConfig()) != Common.ErrorType.TANGO_SUCCESS)
	            {
					Debug.Log(CLASS_NAME + ".Connect() Could not connect to the Tango Service!");
	            }
	            else
	            {
					Debug.Log(CLASS_NAME + ".Connect() Tango client connected to service!");
	            }
			}
        }
        
		/// <summary>
		/// Disconnect from the Tango Service.
		/// </summary>
        private void _TangoDisconnect()
		{
			Debug.Log(CLASS_NAME + ".Disconnect() Disconnecting from the Tango Service");
			m_isDisconnecting = true;
			m_isServiceConnected = false;
            if (TangoServiceAPI.TangoService_disconnect() != Common.ErrorType.TANGO_SUCCESS)
            {
				Debug.Log(CLASS_NAME + ".Disconnect() Could not disconnect from the Tango Service!");
				m_isDisconnecting = false;
            }
            else
            {
				Debug.Log(CLASS_NAME + ".Disconnect() Tango client disconnected from service!");
				m_isDisconnecting = false;
            }
        }

		/// <summary>
		/// Checks to see if the current Tango Service is supported.
		/// </summary>
		/// <returns><c>true</c>, if is valid tango API version is greater
		/// than or equal to the minimum supported version, <c>false</c> otherwise.</returns>
		private bool _IsValidTangoAPIVersion()
		{
			if(!m_hasVersionBeenChecked)
			{
				int versionCode = _GetTangoAPIVersion();
				if(versionCode < 0)
				{
					m_isValidTangoAPIVersion = false;
				}
				else
				{
					m_isValidTangoAPIVersion = (versionCode >= MINIMUM_API_VERSION);
				}
				
				m_hasVersionBeenChecked = true;
			}
			
			return m_isValidTangoAPIVersion;
		}

		/// <summary>
		/// Delete that blocks code execution.
		/// </summary>
		/// <param name="path">Path to file to delete.</param>
		private IEnumerator _Delete(string path)
		{
			File.Delete(path);

			while(File.Exists(path))
			{
				yield return null;
			}
		}

		/// <summary>
		/// Gets the get tango API version code.
		/// </summary>
		/// <returns>The get tango API version code.</returns>
		private static int _GetTangoAPIVersion()
		{
			return AndroidHelper.GetVersionCode("com.projecttango.tango");
		}

        #region NATIVE_FUNCTIONS
        /// <summary>
        /// Interface for native function calls to Tango Service.
        /// </summary>
        private struct TangoServiceAPI
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            [DllImport(Common.TANGO_UNITY_DLL)]
			public static extern int TangoService_initialize (IntPtr JNIEnv, IntPtr appContext);
            
            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int TangoService_connect (IntPtr callbackContext, IntPtr config);
            
            [DllImport(Common.TANGO_UNITY_DLL)]
            public static extern int TangoService_disconnect ();
            #else
            public static int TangoService_initialize(IntPtr JNIEnv, IntPtr appContext)
            {
                return Common.ErrorType.TANGO_SUCCESS;
            }
			public static  int TangoService_connect (IntPtr callbackContext, IntPtr config)
            {
                return Common.ErrorType.TANGO_SUCCESS;
            }
            public static int TangoService_disconnect ()
            {
                return Common.ErrorType.TANGO_SUCCESS;
            }
            #endif
        }
        #endregion // NATIVE_FUNCTIONS
    }
}
