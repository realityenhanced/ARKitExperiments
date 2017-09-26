using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeoLocationController : MonoBehaviour {

    // Script inputs
    public CursorManager m_cursorManager;
    public GameObject m_objectPrefab;

    // Privates
    private Transform m_objectTransform;
    
    // Update is called once per frame
    void Update()
    {
        if (Utils.WasTouchStartDetected())
        {
            if (m_objectTransform == null)
            {
                // On the first tap, instantiate the object at the cursor location.
                m_objectTransform = GameObject.Instantiate(m_objectPrefab, m_cursorManager.GetCurrentCursorPosition(), Quaternion.identity).transform;
            }
            else
            {
                // On subsequent taps, move the object to the new location.
                m_objectTransform.position = m_cursorManager.GetCurrentCursorPosition();
            }
        }
    }
}
