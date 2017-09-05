using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpencvExperimentController : MonoBehaviour {
    // Privates
    OpencvProcessingInterface m_opencvProcessing;
    bool m_isScreenBeingCaptured = false;
    CaptureFrame m_frameCapturer;

	// Use this for initialization
	void Start () {
		m_opencvProcessing = new OpencvProcessingInterface();
        m_frameCapturer = Camera.main.GetComponent<CaptureFrame>();
	}
	
	// Update is called once per frame
	void Update () {
        if (m_isScreenBeingCaptured)
        {
            Debug.Log("Feed the captured frame top opencv");

            m_isScreenBeingCaptured = false;
            Debug.Log(m_opencvProcessing.PerformProcessing(m_frameCapturer.m_lastCapturedFrame));
        }
        else if (Utils.WasTouchStartDetected())
        {
            if (Utils.IsTouchOnUI())
            {
                // Ignore touch events if a button is pressed
                return;
            }

            m_frameCapturer.m_shouldCaptureOnNextFrame = true;
            m_isScreenBeingCaptured = true;

            Debug.Log("Capture screen and feed the opencv function on the next frame.");
        }
    }
}
