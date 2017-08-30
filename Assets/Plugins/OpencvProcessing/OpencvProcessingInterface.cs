using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class OpencvProcessingInterface  {

    [DllImport ("__Internal")]
    private static extern int PerformOperation();

    public int PerformProcessing() {
        return PerformOperation();
    }
}
