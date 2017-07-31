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
	public GameObject m_menuButtonPrefab;
	public LineRenderer m_lineRenderer;
	public GameObject m_actorPrefab;

	// Privates
	GameObject m_partialQuad;
	Mesh m_partialMesh;
	int m_numVerticesAdded = 0;
	bool m_isInScanMode = true;
	GameObject m_actor;
	NavMeshAgent m_actorAgent;

	int[] COUNT_TO_INDEX_MAP =  new int[4] {3,1,2,0};
	Vector3 m_viewPortCenter = new Vector3 (0.5f, 0.5f, 0.0f);
	const string SCAN_MODE_TEXT = "Toggle [Scan]";
	const string PLACE_MODE_TEXT = "Toggle [Place]";

	// Use this for initialization
	protected override void Start () {
		base.Start ();
		UpdateButtonText ();
	}
	
	// Update is called once per frame
	void Update () {
		if (m_isInScanMode) {
			
			Vector3 cursorPos = m_cursorManager.GetCurrentCursorPosition ();
			if (Utils.WasTouchStartDetected ()) {
				if (EventSystem.current.IsPointerOverGameObject ()) {
					// Ignore touch events if a button is pressed
					return;
				}

				if (m_numVerticesAdded == 0) {
					InitializePartialMesh (cursorPos);
					++m_numVerticesAdded;
				} else {
					AddNextVertex (cursorPos);

					if (m_numVerticesAdded == 4) {
						m_partialMesh.RecalculateBounds ();
						m_partialQuad.transform.parent = m_quadsHolder;

						// Build the nav mesh when the quad is added.
						Debug.Log("NavMeshBuilt");
						m_partialQuad.GetComponent<NavMeshSurface> ().BuildNavMesh ();

						m_partialMesh = null;
						m_partialQuad = null;
						m_numVerticesAdded = 0;
					}
				}
			} else {
				if (m_numVerticesAdded > 0) {
					m_lineRenderer.SetPosition (m_numVerticesAdded, cursorPos);
				}
			}
		} else {
			if (Utils.WasTouchStartDetected ()) {
				if (EventSystem.current.IsPointerOverGameObject ()) {
					// Ignore touch events if a button is pressed
					return;
				}

				RaycastHit hitInfo;
				Ray ray = Camera.current.ViewportPointToRay (m_viewPortCenter);
				if (Physics.Raycast (ray, out hitInfo)) {
					var cursorPos = hitInfo.point;
					Debug.Log("Name = " + hitInfo.transform.gameObject.name);
					Debug.Log(System.String.Format("Up Vector of hit : {0} {1} {2}", hitInfo.transform.up.x, hitInfo.transform.up.y, hitInfo.transform.up.z));
					Debug.Log(System.String.Format("Raycast hit : {0} {1} {2}", cursorPos.x, cursorPos.y, cursorPos.z));
					if (m_actor == null) {
						m_actor = GameObject.Instantiate (m_actorPrefab, cursorPos, Quaternion.identity);
						m_actorAgent = m_actor.GetComponent<NavMeshAgent> ();
					} else {						
						m_actorAgent.SetDestination (cursorPos);
						m_actorAgent.isStopped = false;
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
		m_partialMesh = m_partialQuad.GetComponentInChildren<MeshFilter> ().mesh;
		m_partialMesh.colors = new Color[4] { m_quadColor, m_quadColor, m_quadColor, m_quadColor };
		InitMeshVertices (pos);

		m_lineRenderer.SetPositions (new Vector3[4]{pos,pos,pos,pos});
	}

	void AddNextVertex (Vector3 cursorPos)
	{
		Vector3[] vertices = m_partialMesh.vertices;
		vertices[COUNT_TO_INDEX_MAP [m_numVerticesAdded]] = cursorPos;
		m_partialMesh.vertices = vertices;

		m_lineRenderer.SetPosition (m_numVerticesAdded, cursorPos);

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
