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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Tango;

namespace Tango
{
    /// <summary>
    /// Video Overlay Provider class provide video functions
    /// to get frame textures.
    /// </summary>
    public class VideoOverlayProvider
    {
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void TangoService_onImageAvailable(IntPtr callbackContext, Tango.TangoEnums.TangoCameraId cameraId, [In,Out] TangoImageBuffer image);
		
		private static readonly string CLASS_NAME = "VideoOverlayProvider";
		private static IntPtr callbackContext;

        /// <summary>
        /// Connects the texture.
        /// </summary>
        /// <param name="cameraId">Camera identifier.</param>
        /// <param name="textureId">Texture identifier.</param>
        public static void ConnectTexture(TangoEnums.TangoCameraId cameraId, int textureId)
        {
            int returnValue = VideoOverlayAPI.TangoService_connectTextureId(cameraId, textureId);

            if (returnValue != Common.ErrorType.TANGO_SUCCESS)
            {
				Debug.Log("VideoOverlayProvider.ConnectTexture() Texture was not connected to camera!");
            }
        }

        /// <summary>
        /// Renders the latest frame.
        /// </summary>
        /// <param name="cameraId">Camera identifier.</param>
		public static void RenderLatestFrame(TangoEnums.TangoCameraId cameraId)
        {
            int returnValue = VideoOverlayAPI.TangoService_updateTexture(cameraId);
            
            if (returnValue != Common.ErrorType.TANGO_SUCCESS)
            {
				Debug.Log("VideoOverlayProvider.UpdateTexture() Texture was not updated by camera!");
            }
        }

		/// <summary>
		/// Sets the callback for notifications when image data is ready.
		/// </summary>
		/// <param name="cameraId">Camera identifier.</param>
		/// <param name="onImageAvailable">On image available callback handler.</param>
		public static void SetCallback(TangoEnums.TangoCameraId cameraId, TangoService_onImageAvailable onImageAvailable)
		{
			int returnValue = VideoOverlayAPI.TangoService_connectOnFrameAvailable(cameraId, callbackContext, onImageAvailable);
			if(returnValue == Tango.Common.ErrorType.TANGO_SUCCESS)
			{
				Debug.Log(CLASS_NAME + ".SetCallback() Callback was set.");
			}
			else
			{
				Debug.Log(CLASS_NAME + ".SetCallback() Callback was not set!");
			}

		}

        #region NATIVE_FUNCTIONS
        /// <summary>
        /// Video overlay native function import.
        /// </summary>
        private struct VideoOverlayAPI
        {
            #if UNITY_ANDROID && !UNITY_EDITOR

            [DllImport(Common.TANGO_UNITY_DLL)]
			public static extern int TangoService_connectTextureId(TangoEnums.TangoCameraId cameraId, int textureHandle);

            [DllImport(Common.TANGO_UNITY_DLL)]
			public static extern int TangoService_updateTexture(TangoEnums.TangoCameraId cameraId);
			
			[DllImport(Common.TANGO_UNITY_DLL)]
			public static extern int TangoService_connectOnFrameAvailable(TangoEnums.TangoCameraId cameraId,
			                                                              IntPtr context,
			                                                              [In,Out] TangoService_onImageAvailable onImageAvailable);
            #else
            public static int TangoService_connectTextureId(TangoEnums.TangoCameraId cameraId, int textureHandle)
            {
                return Tango.Common.ErrorType.TANGO_SUCCESS;
            }
            
            public static int TangoService_updateTexture(TangoEnums.TangoCameraId cameraId)
            {
                return Tango.Common.ErrorType.TANGO_SUCCESS;
            }

			public static int TangoService_connectOnFrameAvailable(TangoEnums.TangoCameraId cameraId,
			                                                       IntPtr context,
			                                                       [In,Out] TangoService_onImageAvailable onImageAvailable)
			{
				return Tango.Common.ErrorType.TANGO_SUCCESS;
			}
            #endif
            #endregion
        }
    }
}
