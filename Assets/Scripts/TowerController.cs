using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour {
	//Script inputs
	public GameObject m_towerBlockPrefab;
	public int m_numberOfBlocksInBase = 10;

	// Use this for initialization
	void Start () {
		if (m_numberOfBlocksInBase >= 1) {
			LayoutBlocks ();

			// Allow the physics system to start acting on the children.
			Utils.SetChildrenAsKinematic (this.gameObject, false);
		}
	}

	void LayoutBlocks ()
	{
		int currentRow = 0;
		Vector3 currentPos = Vector3.zero;

		var firstBlock = Instantiate (m_towerBlockPrefab, Vector3.zero, Quaternion.identity);
		firstBlock.transform.parent = this.transform;

		Vector3 blockSize = Utils.GetWorldSpaceSize (firstBlock);
		currentPos.y += blockSize.y / 2;
		firstBlock.transform.localPosition = currentPos;

		while ((m_numberOfBlocksInBase - currentRow) > 0) {
			for (int i = 0; i < (m_numberOfBlocksInBase - currentRow); ++i) {				
				if (currentRow != 0 || i != 0) {
					var go = Instantiate (m_towerBlockPrefab, Vector3.zero, Quaternion.identity);
					go.transform.parent = this.transform;
					go.transform.localPosition = currentPos;
				}
				currentPos.z += blockSize.z * 1.5f;
			}
			currentPos.y += blockSize.y;
			currentPos.z = (currentRow + 1) * (blockSize.z * 0.75f);
			++currentRow;
		}
	}
}
