using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureCenterPixel : MonoBehaviour {
    // Script Inputs
    public bool m_shouldCaptureOnNextFrame = false;
    public Color m_lastCapturedColor = Color.green;

    // Privates
    Texture2D m_centerPixTex;

    void Start()
    {
        m_centerPixTex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
    }

    void OnPostRender()
    {
        if (m_shouldCaptureOnNextFrame)
        {
            Resolution res = Screen.currentResolution;
            int x = res.width / 2 - 1;
            int y = res.height / 2 - 1;

            m_lastCapturedColor = GetRenderedColorAt(x, y);
            m_shouldCaptureOnNextFrame = false;
        }
    }

    // Helpers
    Color GetRenderedColorAt(int x, int y)
    {
        m_centerPixTex.ReadPixels(new Rect(x, y, 1, 1), 0, 0);
        m_centerPixTex.Apply();
        
        return m_centerPixTex.GetPixels()[0];
    }
}
