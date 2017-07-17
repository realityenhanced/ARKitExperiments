using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OcclusionSceneController : ShadowSceneController {

	// Script inputs
	public GameObject m_occlusionPlanePrefab;

	// Privates
	Transform m_occlusionPlane;

	// Update is called once per frame
	void Update () {
		if (Utils.WasTouchDetected ()) {
			PlaceActorAtCursor ();
			PlaceOcclusionPlaneAtCursor ();
		}
	}

	void PlaceOcclusionPlaneAtCursor(){
		var pos = m_cursorManager.GetCurrentCursorPosition ();
		var rot = m_cursorManager.GetCurrentCursorRotation ();
		if (m_occlusionPlane == null) {
			m_occlusionPlane = Utils.SpawnGameObjectAt (m_occlusionPlanePrefab, pos, rot).transform;
		}
		else {
			m_occlusionPlane.position = pos;
			m_occlusionPlane.rotation = rot;
		}
	}
}
