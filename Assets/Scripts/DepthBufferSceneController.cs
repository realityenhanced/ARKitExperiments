using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.XR.iOS;

public class DepthBufferSceneController : MonoBehaviour {

    // Constants
	const TextureFormat DepthBufferFormat = TextureFormat.R8;

    // Script inputs
    public CursorManager m_cursorManager;
    public GameObject m_actorPrefab;
	public Camera m_cameraToLookAt;
    public int m_depthBufferWidth = 75;
    public int m_depthBufferHeight = 75;
	public float m_depthBufferUpdatesPerSecond = 1/5.0f;

    // Privates
    Transform m_actorTransform;
    byte[] m_depthBuffer;
    Texture2D m_depthBufferTexture;
	float m_secondsPerUpdate;

	void Start() {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;	
		m_secondsPerUpdate = 1.0f / m_depthBufferUpdatesPerSecond;
	}

    void Update () {
        if (Utils.WasTouchDetected())
        {
            // Place the actor & it's shadow plane at the last detected cursor postion.
            var pos = m_cursorManager.GetCurrentCursorPosition();
            var rot = m_cursorManager.GetCurrentCursorRotation();

            if (m_actorTransform == null)
            {
                m_actorTransform = Utils.SpawnGameObjectAt(m_actorPrefab, pos, rot).transform;
				m_depthBufferTexture = new Texture2D(m_depthBufferWidth, m_depthBufferHeight, DepthBufferFormat, false);

				var renderer = m_actorTransform.GetComponentsInChildren<MeshRenderer>()[0];
				renderer.material.mainTexture = m_depthBufferTexture;

                m_depthBuffer = new byte[m_depthBufferWidth * m_depthBufferHeight];

				StartCoroutine(UpdateDepthBufferCoroutine());
            }
            else
            {
                m_actorTransform.position = pos;
                m_actorTransform.rotation = rot;
            }

			// Face the camera
			m_actorTransform.LookAt(m_cameraToLookAt.transform);
        }
    }

    // Helpers
    private void UpdateDepthBufferTexture()
	{
		ARPoint point = new ARPoint {
			x = 0.0f,
			y = 0.0f
		};

		// TMP:Start
		float xdelta = 1.0f / m_depthBufferWidth;
		float ydelta = 1.0f / m_depthBufferHeight;
		var arSession = UnityARSessionNativeInterface.GetARSessionNativeInterface();
		var offset = 0;
		for (int y = 0; y < m_depthBufferHeight; ++y) {
			for (int x = 0; x < m_depthBufferWidth; ++x) {
				List<ARHitTestResult> hitResults = arSession.HitTest(point, ARHitTestResultType.ARHitTestResultTypeFeaturePoint);
				if (hitResults.Count > 0) {
						m_depthBuffer[offset++] = (byte)(hitResults[0].distance * 255);
				}
				point.x = point.x + xdelta;
			}
			point.y = point.y + ydelta;
			point.x = 0.0f;
		}
		// TMP: Ends

		m_depthBufferTexture.LoadRawTextureData(m_depthBuffer);
		m_depthBufferTexture.Apply ();
    }
    
    private IEnumerator UpdateDepthBufferCoroutine()
    {
        while (true)
        {
			UpdateDepthBufferTexture();
            yield return new WaitForSeconds(m_secondsPerUpdate);
        }
    }
}
