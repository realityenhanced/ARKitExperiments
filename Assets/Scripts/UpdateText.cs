using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateText : MonoBehaviour {

	public void SetText(string text)
    {
        GetComponentInChildren<TextMesh>().text = text;
    }
}
