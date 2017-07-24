using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class CursorManager : MonoBehaviour {

	// Constants

	// Use only feature points for hit testing at the center of the screen.
	ARPoint m_hitPointToUse = new ARPoint {
		x = 0.5f,
		y = 0.5f
	};
	const ARHitTestResultType m_resultTypeToUse = ARHitTestResultType.ARHitTestResultTypeFeaturePoint;
	const float m_delta = 0.0005f;

	// Private
	private Transform m_cursorTransform;

	// Public Inputs
	public GameObject m_cursorPrefab;
	public bool m_useAverageOfNeighbors = false;

	// Use this for initialization
	void Start () {
		m_cursorTransform = Instantiate(m_cursorPrefab).transform;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateTransformUsingWorldHitPoint(m_cursorTransform);
	}

	public Vector3 GetCurrentCursorPosition()
	{
		return m_cursorTransform.position;
	}

	public Quaternion GetCurrentCursorRotation()
	{
		return m_cursorTransform.rotation;
	}

	public void Enable() {
		m_cursorTransform.gameObject.SetActive(true);
	}

	public void Disable() {
		m_cursorTransform.gameObject.SetActive(false);
	}

	// Helpers
	bool UpdateTransformUsingWorldHitPoint(Transform transformToUpdate) {
		Vector3 position = Vector3.zero;

		bool wasCursorUpdated;
		if (m_useAverageOfNeighbors) {
			wasCursorUpdated = GetAverageCursorPosition (ref position);
		} else {
			wasCursorUpdated = GetCursorPosition (ref position);
		}

		m_cursorTransform.position = position;
		return wasCursorUpdated;	
	}

	bool GetCursorPosition(ref Vector3 positionToUpdate) {
		bool wasCursorFound = false;
		List<ARHitTestResult> hitResults = 
			UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(m_hitPointToUse, m_resultTypeToUse);
		if (hitResults.Count > 0) {
			ARHitTestResult hitResult = Utils.GetFirstValidHit (hitResults);
			if (hitResult.isValid) {
				positionToUpdate = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
				wasCursorFound = true;
			}
		}

		return wasCursorFound;
	}

	bool GetAverageCursorPosition(ref Vector3 positionToUpdate) {
		bool wasCursorFound = false;
		float[] pointDeltas = { -m_delta, -m_delta, -m_delta, m_delta, m_delta, 0 };

		Vector3 sum = Vector3.zero;
		ARPoint point = new ARPoint ();
		for (int i = 0; i < pointDeltas.GetLength (0) / 2; ++i) {			
			point.x = m_hitPointToUse.x + pointDeltas [i * 2];
			point.y = m_hitPointToUse.y + pointDeltas [i * 2 + 1];

			List<ARHitTestResult> hitResults = 
				UnityARSessionNativeInterface.GetARSessionNativeInterface ().HitTest (point, m_resultTypeToUse);
			if (hitResults.Count > 0) {
				ARHitTestResult hitResult = Utils.GetFirstValidHit (hitResults);
				if (hitResult.isValid) {
					sum += UnityARMatrixOps.GetPosition (hitResult.worldTransform);

					if (i == 2) {
						positionToUpdate = sum/3;
						wasCursorFound = true;
					}
				} else {
					break;
				}
			}
		}

		return wasCursorFound;
	}
}
