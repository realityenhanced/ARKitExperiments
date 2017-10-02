using UnityEngine;
using System.IO;

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

    public void SaveMeshToFile(string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName + ".ply";
        using (StreamWriter fileWriter = File.CreateText(path))
        {
            // Format description @ http://paulbourke.net/dataformats/ply/

            var numQuads = m_quadsHolder.childCount;

            // Ply Header
            fileWriter.WriteLine("ply");
            fileWriter.WriteLine("format ascii 1.0");
            fileWriter.WriteLine("element vertex {0}", numQuads * 4);
            fileWriter.WriteLine("property float32 x");
            fileWriter.WriteLine("property float32 y");
            fileWriter.WriteLine("property float32 z");
            fileWriter.WriteLine("element face {0}", numQuads);
            fileWriter.WriteLine("property list uchar int vertex_index");
            fileWriter.WriteLine("end_header");

            // Write vertices
            for (int i = 0; i < numQuads; ++i)
            {
                var child = m_quadsHolder.GetChild(i);
                var mesh = child.GetComponent<MeshFilter>().mesh;
                var vertices = mesh.vertices;

                for (int j = 0; j < vertices.Length; ++j)
                {
                    var point = vertices[j];
                    fileWriter.WriteLine("{0} {1} {2}", point.x, point.y, point.z);
                }
            }

            // Write face indices
            var currentPos = 0;
            for (int i = 0; i < numQuads; ++i)
            {
                var child = m_quadsHolder.GetChild(i);
                var mesh = child.GetComponent<MeshFilter>().mesh;
                
                // Index count = 4 (per QUad), followed by indices of vertices.
                fileWriter.WriteLine("4 {0} {1} {2} {3} ", currentPos, currentPos + 1, currentPos + 2, currentPos + 3);
                currentPos += 4;
            }
        }
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
