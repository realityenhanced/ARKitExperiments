using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public class OpencvProcessingInterface  {

    [DllImport ("__Internal")]
    private static extern int PerformOperation(IntPtr buffer, int width, int height);

    public int PerformProcessing() {
        // TEST
	byte[] tmpBuf = new byte[10*10*3];

	int size = Marshal.SizeOf(tmpBuf[0]) * tmpBuf.Length;
        IntPtr pnt = Marshal.AllocHGlobal(size);
            
        Marshal.Copy(tmpBuf, 0, pnt, tmpBuf.Length);               

        int val = PerformOperation(pnt, 10, 10);
        Marshal.FreeHGlobal(pnt);
        // TEST

	return val;
    }
}
