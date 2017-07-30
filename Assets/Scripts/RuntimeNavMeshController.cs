using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuntimeNavMeshController : SceneController {

	// Script inputs
	public GameObject m_generatedQuadPrefab;
	public CursorManager m_cursorManager;
	public Color m_quadColor = Color.blue;
	public Transform m_quadsHolder;
	public GameObject m_menuButtonPrefab;

	// Privates
	GameObject m_partialQuad;
	Mesh m_partialMesh;
	int m_numVerticesAdded = 0;
	bool m_isInScanMode = true;

	int[] COUNT_TO_INDEX_MAP =  new int[4] {3,1,2,0};

	const string SCAN_MODE_TEXT = "Toggle [Scan]";
	const string PLACE_MODE_TEXT = "Toggle [Place]";

	// Use this for initialization
	protected override void Start () {
		base.Start ();
		UpdateButtonText ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Utils.WasTouchStartDetected ()) {
			Vector3 cursorPos = m_cursorManager.GetCurrentCursorPosition ();

			if (m_isInScanMode) {
				if (m_numVerticesAdded == 0) {
					InitializePartialMesh (cursorPos);
					++m_numVerticesAdded;
				} else {
					AddNextVertex (cursorPos);

					if (m_numVerticesAdded == 4) {
						m_partialMesh.RecalculateBounds ();
						m_partialQuad.transform.parent = m_quadsHolder;

						m_partialMesh = null;
						m_partialQuad = null;
						m_numVerticesAdded = 0;
					}
				}
			}
		}
	}

	public void OnToggleClicked() {
		m_isInScanMode = !m_isInScanMode;
		m_quadsHolder.gameObject.SetActive (m_isInScanMode);
		UpdateButtonText ();
	}

	void InitializePartialMesh (Vector3 pos)
	{
		m_partialQuad = Instantiate (m_generatedQuadPrefab);
		m_partialMesh = m_partialQuad.GetComponent<MeshFilter> ().mesh;
		m_partialMesh.colors = new Color[4] { m_quadColor, m_quadColor, m_quadColor, m_quadColor };
		InitMeshVertices (pos);
	}

	void AddNextVertex (Vector3 cursorPos)
	{
		Vector3[] vertices = m_partialMesh.vertices;
		vertices[COUNT_TO_INDEX_MAP [m_numVerticesAdded]] = cursorPos;
		m_partialMesh.vertices = vertices;

		++m_numVerticesAdded;
	}

	void InitMeshVertices (Vector3 pos)
	{
		Vector3[] vertices = m_partialMesh.vertices;
		for (int i = 0; i < 4; ++i) {
			vertices [i] = pos;
		}
		m_partialMesh.vertices = vertices;
	}

	void UpdateButtonText() {
		m_menuButtonPrefab.GetComponentInChildren<Text> ().text = m_isInScanMode ? SCAN_MODE_TEXT : PLACE_MODE_TEXT;
	}
}
