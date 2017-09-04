using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

// testing out retrieving the color from the rendered content by hiding the cursor for a frame and reading the RT.
public class RealWorldColorPickerController : MonoBehaviour {

	// Script inputs
	public Camera m_arCamera;
	public int m_offsetFromCenter = 10;
	public GameObject m_actorPrefab;
	public GameObject m_shadowPlanePrefab;
	public CursorManager m_cursorManager;

	// Privates
	bool m_isObjectPlaced = false;
	bool m_isCursorHidden = false;
	Material m_materialToUpdate;
	GameObject m_actor;
	GameObject m_shadowPlane;
	int m_animationId;
	Animator m_actorAnimator;
    CaptureCenterPixel m_pixelCapturer;

    // Use this for initialization
    void Start () {
		m_animationId = Animator.StringToHash ("wasColored");
        m_pixelCapturer = m_arCamera.GetComponent<CaptureCenterPixel>();
    }
	
	// Update is called once per frame
	void Update () {
		if (!m_isObjectPlaced) {
			if (Utils.WasTouchStartDetected()) {
				// Place the actor & it's shadow plane at the last detected cursor postion.
				var pos = m_cursorManager.GetCurrentCursorPosition();
				var rot = m_cursorManager.GetCurrentCursorRotation();

				if (m_actor == null) {
					m_actor = GameObject.Instantiate (m_actorPrefab, pos, rot);
					m_materialToUpdate = Utils.FindMaterialOnObject(m_actor, "COLOR BASICO 04");
					m_actorAnimator = m_actor.GetComponentInChildren<Animator> ();
				}

				if (m_shadowPlane == null) {
					m_shadowPlane = GameObject.Instantiate (m_shadowPlanePrefab, pos, rot);
				}

				m_isObjectPlaced = true;
			}
		} else if (m_isCursorHidden) {
			Debug.Log ("Capture color and enable cursor");
			m_actorAnimator.SetTrigger (m_animationId);
            
            m_materialToUpdate.color = m_pixelCapturer.m_lastCapturedColor;

            m_cursorManager.Enable();
			m_isCursorHidden = false;
		} else if (Utils.WasTouchStartDetected()) {
			Debug.Log ("Hide cursor and render a frame before capturing the color.");
			m_cursorManager.Disable();
            m_pixelCapturer.m_shouldCaptureOnNextFrame = true;
            m_isCursorHidden = true;
		}
	}
}
