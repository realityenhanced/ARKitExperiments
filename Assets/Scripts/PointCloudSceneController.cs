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
	ParticleSystem m_particleSystem;
	bool m_isScreenPressed = false;
	Vector3 m_lastPosition = Vector3.zero;
	uint m_currentPointCloudId = 0;

	void Start() {
		m_pointCloud = new List<Vector3> ();
		m_particleSystem = Instantiate (m_particleSystemPrefab);

		// Clear any ply files that were written last time.
		Utils.DeleteAllAppFiles ();
	}

	// Update is called once per frame
	void Update () {
		if (Utils.WasTouchStartDetected ()) {
			m_pointCloud.Clear ();
			AddPoint(m_cursorManager.GetCurrentCursorPosition ());
			m_isScreenPressed = true;
		} else if (Utils.WasTouchStopDetected ()) {
			Utils.SavePointCloudToPlyFile("PointCloud_" + m_currentPointCloudId + ".ply");
			++m_currentPointCloudId;
			m_isScreenPressed = false;
		} else if (m_isScreenPressed) {
			Vector3 currentPos = m_cursorManager.GetCurrentCursorPosition ();
			float distance = Vector3.Distance (currentPos, m_lastPosition);
			Debug.Log ("Distance = " + distance.ToString ());
			if (distance > m_lowerThreshold && distance < m_upperThreshold) {
				AddPoint (currentPos);
			}
		}
	}

	void AddPoint(Vector3 point) {
		m_pointCloud.Add (point);
		m_lastPosition = point;
		UpdateParticles ();
	}

	void UpdateParticles() {
		if (m_pointCloud.Count > 0) {
			int numParticles = m_pointCloud.Count;
			ParticleSystem.Particle[] particles = new ParticleSystem.Particle[numParticles];
			for (int i = 0; i < numParticles; ++i) {
				Vector3 currentPoint = m_pointCloud [i];
				particles [i].position = currentPoint;
				particles [i].startColor = m_particleColor;
				particles [i].startSize = m_particleSize;
			}
			m_particleSystem.SetParticles (particles, numParticles);
		} else {
			ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1];
			particles [0].startSize = 0.0f;
			m_particleSystem.SetParticles (particles, 1);
		}
	}
}
