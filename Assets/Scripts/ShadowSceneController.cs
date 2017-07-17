using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowSceneController : SceneController {

	// Script inputs
	public CursorManager m_cursorManager;
	public GameObject m_actorPrefab;
	public GameObject m_shadowPlanePrefab;

	// Privates
	Transform m_actorTransform;
	Transform m_shadowPlaneTransform;

	// Update is called once per frame
	void Update() {
		if (Utils.WasTouchDetected()) {
			PlaceActorAtCursor ();
		}
	}

	protected void PlaceActorAtCursor ()
	{
		// Place the actor & it's shadow plane at the last detected cursor postion.
		var pos = m_cursorManager.GetCurrentCursorPosition ();
		var rot = m_cursorManager.GetCurrentCursorRotation ();
		if (m_actorTransform == null) {
			m_actorTransform = Utils.SpawnGameObjectAt (m_actorPrefab, pos, rot).transform;
		}
		else {
			m_actorTransform.position = pos;
			m_actorTransform.rotation = rot;
		}
		if (m_shadowPlaneTransform == null) {
			m_shadowPlaneTransform = Utils.SpawnGameObjectAt (m_shadowPlanePrefab, pos, rot).transform;
		}
		else {
			m_shadowPlaneTransform.position = pos;
			m_shadowPlaneTransform.rotation = rot;
		}
	}
}
