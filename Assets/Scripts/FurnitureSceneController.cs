using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureSceneController : SceneController {

	// Script inputs
	public CursorManager m_cursorManager;
	public GameObject m_actorPrefab;

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
                    m_furnitureTransform.position = new Vector3(cursorPos.x, m_groundPos.y, cursorPos.z);
                }
                break;
            case FurnitureState.Placed:
                break;
            default:
                break;
        }
	}
}
