using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.XR.iOS;
using UnityEngine.Rendering;

// An unsuccessful attempt at creating a depth buffer from HitTests.
public class DepthBufferSceneController : MonoBehaviour {

    // Constants
	const TextureFormat DepthBufferFormat = TextureFormat.R8;

    // Script inputs
    public CursorManager m_cursorManager;
    public Camera m_camera;
    public int m_depthBufferWidth = 50;
    public int m_depthBufferHeight = 100;
	public float m_depthBufferUpdatesPerSecond = 1/5.0f;
	public Material m_depthMaterial;

    // Privates
    byte[] m_depthBuffer;
    Texture2D m_depthBufferTexture;
	float m_secondsPerUpdate;
	CommandBuffer m_bltCommandBuffer;
	bool m_isInitialized = false;

	void Start() {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;	
		m_secondsPerUpdate = 1.0f / m_depthBufferUpdatesPerSecond;
	}

	void OnDestroy()
	{
		m_camera.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, m_bltCommandBuffer);
	}

    void Update () {
        if (Utils.WasTouchStartDetected())
        {
            if (!m_isInitialized)
            {
         		m_depthBufferTexture = new Texture2D(m_depthBufferWidth, m_depthBufferHeight, DepthBufferFormat, false);
				m_depthBufferTexture.filterMode = FilterMode.Bilinear;
				m_depthBufferTexture.wrapMode = TextureWrapMode.Repeat;

				m_depthMaterial.SetTexture("_texture", m_depthBufferTexture);
				m_bltCommandBuffer = new CommandBuffer(); 
				m_bltCommandBuffer.Blit(null, BuiltinRenderTextureType.CurrentActive, m_depthMaterial);
				m_camera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, m_bltCommandBuffer);

                m_depthBuffer = new byte[m_depthBufferWidth * m_depthBufferHeight];

				StartCoroutine(UpdateDepthBufferCoroutine());
				m_isInitialized = true;
            }
        }
    }

    // Helpers
    private void UpdateDepthBuffer()
	{
		ARPoint point = new ARPoint {
			x = 0.0f,
			y = 0.0f
		};

		float xdelta = 1.0f / m_depthBufferWidth;
		float ydelta = 1.0f / m_depthBufferHeight;
		var arSession = UnityARSessionNativeInterface.GetARSessionNativeInterface();
		var offset = 0;
		for (int y = 0; y < m_depthBufferHeight; ++y) {
			for (int x = 0; x < m_depthBufferWidth; ++x) {
				List<ARHitTestResult> hitResults = arSession.HitTest(point, ARHitTestResultType.ARHitTestResultTypeFeaturePoint);
				if (hitResults.Count > 0 && hitResults[0].isValid) {
                    // For now use only values between 0 and 1.
                    var d = (hitResults[0].distance < 0) ? 0 : hitResults[0].distance;
                    d = (d > 1) ? 1 : d;
                    m_depthBuffer[offset] = (byte)(d * 255);
				} else {
					m_depthBuffer[offset] = 255;
				}
				offset++;
				point.x = point.x + xdelta;
			}
			point.y = point.y + ydelta;
			point.x = 0.0f;
		}
    }

	void ApplyDepthBuffer()
	{
		m_depthBufferTexture.LoadRawTextureData(m_depthBuffer);
		m_depthBufferTexture.Apply();
	}
    
    private IEnumerator UpdateDepthBufferCoroutine()
    {
        while (true)
        {
			UpdateDepthBuffer();
			ApplyDepthBuffer();
            
			yield return new WaitForSeconds(m_secondsPerUpdate);
        }
    }
}
