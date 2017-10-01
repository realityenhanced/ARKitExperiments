using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeoLocationController : MonoBehaviour {

    // Script inputs
    public CursorManager m_cursorManager;
    public GameObject m_objectPrefab;
    public Transform m_cameraTransform;

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

            StopAllCoroutines();
            StartCoroutine(PointObjectToGeographicNorthPole(m_objectTransform, m_cameraTransform));
        }
    }

    // Helpers
    private IEnumerator PointObjectToGeographicNorthPole(Transform objectTransform, Transform cameraTransform)
    {
        // On ios, the location framework prompts the user for permission.
        Input.location.Start();

        // Wait until service initializes.
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location");
            yield break;
        }
        else
        {
            string latLonText = "Lat: " + Input.location.lastData.latitude + "\nLon: " + Input.location.lastData.longitude + "\nAlt: " + Input.location.lastData.altitude;
            m_objectTransform.gameObject.GetComponent<UpdateText>().SetText(latLonText);

        }

        Input.location.Stop();
    }
}
