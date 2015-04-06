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
using System.Collections;
using UnityEngine;
using Tango;

/// <summary>
/// Encapsulates functionality to draw the video overlay. EXPERIMENTAL.
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(Camera))]
public class ARScreen : MonoBehaviour
{
    public float m_frustumLength = 3.0f; // HACK : Chase
	public Material m_screenMaterial;
    
	private Texture2D m_texture;
    private float m_frustumHeight = 0.0f;
    private float m_frustumWidth = 0.0f;
    private Vector3[] m_frustumPoints;
    private Mesh m_mesh;
    private MeshFilter m_meshFilter;
	private TangoApplication m_tangoApplication;
	private bool m_readyToDraw = false;

    /// <summary>
    /// Gets the frustum points of AR Screen.
    /// </summary>
    /// <returns>Retursn the frustum points of AR Screen.</returns>
    public Vector3[] GetFrustumPoints()
    {
        return m_frustumPoints;
    }

    /// <summary>
    /// Initialize the AR Screen.
    /// </summary>
    private void Awake()
    {
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
//			VideoOverlayProvider.ExperimentalConnectTexture(TangoEnums.TangoCameraId.TANGO_CAMERA_COLOR, m_texture.GetNativeTextureID(), _OnUnityFrameAvailable);
			Debug.Log("Texture set!");
		}
		
		m_tangoApplication = FindObjectOfType<Tango.TangoApplication>();
		
		if(m_tangoApplication != null)
		{
			m_tangoApplication.RegisterPermissionsCallback(_OnTangoApplicationPermissionsEvent);
		}
    }
    
    /// <summary>
    /// Creates a plane and sizes it for the AR Screen.
    /// </summary>
    public void ResizeScreen()
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
	}

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
	void OnPreRender()
	{
		if(m_readyToDraw)
		{
			VideoOverlayProvider.RenderLatestFrame(TangoEnums.TangoCameraId.TANGO_CAMERA_COLOR);
			GL.InvalidateState();
		}
	}
}