using System;
using UnityEngine;
using UnityEngine.XR.iOS;
using System.Collections.Generic;
using System.IO;

public class Utils
{
	static public bool WasTouchStartDetected()
	{
		return Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Began;
	}

	static public bool WasTouchStopDetected()
	{
		return Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Ended;
	}

	static public GameObject SpawnGameObjectAt(GameObject prefab, Vector3 pos, Quaternion rot)
	{
		var actor = GameObject.Instantiate (prefab);	
		actor.transform.position = pos;
		actor.transform.rotation = rot;

		return actor;
	}

	static public ARHitTestResult GetFirstValidHit(List<ARHitTestResult> hitResults)
	{
		ARHitTestResult hitResult = hitResults[0]; // Return the first hit, if no valid hits were found.
		foreach (var h in hitResults) {
			if (h.isValid) {
				hitResult = h;
				break;
			}
		}
		return hitResult;
	}

	// Find the first instance of a material on the renderers associated with the game object
	public static Material FindMaterialOnObject(GameObject go, string materialName)
	{
		Material materialInstance = null;

		// Check if this is a simple mesh first
		var meshRenderers = go.GetComponentsInChildren<MeshRenderer>();
		if (meshRenderers.Length == 0)
		{
			// Check if it is a Skinned Mesh renderer
			var skinnedRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>();
			for (int i = 0; i < skinnedRenderers.Length; ++i)
			{
				materialInstance = Utils.FindMaterialOnRenderer(skinnedRenderers[i], materialName);
				if (materialInstance)
				{
					break;
				}
			}
		}
		else
		{
			for (int i = 0; i < meshRenderers.Length; ++i)
			{
				materialInstance = Utils.FindMaterialOnRenderer(meshRenderers[i], materialName);
				if (materialInstance)
				{
					break;
				}
			}
		}

		return materialInstance;
	}

	// Find the first instance of a material on a renderer 
	public static Material FindMaterialOnRenderer(Renderer renderer, string materialName)
	{
		Material materialInstance = null;
		for (int i = 0; i < renderer.materials.Length; ++i)
		{
			if (renderer.materials[i].name.StartsWith(materialName))
			{
				materialInstance = renderer.materials[i];
				break;
			}
		}

		return materialInstance;
	}

	public static float GetWorldSpaceHeight(GameObject go) {
		float height = 0.0f;

		var boxCollider = go.GetComponent<BoxCollider> ();
		if (boxCollider) {
			height = boxCollider.bounds.extents.y * 2;
		} else {
			Debug.LogError ("GetWorldSpaceHeight: GameObject doesnt have a Box Collider set.");
		}

		return height;
	}

	public static void SavePointCloudToPlyFile(List<Vector3> pointCloud, string fileName) {
		string path = Application.persistentDataPath + "/" + fileName;
		using (StreamWriter fileWriter = File.CreateText (path)) {
			// Format : https://people.sc.fsu.edu/~jburkardt/data/ply/ply.html

			// Ply Header
			fileWriter.WriteLine ("ply");
			fileWriter.WriteLine ("format ascii 1.0");
			fileWriter.WriteLine ("element vertex {0}", pointCloud.Count);
			fileWriter.WriteLine ("property float32 x");
			fileWriter.WriteLine ("property float32 y");
			fileWriter.WriteLine ("property float32 z");
			fileWriter.WriteLine ("end_header");

			// points
			foreach (var point in pointCloud) {
				fileWriter.WriteLine ("{0} {1} {2}", point.x, point.y, point.z);
			}
		}
	}

	public static void DeleteAllAppFiles() {
		DirectoryInfo di = new DirectoryInfo (Application.persistentDataPath);
		foreach (FileInfo file in di.GetFiles())
		{
			file.Delete(); 
		}
	}
}
