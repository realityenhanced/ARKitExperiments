using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class OpencvExperimentController : MonoBehaviour {
    // Inputs
    public GameObject m_actorPrefab;
    public float m_scanInterval = 1.0f;

    // Privates
    OpencvProcessingInterface m_opencvProcessing;
    bool m_isScreenBeingCaptured = false;
    CaptureFrame m_frameCapturer;
    UnityARSessionNativeInterface m_arSession;
    private Transform m_actor;
    private Rect m_boundingRect = new Rect(0,0,0,0);
    const ARHitTestResultType m_resultTypeToUse = ARHitTestResultType.ARHitTestResultTypeFeaturePoint;
    ARPoint m_hitPoint = new ARPoint
    {
        x = 0.5f,
        y = 0.5f
    };
    Material m_materialToUpdate;

    enum STATE
    {
        UNINITIALIZED = 0,
        WAIT_FOR_TAP,
        START_SCANNING,
        IN_SCANNING_MODE
    }
    STATE m_state = STATE.UNINITIALIZED;

    // Use this for initialization
    void Start () {
        m_opencvProcessing = new OpencvProcessingInterface();
        m_frameCapturer = Camera.main.GetComponent<CaptureFrame>();
        m_arSession = UnityARSessionNativeInterface.GetARSessionNativeInterface();

        m_state = STATE.WAIT_FOR_TAP;
    }
    
    // Update is called once per frame
    void Update () {
        if (m_state == STATE.WAIT_FOR_TAP && m_isScreenBeingCaptured)
        {
            m_opencvProcessing.SaveDescriptorsForFrame(m_frameCapturer.m_lastCapturedFrame);
            m_isScreenBeingCaptured = false;
            m_state = STATE.START_SCANNING;
        }
        else if (m_state == STATE.WAIT_FOR_TAP && Utils.WasTouchStartDetected())
        {
            // On a touch event, capture the screen and on the next Update call, save the descriptors for the image. 
            m_frameCapturer.m_shouldCaptureOnNextFrame = true;
            m_isScreenBeingCaptured = true;
        }
        else if (m_state == STATE.START_SCANNING)
        {
            StartCoroutine(ScanForMatchesAndUpdateActorPosition(m_scanInterval));
            m_state = STATE.IN_SCANNING_MODE;
        }
    }

    // Coroutine that scans for matching descriptors and places the actor object at the location of the match.
    private IEnumerator ScanForMatchesAndUpdateActorPosition(float timeInterval) {
        if (m_actor == null)
        {
            m_actor = GameObject.Instantiate(m_actorPrefab).transform;
            m_materialToUpdate = Utils.FindMaterialOnObject(m_actor.gameObject, "COLOR BASICO 04");
        }

        while(true)
        {
            if (m_isScreenBeingCaptured)
            {
                if (m_opencvProcessing.MatchDescriptorsForFrame(m_frameCapturer.m_lastCapturedFrame, ref m_boundingRect) == 1)
                {
                    float x = m_boundingRect.x + m_boundingRect.width / 2;
                    float y = m_boundingRect.y + m_boundingRect.height / 2;

                    PlaceActorAt(x, y);
                    m_materialToUpdate.color = m_opencvProcessing.GetColorFromFrameAt((int)x, (int)y);
                    m_actor.gameObject.SetActive(true);
                }
                else
                {
                    // Hide the actor if no matches were found.
                    m_actor.gameObject.SetActive(false);
                }
                
                m_isScreenBeingCaptured = false;
                yield return new WaitForSeconds(timeInterval);
            }
            else
            {
                // Hide the actor to allow only the camera contents to be captured.
                m_actor.gameObject.SetActive(false);

                m_frameCapturer.m_shouldCaptureOnNextFrame = true;
                m_isScreenBeingCaptured = true;
            }
        }
    }

    private void PlaceActorAt(float x, float y)
    {
        m_hitPoint.x = x / (float)Screen.currentResolution.width;
        m_hitPoint.y = y / (float)Screen.currentResolution.height;

        List<ARHitTestResult> hitResults = m_arSession.HitTest(m_hitPoint, m_resultTypeToUse);
        if (hitResults.Count > 0)
        {
            ARHitTestResult hitResult = Utils.GetFirstValidHit(hitResults);
            if (hitResult.isValid)
            {
                m_actor.position = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
            }
        }
        else
        {
            Debug.Log("No hit points");
        }
    }
}
