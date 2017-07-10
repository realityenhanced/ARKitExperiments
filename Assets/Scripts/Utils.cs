using System;
using UnityEngine;
using UnityEngine.XR.iOS;
using System.Collections.Generic;

public class Utils
{
	static public bool WasTouchDetected()
	{
		return Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Began;
	}

	static public GameObject SpawnGameObjectAt(GameObject prefab, Vector3 pos, Quaternion rot)
	{
		var actor = GameObject.Instantiate (prefab);	
		actor.transform.position = pos;
		actor.transform.rotation = rot;

		return actor;
	}

	static public ARHitTestResult GetFirstValidHit(List<ARHitTestResult> hitResults)
	{
		ARHitTestResult hitResult = hitResults[0]; // Return the first hit, if no valid hits were found.
		foreach (var h in hitResults) {
			if (h.isValid) {
				hitResult = h;
				break;
			}
		}
		return hitResult;
	}
}
