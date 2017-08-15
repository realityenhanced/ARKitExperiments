using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorCameraController : MonoBehaviour {

#if UNITY_EDITOR
    float m_currentRotationX = 0;
    float m_currentRotationY = 0;

    private void Start()
    {
        // Prevent Unity AR Camera manager from taking over in Editor mode.
        GetComponentInChildren<UnityARCameraManager>().gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += Vector3.right * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += Vector3.left * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += Vector3.forward * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += Vector3.back * Time.deltaTime;
        }
        else if (Input.GetMouseButton(0))
        {
            m_currentRotationX += Input.GetAxis("Mouse X") * 100 *  Time.deltaTime;
            m_currentRotationY += Input.GetAxis("Mouse Y") * 100 * Time.deltaTime;
            m_currentRotationY = Mathf.Clamp(m_currentRotationY, -360, 360);
            transform.localEulerAngles = new Vector3(-m_currentRotationY, m_currentRotationX, 0);
        }
    }
#endif
}
