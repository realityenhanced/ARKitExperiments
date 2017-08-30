using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpencvExperimentController : MonoBehaviour {

        OpencvProcessingInterface m_opencvProcessing;

	// Use this for initialization
	void Start () {
		m_opencvProcessing = new OpencvProcessingInterface();
		Debug.Log(m_opencvProcessing.PerformProcessing());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
