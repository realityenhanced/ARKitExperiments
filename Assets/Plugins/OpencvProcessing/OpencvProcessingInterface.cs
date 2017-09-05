using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class OpencvProcessingInterface  {

	[DllImport ("__Internal")]
	private static extern int PerformOperation(IntPtr buffer, int width, int height);

	public int PerformProcessing(Texture2D frame) {
        Debug.Log("Frame width = " + frame.width + " height = " + frame.height + " format = " + frame.format);

		// TEST
		byte[] tmpBuf = frame.GetRawTextureData();

		int size = Marshal.SizeOf(tmpBuf[0]) * tmpBuf.Length;
		IntPtr pnt = Marshal.AllocHGlobal(size);
			
		Marshal.Copy(tmpBuf, 0, pnt, tmpBuf.Length);               

		int val = PerformOperation(pnt, frame.width, frame.height);
		Marshal.FreeHGlobal(pnt);
		// TEST

		return val;
	}
}
