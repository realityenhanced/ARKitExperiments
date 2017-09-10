using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpencvExperimentController : MonoBehaviour {
    // Privates
    OpencvProcessingInterface m_opencvProcessing;
    bool m_isScreenBeingCaptured = false;
    CaptureFrame m_frameCapturer;
    bool m_shouldSaveDescriptor = true;
    
	// Use this for initialization
	void Start () {
	    m_opencvProcessing = new OpencvProcessingInterface();
            m_frameCapturer = Camera.main.GetComponent<CaptureFrame>();
	}
	
	// Update is called once per frame
	void Update () {
        if (m_shouldSaveDescriptor && m_isScreenBeingCaptured)
        {
            Debug.Log("Feed the captured frame top opencv");

            m_isScreenBeingCaptured = false;
            
            Debug.Log(m_opencvProcessing.SaveDescriptorsForFrame(m_frameCapturer.m_lastCapturedFrame));

            // Save descriptors only on the first tap.
            m_shouldSaveDescriptor = false;
        }
        else if (m_shouldSaveDescriptor && Utils.WasTouchStartDetected())
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
	else if (! m_shouldSaveDescriptor)
        {
            if (m_isScreenBeingCaptured)
            {
                Rect boundingRect = new Rect(0,0,0,0);
                Debug.Log(m_opencvProcessing.MatchDescriptorsForFrame(m_frameCapturer.m_lastCapturedFrame, ref boundingRect));
                Debug.Log(boundingRect);

                m_frameCapturer.m_shouldCaptureOnNextFrame = true;
            }
            else
            {
                 m_frameCapturer.m_shouldCaptureOnNextFrame = true;
                 m_isScreenBeingCaptured = true;
            }
        }
    }
}
