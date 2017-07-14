using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

// testing out retrieving the color from the rendered content by hiding the cursor for a frame and reading the RT.
public class RealWorldColorPickerController : MonoBehaviour {

	// Script inputs
	public Camera m_arCamera;
	public int m_offsetFromCenter = 10;
	public GameObject m_objectToColor;
	public CursorManager m_cursorManager;

	// Privates
	UnityARVideo m_arVideo;
	Texture2D m_centerPixTex;
	bool m_isCursorHidden = false;

	// Use this for initialization
	void Start () {
		m_arVideo = m_arCamera.GetComponent<UnityARVideo> ();
		m_centerPixTex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
	}
	
	// Update is called once per frame
	void Update () {
		if (m_isCursorHidden) {
			Debug.Log ("Capture color and enable cursor");
			Resolution res = Screen.currentResolution;
			int x = res.width / 2 - 1;
			int y = res.height / 2 - 1;
			m_objectToColor.GetComponent<MeshRenderer>().material.color = GetRealWorldColorAt (x, y);

			m_cursorManager.Enable();
			m_isCursorHidden = false;
		} else if (Utils.WasTouchDetected()) {
			Debug.Log ("Hide cursor and render a frame before capturing the color.");
			m_cursorManager.Disable();
			m_isCursorHidden = true;
		}
	}

	// Helpers
	Color GetRealWorldColorAt(int x, int y) {
		Color realWorldColor = Color.green;

		if (!m_centerPixTex) {
			m_centerPixTex = new Texture2D (1, 1, TextureFormat.RGBA32, false);
		}

		// Hacky: Make sure the pixel is not occluded by the cursor.
		m_centerPixTex.ReadPixels(
			new Rect(Screen.currentResolution.width/2 - 1, Screen.currentResolution.height/2 - 1, 1,1), 0, 0);
		m_centerPixTex.Apply();

		realWorldColor = m_centerPixTex.GetPixels()[0];
	
		Debug.Log ("Color = " + realWorldColor.ToString());

		return realWorldColor;
	}
}
