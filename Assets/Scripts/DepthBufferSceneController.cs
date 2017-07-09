using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthBufferSceneController : SceneController {

    // Script inputs
    public CursorManager m_cursorManager;
    public GameObject m_actorPrefab;
	public Camera m_cameraToLookAt;

    // Privates
    Transform m_actorTransform;

    void Update () {
        if (Utils.WasTouchDetected())
        {
            // Place the actor & it's shadow plane at the last detected cursor postion.
            var pos = m_cursorManager.GetCurrentCursorPosition();
            var rot = m_cursorManager.GetCurrentCursorRotation();

            if (m_actorTransform == null)
            {
                m_actorTransform = Utils.SpawnGameObjectAt(m_actorPrefab, pos, rot).transform;
            }
            else
            {
                m_actorTransform.position = pos;
                m_actorTransform.rotation = rot;
            }

			// Face the camera
			m_actorTransform.LookAt(m_cameraToLookAt.transform);
        }
    }
}
