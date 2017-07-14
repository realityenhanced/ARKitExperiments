using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class RealWorldColorPickerController : MonoBehaviour {

	// Script inputs
	public Camera m_arCamera;

	// Privates
	UnityARVideo m_arVideo;

	// Use this for initialization
	void Start () {
		m_arVideo = m_arCamera.GetComponent<UnityARVideo> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Utils.WasTouchDetected()) {
			Debug.Log ("Touch detected. Sample the video textures and set it on the object.");
			Resolution res = Screen.currentResolution;
			int x = res.width / 2 - 1;
			int y = res.height / 2 - 1;
			var color = GetRealWorldColorAt (x, y);
		}
	}

	Color GetRealWorldColorAt(int x, int y) {
		Color realWorldColor = Color.green;

		var videoTextureY = (Texture2D)m_arVideo.m_ClearMaterial.GetTexture ("_textureY");
		var videoTextureCbCr = (Texture2D)m_arVideo.m_ClearMaterial.GetTexture ("_textureCbCr");
		if (videoTextureY && videoTextureCbCr) {
			// Get the Y & CbCr components at the screen pos.
			// ...

		} else {
			Debug.Log ("Skip color check: Textures are invalid.");
		}

		Debug.Log ("Color = " + realWorldColor.ToString());

		return realWorldColor;
	}
}
