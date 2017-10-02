using UnityEngine;
using UnityEngine.UI;

public class ManualScannerController : MonoBehaviour {
    // Script inputs
    public GameObject m_menuButton;

    // Use this for initialization
    void Start () {
        m_menuButton.GetComponentInChildren<Text>().text = "Save Mesh";
    }

    public void OnSaveMeshClicked()
    {
        GetComponent<QuadMeshBuilder>().SaveMeshToFile("RoomMesh");
    }
}
