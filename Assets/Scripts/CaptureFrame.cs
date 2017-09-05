using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureFrame : MonoBehaviour
{
    // Script Inputs
    public bool m_shouldCaptureOnNextFrame = false;
    public Texture2D m_lastCapturedFrame;

    void OnPostRender()
    {
        if (m_shouldCaptureOnNextFrame)
        {
            Camera camera = GetComponent<Camera>();
            int rtWidth = camera.activeTexture.width;
            int rtHeight = camera.activeTexture.height;
            if (m_lastCapturedFrame == null || m_lastCapturedFrame.width != rtWidth || m_lastCapturedFrame.height != rtHeight)
            {
                m_lastCapturedFrame = new Texture2D(rtWidth, rtHeight, TextureFormat.ARGB32, false);
            }

            m_lastCapturedFrame.ReadPixels(new Rect(0, 0, rtWidth, rtHeight), 0, 0);
            m_lastCapturedFrame.Apply();

            m_shouldCaptureOnNextFrame = false;
        }
    }
}