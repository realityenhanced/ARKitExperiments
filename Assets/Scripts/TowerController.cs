using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour {
	// Use this for initialization
	void Start () {
		// Allow the physics system to start acting on the children.
		Utils.SetChildrenAsKinematic (this.gameObject, false);
	}
}
