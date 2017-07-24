using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloudSceneController : SceneController {

	// Inputs
	public ParticleSystem m_particleSystemPrefab;
	public float m_particleSize = 0.01f;
	public CursorManager m_cursorManager;
	public Color m_particleColor = Color.green;
	public float m_lowerThreshold = 0.01f;
	public float m_upperThreshold = 0.05f;

	// Privates
	List<Vector3> m_pointCloud;
	List<Color> m_pointColors;
	ParticleSystem m_particleSystem;
	bool m_isCaptureInProgress = false;
	Vector3 m_lastPosition = Vector3.zero;
	uint m_currentPointCloudId = 0;
	Texture2D m_screenPixelTexture;
	bool m_shouldSampleColorOnNextFrame = false;

	void Start() {
		m_pointCloud = new List<Vector3> ();
		m_pointColors = new List<Color> ();
		m_particleSystem = Instantiate (m_particleSystemPrefab);

		// Clear any ply files that were written last time.
		Utils.DeleteAllAppFiles ();
	}

	// Update is called once per frame
	void Update () {
		if (Utils.WasTouchStartDetected ()) {
			if (m_isCaptureInProgress) {
				StopCaptureSession ();
				m_isCaptureInProgress = false;
			} else {
				ClearPointCloud ();
				AddPoint (m_cursorManager.GetCurrentCursorPosition ());
				m_isCaptureInProgress = true;
			}
		} else if (m_isCaptureInProgress) {
			if (m_shouldSampleColorOnNextFrame) {
				SampleColor ();
			} else {
				Vector3 currentPos = m_cursorManager.GetCurrentCursorPosition ();
				float distance = Vector3.Distance (currentPos, m_lastPosition);
				if (distance > m_lowerThreshold && distance < m_upperThreshold) {
					AddPoint (currentPos);
				}
			}
		}
	}

	// Helpers
	void AddPoint(Vector3 point) {
		m_pointCloud.Add (point);
		m_lastPosition = point;
		UpdateParticles (false);

		// Render only the phone's camera view on the next frame for capturing the color.
		ShowSceneElements(false);
		m_shouldSampleColorOnNextFrame = true;
	}

	void HideParticles ()
	{
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1];
		particles [0].startSize = 0.0f;
		m_particleSystem.SetParticles (particles, 1);
	}

	void UpdateParticles(bool shouldShowPointColor) {
		if (m_pointCloud.Count > 0) {
			int numParticles = m_pointCloud.Count;
			ParticleSystem.Particle[] particles = new ParticleSystem.Particle[numParticles];
			for (int i = 0; i < numParticles; ++i) {
				Vector3 currentPoint = m_pointCloud [i];
				particles [i].position = currentPoint;

				if (shouldShowPointColor) {
					particles [i].startColor = m_pointColors [i];
				} else {
					particles [i].startColor = m_particleColor;					
				}

				particles [i].startSize = m_particleSize;
			}
			m_particleSystem.SetParticles (particles, numParticles);
		} else {
			HideParticles ();
		}
	}

	Color GetScreenColorAt(int x, int y) {
		if (!m_screenPixelTexture) {
			m_screenPixelTexture = new Texture2D (1, 1, TextureFormat.RGBA32, false);
		}

		m_screenPixelTexture.ReadPixels(
			new Rect(x, y, 1, 1),
			0, 0);
		m_screenPixelTexture.Apply();

		return m_screenPixelTexture.GetPixels()[0];
	}

	void ClearPointCloud ()
	{
		m_pointCloud.Clear ();
		m_pointColors.Clear ();
	}

	void ShowSceneElements (bool shouldShow)
	{
		if (shouldShow) {
			m_cursorManager.Enable ();
			UpdateParticles (false);
		} else {
			m_cursorManager.Disable ();
			HideParticles ();
		}
	}

	void SampleColor ()
	{
		var color = GetScreenColorAt (Screen.currentResolution.width / 2 - 1, Screen.currentResolution.height / 2 - 1);
		m_pointColors.Add (color);
		ShowSceneElements (true);
		m_shouldSampleColorOnNextFrame = false;
	}

	void StopCaptureSession ()
	{
		if (m_shouldSampleColorOnNextFrame) {
			SampleColor ();
		}

		// Show actual point colors on the particles once capture session has ended.
		UpdateParticles (true);

		Utils.SavePointCloudToPlyFile (m_pointCloud, m_pointColors, "PointCloud_" + m_currentPointCloudId + ".ply");
		++m_currentPointCloudId;
	}
}
