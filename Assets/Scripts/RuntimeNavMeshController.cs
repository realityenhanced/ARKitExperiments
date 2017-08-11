using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class RuntimeNavMeshController : SceneController {

	// Script inputs
	public GameObject m_generatedQuadPrefab;
	public CursorManager m_cursorManager;
	public Color m_quadColor = Color.blue;
	public Transform m_quadsHolder;
	public GameObject m_menuButton;
	public LineRenderer m_lineRenderer;
	public GameObject m_actorPrefab;
	public Material m_scanModeMaterial;
	public Material m_placeModeMaterial;
	public GameObject m_navMeshLinkPrefab;

	// Privates
	GameObject m_partialQuad;
	Mesh m_partialMesh;
	int m_numVerticesAdded = 0;
	bool m_isInScanMode = true;
	GameObject m_actor;
	NavMeshAgent m_actorAgent;
	GameObject m_lastQuadAdded;
	List<Vector3[]> m_quadVerticesBeforeTilt = new List<Vector3[]>();

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
				if (m_actor == null) {
					// On the first touch, spawn the actor.
					m_actor = GameObject.Instantiate (m_actorPrefab, cursorPos, Quaternion.identity);
					m_actorAgent = m_actor.GetComponent<NavMeshAgent> ();
				} else {
					// On subsequent touches, move the dog on the nav mesh.
					m_actorAgent.SetDestination (cursorPos);
					m_actorAgent.isStopped = false;

					m_actor.GetComponent<AudioSource> ().Play ();
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
		m_partialMesh = m_partialQuad.GetComponentInChildren<MeshFilter> ().mesh;
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
					UpdateUpVectorOfObject (m_partialQuad);

					// Update the mesh collider
					var meshCollider = m_partialQuad.GetComponentInChildren<MeshCollider> ();
					meshCollider.sharedMesh = m_partialMesh;

					m_partialQuad.transform.parent = m_quadsHolder;

					// Build the nav mesh when the quad is added.
					var navMesh = m_partialQuad.GetComponent<NavMeshSurface> ();
					navMesh.BuildNavMesh ();

					if (m_lastQuadAdded != null) {
						UpdateNavMeshLink (m_lastQuadAdded, m_partialQuad);
					}

					m_lastQuadAdded = m_partialQuad;
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

	void UpdateNavMeshLink(GameObject quad1, GameObject quad2) {
		NavMeshLink linkOfQuad1 = quad1.GetComponent<NavMeshLink> ();

		var mesh1Verts = m_quadVerticesBeforeTilt [m_quadVerticesBeforeTilt.Count - 2];
		var mesh2Verts = m_quadVerticesBeforeTilt [m_quadVerticesBeforeTilt.Count - 1];

		// ASSUMPTION: Quads are alway drawn top left -> top right -> bottom right -> bottom left
		var topCenter1 = (mesh1Verts [0] + mesh1Verts [1]) / 2;
		var bottomCenter2 = (mesh2Verts[2] + mesh2Verts[3]) / 2;

		NavMeshHit hit;
		var point1 = topCenter1;
		var point2 = bottomCenter2;
		if (NavMesh.SamplePosition (topCenter1, out hit, 100.0f, NavMesh.AllAreas)) {
			point1 = quad1.transform.InverseTransformPoint(hit.position);
		}

		if (NavMesh.SamplePosition (bottomCenter2, out hit, 100.0f, NavMesh.AllAreas)) {
			point2 = quad1.transform.InverseTransformPoint(hit.position);
		}
			
		linkOfQuad1.startPoint = point1;
		linkOfQuad1.endPoint = point2;

		// Visualize link
		var linkObj1 = Instantiate (m_navMeshLinkPrefab, Vector3.zero, Quaternion.identity);
		linkObj1.transform.parent = quad1.transform;
		linkObj1.transform.localPosition = point1;

		var linkObj2 = Instantiate (m_navMeshLinkPrefab, Vector3.zero, Quaternion.identity);
		linkObj2.transform.parent = quad1.transform;
		linkObj2.transform.localPosition = point2;
		//

		linkOfQuad1.width = (mesh1Verts [1] - mesh1Verts [0]).magnitude;
		linkOfQuad1.UpdateLink ();
	}

	void UpdateUpVectorOfObject(GameObject obj) {
		var meshObjTransform = obj.transform.GetChild (0);
		var mesh = meshObjTransform.gameObject.GetComponent<MeshFilter> ().mesh;

		Vector3[] oldVertices = mesh.vertices;
		m_quadVerticesBeforeTilt.Add(oldVertices);

		Vector3[] vertices = mesh.vertices;

		// Use the cross product of the lines formed by three vertices on the quad.
		var normal = Vector3.Cross((vertices[3] - vertices[1]), (vertices[3] - vertices[2])).normalized;
		Quaternion tilt = Quaternion.FromToRotation(normal, meshObjTransform.up);

		// Counter the rotation that will be caused by the up vector update below.
		for (int i=0; i<vertices.GetLength(0); ++i) {
			vertices [i] = tilt * vertices [i];
		}

		mesh.vertices = vertices;
		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();

		obj.transform.up = normal;
	}
}