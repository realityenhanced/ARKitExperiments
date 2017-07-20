using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LauncherSceneController : MonoBehaviour {

	public void OnSceneSelection(string sceneName) {
		SceneManager.LoadScene (sceneName, LoadSceneMode.Single);
	}
}
