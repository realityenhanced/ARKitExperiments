using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureSceneController : SceneController {

	// Script inputs
	public CursorManager m_cursorManager;
	public GameObject m_actorPrefab;
    public float m_minimumThreshold = 0.001f;

    // Privates
    private Transform m_furnitureTransform;

    enum FurnitureState
    {
        NotPlaced,
        PlacementInProgress,
        Placed
    };
    private FurnitureState m_furnitureState = FurnitureState.NotPlaced;

    private Vector3 m_groundPos;

	// Update is called once per frame
	void Update() {
        switch(m_furnitureState)
        {
            case FurnitureState.NotPlaced:
                if (Utils.WasTouchStartDetected())
                {
                    // On the first tap, instantiate the furniture at the cursor location
                    // and move the furniture along the ground (with the cursor) till the second tap is detected.
                    m_groundPos = m_cursorManager.GetCurrentCursorPosition();
                    m_furnitureTransform = GameObject.Instantiate(m_actorPrefab, m_groundPos, Quaternion.identity).transform;

                    // Once the ground plane is established, move the cursor manager to raycast mode. (The World Hit Test mode is too noisy in low light conditions)
                    // NOTE: The furniture prefab has a very large shadow plane that has raycast enabled on it. (The furniture doesn't participate in raycasts)
                    m_cursorManager.SetMode(false);

                    m_furnitureState = FurnitureState.PlacementInProgress;
                }
                break;
            case FurnitureState.PlacementInProgress:
                if (Utils.WasTouchStartDetected())
                {
                    // On the second tap, ground the furniture.
                    m_furnitureState = FurnitureState.Placed;
                }
                else
                {
                    // Move the furniture along with the cursor without modifying the Y position.
                    var cursorPos = m_cursorManager.GetCurrentCursorPosition();
                    if ((cursorPos - m_furnitureTransform.position).magnitude > m_minimumThreshold)
                    {
                        m_furnitureTransform.position = new Vector3(cursorPos.x, m_groundPos.y, cursorPos.z);
                    }
                }
                break;
            case FurnitureState.Placed:
                break;
            default:
                break;
        }
	}
}
