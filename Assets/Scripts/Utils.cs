using System;
using UnityEngine;

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
}
