using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OcclusionSceneController : ShadowSceneController {

	// Script inputs
	public GameObject m_occlusionPlanePrefab;
	public float m_durationOfAnimationInSeconds = 4.0f;

	// Privates
	Transform m_occlusionPlane;

	// Update is called once per frame
	void Update () {
		if (Utils.WasTouchStartDetected ()) {
			PlaceActorAtCursor ();
			PlaceOcclusionPlaneAtCursor ();

			StopAllCoroutines ();
			StartCoroutine (MoveActorUp (m_durationOfAnimationInSeconds));
		}
	}

	IEnumerator MoveActorUp(float durationInSeconds) {
		var actorHeight = Utils.GetWorldSpaceHeight (m_actorTransform.gameObject);
		m_actorTransform.position = new Vector3(m_actorTransform.position.x, m_actorTransform.position.y - actorHeight, m_actorTransform.position.z);

		const float updatesPerSecond = 30.0f;
		float incrementPerUpdate = actorHeight / (durationInSeconds * updatesPerSecond);
		while (actorHeight > 0) {
			m_actorTransform.position = new Vector3(m_actorTransform.position.x, m_actorTransform.position.y + incrementPerUpdate, m_actorTransform.position.z);
			actorHeight -= incrementPerUpdate;
			yield return new WaitForSeconds (1.0f / updatesPerSecond);
		}

		// Start animating the actor after moving it up.
		var animator = m_actorTransform.gameObject.GetComponentInChildren<Animator>();
		animator.SetTrigger (Animator.StringToHash ("startAnimating"));
	}

	void PlaceOcclusionPlaneAtCursor(){
		var pos = m_cursorManager.GetCurrentCursorPosition ();
		var rot = m_cursorManager.GetCurrentCursorRotation ();
		if (m_occlusionPlane == null) {
			m_occlusionPlane = GameObject.Instantiate (m_occlusionPlanePrefab, pos, rot).transform;
		}
		else {
			m_occlusionPlane.position = pos;
			m_occlusionPlane.rotation = rot;
		}
	}
}
