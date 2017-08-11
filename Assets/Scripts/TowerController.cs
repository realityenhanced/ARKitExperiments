using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour {
	// Script Inputs
	public float m_animationDuration = 4.0f;

	// Use this for initialization
	void Start () {
		StartCoroutine (MoveSelfUp (m_animationDuration));
	}

	IEnumerator MoveSelfUp(float durationInSeconds) {
		// Prevent the physics system to start acting on the children.
		Utils.SetChildrenAsKinematic (this.gameObject, true);

		var actorHeight = Utils.GetWorldSpaceHeight (this.gameObject);
		transform.position = new Vector3(transform.position.x, transform.position.y - actorHeight, transform.position.z);

		const float updatesPerSecond = 30.0f;
		float incrementPerUpdate = actorHeight / (durationInSeconds * updatesPerSecond);
		while (actorHeight > 0) {
			transform.position = new Vector3(transform.position.x, transform.position.y + incrementPerUpdate, transform.position.z);
			actorHeight -= incrementPerUpdate;
			yield return new WaitForSeconds (1.0f / updatesPerSecond);
		}

		// Allow the physics system to start acting on the children.
		Utils.SetChildrenAsKinematic (this.gameObject, false);
	}

}
