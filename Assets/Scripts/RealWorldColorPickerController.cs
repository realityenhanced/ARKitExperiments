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
	UnityARVideo m_arVideo;
	Texture2D m_centerPixTex;
	bool m_isObjectPlaced = false;
	bool m_isCursorHidden = false;
	Material m_materialToUpdate;
	GameObject m_actor;
	GameObject m_shadowPlane;
	int m_animationId;
	Animator m_actorAnimator;

	// Use this for initialization
	void Start () {
		m_arVideo = m_arCamera.GetComponent<UnityARVideo> ();
		m_centerPixTex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
		m_animationId = Animator.StringToHash ("wasColored");
	}
	
	// Update is called once per frame
	void Update () {
		if (!m_isObjectPlaced) {
			if (Utils.WasTouchDetected()) {
				// Place the actor & it's shadow plane at the last detected cursor postion.
				var pos = m_cursorManager.GetCurrentCursorPosition();
				var rot = m_cursorManager.GetCurrentCursorRotation();

				if (m_actor == null) {
					var go = Utils.SpawnGameObjectAt (m_actorPrefab, pos, rot);
					m_materialToUpdate = Utils.FindMaterialOnObject(go, "COLOR BASICO 04");
					m_actorAnimator = go.GetComponentInChildren<Animator> ();
				}

				if (m_shadowPlane == null) {
					m_shadowPlane = Utils.SpawnGameObjectAt(m_shadowPlanePrefab, pos, rot);
				}

				m_isObjectPlaced = true;
			}
		} else if (m_isCursorHidden) {
			Debug.Log ("Capture color and enable cursor");
			Resolution res = Screen.currentResolution;
			int x = res.width / 2 - 1;
			int y = res.height / 2 - 1;

			m_materialToUpdate.color = GetRealWorldColorAt (x, y);
			m_actorAnimator.SetTrigger (m_animationId);

			m_cursorManager.Enable();
			m_isCursorHidden = false;
		} else if (Utils.WasTouchDetected()) {
			Debug.Log ("Hide cursor and render a frame before capturing the color.");
			m_cursorManager.Disable();
			m_isCursorHidden = true;
		}
	}

	// Helpers
	Color GetRealWorldColorAt(int x, int y) {
		Color realWorldColor = Color.green;

		if (!m_centerPixTex) {
			m_centerPixTex = new Texture2D (1, 1, TextureFormat.RGBA32, false);
		}

		// Hacky: Make sure the pixel is not occluded by the cursor.
		m_centerPixTex.ReadPixels(
			new Rect(Screen.currentResolution.width/2 - 1, Screen.currentResolution.height/2 - 1, 1,1), 0, 0);
		m_centerPixTex.Apply();

		realWorldColor = m_centerPixTex.GetPixels()[0];
	
		Debug.Log ("Color = " + realWorldColor.ToString());

		return realWorldColor;
	}
}
