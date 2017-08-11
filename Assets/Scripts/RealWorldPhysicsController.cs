using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AI;

// TODO: Refactor common aspects into individual components
public class RealWorldPhysicsController : SceneController {

	// Script inputs
	public GameObject m_generatedQuadPrefab;
	public CursorManager m_cursorManager;
	public Color m_quadColor = Color.blue;
	public Transform m_quadsHolder;
	public GameObject m_menuButton;
	public LineRenderer m_lineRenderer;
	public GameObject m_towerPrefab;
	public GameObject m_ballPrefab;
	public Material m_scanModeMaterial;
	public Material m_placeModeMaterial;

	// Privates
	GameObject m_partialQuad;
	Mesh m_partialMesh;
	int m_numVerticesAdded = 0;
	bool m_isInScanMode = true;
	GameObject m_tower;

	const string SCAN_MODE_TEXT = "Toggle [Scan]";
	const string PLACE_MODE_TEXT = "Toggle [Place]";

	// Use this for initialization
	protected override void Start () {
		base.Start ();
		UpdateButtonText ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Utils.IsTouchOnUI()) {
			// Ignore touch events if a button is pressed
			return;
		}

		Vector3 cursorPos = m_cursorManager.GetCurrentCursorPosition ();
		if (m_isInScanMode) {
			PerformScan (cursorPos);
		} else {
			if (Utils.WasTouchStartDetected ()) {
				var cameraTransform = Camera.current.transform;
				if (m_tower == null) {
					m_tower = GameObject.Instantiate (m_towerPrefab, cursorPos, Quaternion.Euler(0, 90.0f + cameraTransform.rotation.eulerAngles.y, 0));
				} else {
					var ball = GameObject.Instantiate (m_ballPrefab, cameraTransform.position, Quaternion.identity);
					ball.GetComponentInChildren<Rigidbody> ().AddForce (cameraTransform.forward * 200.0f);
				}
			}
		}
	}

	public void OnToggleClicked() {
		m_isInScanMode = !m_isInScanMode;

		m_lineRenderer.enabled = m_isInScanMode;
		Utils.SetMaterialOnChildren (m_quadsHolder, m_isInScanMode ? m_scanModeMaterial : m_placeModeMaterial);
		m_cursorManager.SetMode (m_isInScanMode);

		UpdateButtonText ();
	}

	// Helpers
	void InitializePartialMesh (Vector3 pos)
	{
		m_partialQuad = Instantiate (m_generatedQuadPrefab);
		m_partialMesh = m_partialQuad.GetComponent<MeshFilter> ().mesh;
		m_partialMesh.colors = new Color[4] { m_quadColor, m_quadColor, m_quadColor, m_quadColor };
		m_partialMesh.triangles = new int[6] {0, 1, 2, 0, 2, 3};
		InitMeshVertices (pos);

		m_lineRenderer.SetPositions (new Vector3[4]{pos,pos,pos,pos});
	}

	void AddNextVertex (Vector3 cursorPos)
	{
		Vector3[] vertices = m_partialMesh.vertices;
		vertices[m_numVerticesAdded] = cursorPos;
		m_partialMesh.vertices = vertices;

		m_lineRenderer.SetPosition (m_numVerticesAdded, cursorPos);

		++m_numVerticesAdded;
	}

	void InitMeshVertices (Vector3 pos)
	{
		Vector3[] vertices = m_partialMesh.vertices;
		for (int i = 0; i < vertices.GetLength(0); ++i) {
			vertices [i] = pos;
		}
		m_partialMesh.vertices = vertices;
	}

	void UpdateButtonText() {
		m_menuButton.GetComponentInChildren<Text> ().text = m_isInScanMode ? SCAN_MODE_TEXT : PLACE_MODE_TEXT;
	}

	// Adds vertices of a quad on each touch and builds a nav mesh for each quad that was created.
	void PerformScan (Vector3 cursorPos)
	{
		if (Utils.WasTouchStartDetected ()) {
			if (m_numVerticesAdded == 0) {
				InitializePartialMesh (cursorPos);
				++m_numVerticesAdded;
			}
			else {
				AddNextVertex (cursorPos);
				if (m_numVerticesAdded == 4) {
					// Update the mesh collider
					var meshCollider = m_partialQuad.GetComponent<MeshCollider> ();
					meshCollider.sharedMesh = m_partialMesh;

					m_partialQuad.transform.parent = m_quadsHolder;

					m_partialMesh = null;
					m_partialQuad = null;
					m_numVerticesAdded = 0;
				}
			}
		}
		else {
			if (m_numVerticesAdded > 0) {
				m_lineRenderer.SetPosition (m_numVerticesAdded, cursorPos);
			}
		}
	}
}