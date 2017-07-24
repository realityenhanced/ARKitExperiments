using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour {
	protected virtual void Start () {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;	
	}
}
