using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class OpencvExperimentController : MonoBehaviour {
    // Privates
    OpencvProcessingInterface m_opencvProcessing;
    bool m_isScreenBeingCaptured = false;
    CaptureFrame m_frameCapturer;
    bool m_shouldSaveDescriptor = true;
    public GameObject m_actorPrefab;

    private Transform m_actor;
    private Rect m_boundingRect = new Rect(0,0,0,0);
    const ARHitTestResultType m_resultTypeToUse = ARHitTestResultType.ARHitTestResultTypeFeaturePoint;

    ARPoint m_hitPoint = new ARPoint
    {
        x = 0.5f,
        y = 0.5f
    };

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
                Debug.Log(m_opencvProcessing.MatchDescriptorsForFrame(m_frameCapturer.m_lastCapturedFrame, ref m_boundingRect));
                Debug.Log(m_boundingRect);

                if (m_actor == null)
                {
                    m_actor = GameObject.Instantiate(m_actorPrefab).transform;
                }

                PlaceActorAt(m_boundingRect.x, m_boundingRect.y);

                m_frameCapturer.m_shouldCaptureOnNextFrame = true;
            }
            else
            {
                 m_frameCapturer.m_shouldCaptureOnNextFrame = true;
                 m_isScreenBeingCaptured = true;
            }
        }
    }

    private void PlaceActorAt(float x, float y)
    {
        m_hitPoint.x = x / (float)Screen.currentResolution.width;
        m_hitPoint.y = y / (float)Screen.currentResolution.height;

        Debug.Log("HIT POINT = " + m_hitPoint);

        List<ARHitTestResult> hitResults =
            UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(m_hitPoint, m_resultTypeToUse);
        if (hitResults.Count > 0)
        {
            ARHitTestResult hitResult = Utils.GetFirstValidHit(hitResults);
            if (hitResult.isValid)
            {
                m_actor.position = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
                Debug.Log("Actor pos updated");
            }
        }
        else
        {
            Debug.Log("No hit points");
        }
    }
}
