using System.Collections;
using System;
using UnityEngine;

public class DepthBufferSceneController : SceneController {

    // Constants
    const int BytesPerDepthPixel = 2;

    // Script inputs
    public CursorManager m_cursorManager;
    public GameObject m_actorPrefab;
	public Camera m_cameraToLookAt;
    public int m_depthBufferWidth = 640;
    public int m_depthBufferHeight = 360;

    // Privates
    Transform m_actorTransform;
    byte[] m_depthBuffer;
    Texture2D m_depthBufferTexture;

    void Update () {
        if (Utils.WasTouchDetected())
        {
            // Place the actor & it's shadow plane at the last detected cursor postion.
            var pos = m_cursorManager.GetCurrentCursorPosition();
            var rot = m_cursorManager.GetCurrentCursorRotation();

            if (m_actorTransform == null)
            {
                m_actorTransform = Utils.SpawnGameObjectAt(m_actorPrefab, pos, rot).transform;
                m_depthBufferTexture = new Texture2D(m_depthBufferWidth, m_depthBufferHeight, TextureFormat.R16, false);

                var renderer = m_actorTransform.GetComponent<MeshRenderer>();
                renderer.material.mainTexture = m_depthBufferTexture;

                m_depthBuffer = new byte[m_depthBufferWidth * m_depthBufferHeight * BytesPerDepthPixel];

                // TMP: Start
                StartCoroutine(TestSwitchingColors());
                // TMP: Ends
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
    private void UpdateDepthBuffer()
    {
        if (m_depthBufferTexture)
        {
            m_depthBufferTexture.LoadRawTextureData(m_depthBuffer);
        }
    }

    private IEnumerator TestSwitchingColors()
    {
        bool isWhite = false;
        while (true)
        {
            if (isWhite)
            {
                SetDepthBufferColor(0.0f);
            }
            else
            {
                SetDepthBufferColor(1.0f);
            }
            isWhite = !isWhite;

            yield return new WaitForSeconds(1.0f);
        }
    }

    private void SetDepthBufferColor(float val)
    {
        System.UInt16 uint16Val = (System.UInt16)(val * System.UInt16.MaxValue);
        byte[] vals = BitConverter.GetBytes(uint16Val);
        for (var i = 0; i < m_depthBuffer.GetLength(0) / BytesPerDepthPixel; ++i) {
            m_depthBuffer.SetValue(vals[0], i*BytesPerDepthPixel);
            m_depthBuffer.SetValue(vals[1], (i*BytesPerDepthPixel)+1);
        }
    }
}
