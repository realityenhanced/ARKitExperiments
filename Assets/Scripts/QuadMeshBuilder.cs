using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadMeshBuilder : MonoBehaviour {

    // Script inputs
    public CursorManager m_cursorManager;
    public GameObject m_generatedQuadPrefab;
    public Color m_quadColor = Color.blue;
    public Transform m_quadsHolder;
    public LineRenderer m_lineRenderer;
    public Material m_materialToUseWhenVisible;
    public Material m_materialToUseWhenHidden;

    // Privates
    GameObject m_partialQuad;
    Mesh m_partialMesh;
    int m_numVerticesAdded = 0;
    bool m_isHidden = false;
	
	void Update () {
        if (m_isHidden)
        {
            return;
        }

        if (Utils.IsTouchOnUI())
        {
            // Ignore touch events if a button is pressed
            return;
        }

        if (Utils.WasTouchStartDetected())
        {
            Vector3 cursorPos = m_cursorManager.GetCurrentCursorPosition();
            if (m_numVerticesAdded == 0)
            {
                InitializePartialMesh(cursorPos);
                ++m_numVerticesAdded;
            }
            else
            {
                AddNextVertex(cursorPos);
                if (m_numVerticesAdded == 4)
                {
                    // Update the mesh collider
                    var meshCollider = m_partialQuad.GetComponent<MeshCollider>();
                    meshCollider.sharedMesh = m_partialMesh;

                    m_partialQuad.transform.parent = m_quadsHolder;

                    m_partialMesh = null;
                    m_partialQuad = null;
                    m_numVerticesAdded = 0;
                }
            }
        }
        else
        {
            if (m_numVerticesAdded > 0)
            {
                Vector3 cursorPos = m_cursorManager.GetCurrentCursorPosition();
                m_lineRenderer.SetPosition(m_numVerticesAdded, cursorPos);
            }
        }
    }

    public void ToggleMeshVisibility(bool shouldShowMesh)
    {
        m_lineRenderer.enabled = shouldShowMesh;
        Utils.SetMaterialOnChildren(m_quadsHolder, shouldShowMesh ? m_materialToUseWhenVisible : m_materialToUseWhenHidden);

        m_isHidden = true;
    }

    // Helpers
    void InitializePartialMesh(Vector3 pos)
    {
        m_partialQuad = Instantiate(m_generatedQuadPrefab);
        m_partialMesh = m_partialQuad.GetComponent<MeshFilter>().mesh;
        m_partialMesh.colors = new Color[4] { m_quadColor, m_quadColor, m_quadColor, m_quadColor };
        m_partialMesh.triangles = new int[6] { 0, 1, 2, 0, 2, 3 };
        InitMeshVertices(pos);

        m_lineRenderer.SetPositions(new Vector3[4] { pos, pos, pos, pos });
    }


    void AddNextVertex(Vector3 cursorPos)
    {
        Vector3[] vertices = m_partialMesh.vertices;
        vertices[m_numVerticesAdded] = cursorPos;
        m_partialMesh.vertices = vertices;

        m_lineRenderer.SetPosition(m_numVerticesAdded, cursorPos);

        ++m_numVerticesAdded;
    }

    void InitMeshVertices(Vector3 pos)
    {
        Vector3[] vertices = m_partialMesh.vertices;
        for (int i = 0; i < vertices.GetLength(0); ++i)
        {
            vertices[i] = pos;
        }
        m_partialMesh.vertices = vertices;
    }
}
