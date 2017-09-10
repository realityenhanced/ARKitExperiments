using UnityEngine;
using System.Runtime.InteropServices;
using System;

// TODO:
// - Figure out a way to pass the Y texture directly to the native plugin to prevent marshalling.
public class OpencvProcessingInterface  {
    int[] m_outArray = new int[4];
    IntPtr m_marshalledBuffer;
    int m_marshalledSize;
    byte[] m_lastBufferMarshalled;
    int m_lastBufferMarshalledWidth;

    [DllImport ("__Internal")]
    private static extern int SaveDescriptors(IntPtr buffer, int width, int height);

    [DllImport ("__Internal")]
    private static extern int MatchDescriptors(IntPtr buffer, int width, int height, [In, Out] int[] boundingRect);

    public int SaveDescriptorsForFrame(Texture2D frame) {
        MarshalTexture(frame);
        return SaveDescriptors(m_marshalledBuffer, frame.width, frame.height);
    }

    public int MatchDescriptorsForFrame(Texture2D frame, ref Rect boundingRect) {
        MarshalTexture(frame);
        int val = MatchDescriptors(m_marshalledBuffer, frame.width, frame.height, m_outArray);
        if (val == 1) {
            boundingRect = new UnityEngine.Rect(m_outArray[0], m_outArray[1], m_outArray[2], m_outArray[3]);
        }

        return val;
    }

    public Color GetColorFromFrameAt(int x, int y)
    {
        int pixelStart = 3 * (y * m_lastBufferMarshalledWidth + x);

        byte r = m_lastBufferMarshalled[pixelStart];
        byte g = m_lastBufferMarshalled[pixelStart + 1];
        byte b = m_lastBufferMarshalled[pixelStart + 2];

        return new Color(r, g, b, 1.0f);
    }

    ~OpencvProcessingInterface()
    {
        if (m_marshalledBuffer != null)
        {
            Marshal.FreeHGlobal(m_marshalledBuffer);
        }
    }

    private void MarshalTexture(Texture2D frame) {
        if (frame.format != TextureFormat.RGB24)
        {
            Debug.LogError("Invalid texture format.");
        }
        else
        {
            if (m_lastBufferMarshalled == null || m_lastBufferMarshalled.Length != frame.width * frame.height * 3)
            {
                Debug.Log("Creating new buffer");
                m_lastBufferMarshalled = frame.GetRawTextureData();

                m_lastBufferMarshalledWidth = frame.width;
            }

            int size = Marshal.SizeOf(m_lastBufferMarshalled[0]) * m_lastBufferMarshalled.Length;
            if (m_marshalledBuffer == null || m_marshalledSize != size)
            {
                if (m_marshalledBuffer != null)
                {
                    Debug.Log("Free old marshalled buffer");
                    Marshal.FreeHGlobal(m_marshalledBuffer);
                }
                m_marshalledBuffer = Marshal.AllocHGlobal(size);
                m_marshalledSize = size;

                Debug.Log("Created new marshalled buffer");
            }
            Marshal.Copy(m_lastBufferMarshalled, 0, m_marshalledBuffer, m_lastBufferMarshalled.Length);
        }
    }
}
