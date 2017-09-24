using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureSceneController : SceneController {

	// Script inputs
	public CursorManager m_cursorManager;
	public GameObject m_actorPrefab;

	// Privates
	protected Transform m_actorTransform;

	// Update is called once per frame
	void Update() {
		if (Utils.WasTouchStartDetected()) {
			PlaceActorAtCursor ();
		}
	}

	protected void PlaceActorAtCursor ()
	{
		// Place the actor & it's shadow plane at the last detected cursor postion.
		var pos = m_cursorManager.GetCurrentCursorPosition ();
		var rot = m_cursorManager.GetCurrentCursorRotation ();
		if (m_actorTransform == null) {
			m_actorTransform = GameObject.Instantiate (m_actorPrefab, pos, rot).transform;
		}
		else {
			m_actorTransform.position = pos;
			m_actorTransform.rotation = rot;
		}
	}
}
