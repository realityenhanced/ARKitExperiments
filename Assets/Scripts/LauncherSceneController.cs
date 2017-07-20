using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LauncherSceneController : MonoBehaviour {
	// Inputs
	public string[] m_buttonMapping; // Button Name, Scene Name stored consecutively.
	public RectTransform m_menuContainer;
	public GameObject m_menuButtonPrefab;

	void Start() {
		for (int i = 0; i < m_buttonMapping.GetLength (0)/2; ++i) {
			GameObject button = Instantiate (m_menuButtonPrefab);
			button.GetComponentInChildren<Text> ().text = m_buttonMapping [i * 2];

			RectTransform trans = (RectTransform)button.GetComponent<RectTransform>();
			trans.SetPositionAndRotation(new Vector3(0, (trans.rect.height * i) + 10.0f, 1.0f), Quaternion.identity);
			button.transform.SetParent (m_menuContainer);

			var clickEvent = button.GetComponent<Button>().onClick;

			int index = i * 2 + 1;
			clickEvent.AddListener(() => OnSceneSelection( index ) );
		}
	}

	public void OnSceneSelection(int index) {
		SceneManager.LoadScene (m_buttonMapping[index], LoadSceneMode.Single);
	}
}
