using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class OpencvProcessingInterface  {
        int[] m_outArray = new int[4];

	[DllImport ("__Internal")]
	private static extern int SaveDescriptors(IntPtr buffer, int width, int height);

	[DllImport ("__Internal")]
	private static extern int MatchDescriptors(IntPtr buffer, int width, int height, [In, Out] int[] boundingRect);

	public int SaveDescriptorsForFrame(Texture2D frame) {
            // TODO: refactor and use RAII
	    byte[] tmpBuf = frame.GetRawTextureData();

	    int size = Marshal.SizeOf(tmpBuf[0]) * tmpBuf.Length;
	    IntPtr pnt = Marshal.AllocHGlobal(size);
			
	    Marshal.Copy(tmpBuf, 0, pnt, tmpBuf.Length);               

	    int val = SaveDescriptors(pnt, frame.width, frame.height);
	    Marshal.FreeHGlobal(pnt);

	    return val;
	}

	public int MatchDescriptorsForFrame(Texture2D frame, ref Rect boundingRect) {
            // TODO: refactor and use RAII
	    byte[] tmpBuf = frame.GetRawTextureData();

	    int size = Marshal.SizeOf(tmpBuf[0]) * tmpBuf.Length;
	    IntPtr pnt = Marshal.AllocHGlobal(size);
			
	    Marshal.Copy(tmpBuf, 0, pnt, tmpBuf.Length);               

	    int val = MatchDescriptors(pnt, frame.width, frame.height, m_outArray);
	    Marshal.FreeHGlobal(pnt);

            if (val == 1) {
                 boundingRect = new UnityEngine.Rect(m_outArray[0], m_outArray[1], m_outArray[2], m_outArray[3]);
            }

	    return val;
	}
}
