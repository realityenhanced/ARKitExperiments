using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class RealWorldPhysicsController : SceneController {

    // Script inputs
    public CursorManager m_cursorManager;
    public GameObject m_menuButton;
    public GameObject m_towerPrefab;
    public GameObject m_ballPrefab;

	// Privates
    bool m_isInScanMode = true;
	GameObject m_tower;

	const string SCAN_MODE_TEXT = "Toggle [Scan]";
	const string PLACE_MODE_TEXT = "Toggle [Place]";

	// Use this for initialization
	protected override void Start () {
		base.Start ();
		UpdateButtonText ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Utils.IsTouchOnUI()) {
			// Ignore touch events if a button is pressed
			return;
		}

		if (!m_isInScanMode) {
			if (Utils.WasTouchStartDetected ()) {
				var cameraTransform = Camera.main.transform;
				if (m_tower == null) {
                    Vector3 cursorPos = m_cursorManager.GetCurrentCursorPosition();
                    m_tower = GameObject.Instantiate (m_towerPrefab, cursorPos, Quaternion.Euler(0, 90.0f + cameraTransform.rotation.eulerAngles.y, 0));
				} else {
					var ball = GameObject.Instantiate (m_ballPrefab, cameraTransform.position, Quaternion.identity);
					ball.GetComponentInChildren<Rigidbody> ().AddForce (cameraTransform.forward * 1000.0f);
				}
			}
		}
	}

	public void OnToggleClicked() {
		m_isInScanMode = !m_isInScanMode;

        GetComponent<QuadMeshBuilder>().ToggleMeshVisibility(/*shouldShowMesh*/m_isInScanMode);
		m_cursorManager.SetMode (m_isInScanMode);

		UpdateButtonText ();
	}

	// Helpers
	void UpdateButtonText() {
		m_menuButton.GetComponentInChildren<Text> ().text = m_isInScanMode ? SCAN_MODE_TEXT : PLACE_MODE_TEXT;
	}
}