using System;
using UnityEngine;
using UnityEngine.XR.iOS;
using System.Collections.Generic;
using System.IO;
using UnityEngine.EventSystems;

public class Utils
{
	static public bool WasTouchStartDetected()
	{
		return Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Began;
	}

	static public bool IsTouchOnUI()
	{
		return Input.touchCount == 1 && EventSystem.current.IsPointerOverGameObject (Input.GetTouch(0).fingerId);
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

	public static Vector3 GetWorldSpaceSize(GameObject go) {
		Vector3 size = Vector3.zero;

		var boxCollider = go.GetComponent<BoxCollider> ();
		if (boxCollider) {
			size.x = boxCollider.bounds.extents.x * 2;
			size.y = boxCollider.bounds.extents.y * 2;
			size.z = boxCollider.bounds.extents.z * 2;
		} else {
			Debug.LogError ("GetWorldSpaceSize: GameObject doesnt have a Box Collider set.");
		}

		return size;
	}

	public static void SavePointCloudToPlyFile(List<Vector3> pointCloud, List<Color> pointColors, string fileName) {
		if (pointCloud.Count != pointColors.Count) {
			Debug.LogError ("SavePointCloudToPlyFile: Invalid input.");
			return;
		}

		string path = Application.persistentDataPath + "/" + fileName;
		using (StreamWriter fileWriter = File.CreateText (path)) {
			// Format description @ http://paulbourke.net/dataformats/ply/

			// Ply Header
			fileWriter.WriteLine ("ply");
			fileWriter.WriteLine ("format ascii 1.0");
			fileWriter.WriteLine ("element vertex {0}", pointCloud.Count);
			fileWriter.WriteLine ("property float32 x");
			fileWriter.WriteLine ("property float32 y");
			fileWriter.WriteLine ("property float32 z");
			fileWriter.WriteLine ("property uchar red");
			fileWriter.WriteLine ("property uchar green");
			fileWriter.WriteLine ("property uchar blue");
			fileWriter.WriteLine ("end_header");

			// point & colors
			for (int i=0; i<pointCloud.Count; ++i) {
				var point = pointCloud [i];
				var color = pointColors [i];
				fileWriter.WriteLine ("{0} {1} {2} {3} {4} {5}", 
					point.x, point.y, point.z,
					(byte)(color.r * 255.0f), (byte)(color.g * 255.0f), (byte)(color.b * 255.0f)
				);
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

	public static void SetMaterialOnChildren (Transform parent, Material material)
	{
		var childMeshRenderers = parent.GetComponentsInChildren<MeshRenderer> ();
		foreach (var child in childMeshRenderers) {
			child.material = material;
		}
	}

	public static void SetChildrenAsKinematic (GameObject parent, bool shouldEnableKinematic) {
		var childRigidBodies = parent.GetComponentsInChildren<Rigidbody> ();
		foreach (var child in childRigidBodies) {
			child.isKinematic = shouldEnableKinematic;
		}
	}
}
