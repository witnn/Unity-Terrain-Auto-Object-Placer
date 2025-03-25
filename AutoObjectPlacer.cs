// This script is made from witnn. Hopefully helps your projects :D
// For using create a empty GameObject and attach this component.

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AutoObjectPlacer : MonoBehaviour
{
	[Header("Auto Object Placing System")]
	public Terrain terrain;
	public GameObject objectPrefab;
	[Space]
	public int numberOfObjects = 1000;
	public bool isSpawnToNormal = false;
	[Header("Slope Limits")]
	public float minSlope = 0f;
	public float maxSlope = 30f;
	[Header("Height Limits")]
	public float minHeight = 0f;
	public float maxHeight = 1000f;
	[Header("Scale Variation Limits")]
	public float minScale = 0.8f;
	public float maxScale = 1.2f;


	// Object placement method
	public void PlaceObjects()
	{
		if (terrain == null || objectPrefab == null)
		{
			Debug.LogWarning("Terrain veya Object Prefab atanmadı!");
			return;
		}

		TerrainData terrainData = terrain.terrainData;
		Vector3 terrainSize = terrainData.size;

		for (int i = 0; i < numberOfObjects; i++)
		{
			float x = Random.Range(0, terrainSize.x);
			float z = Random.Range(0, terrainSize.z);
			float y = terrain.SampleHeight(new Vector3(x, 0, z)) + terrain.transform.position.y;

			if (y < minHeight || y > maxHeight)
			{
				continue;
			}

			Vector3 position = new Vector3(x, y, z);
			Vector3 normal = terrainData.GetInterpolatedNormal(x / terrainSize.x, z / terrainSize.z);
			float slope = Vector3.Angle(normal, Vector3.up);

			if (slope <= maxSlope && slope >= minSlope)
			{
				Quaternion rotation = isSpawnToNormal
					? Quaternion.FromToRotation(Vector3.up, normal)
					: Quaternion.Euler(0, Random.Range(0f, 360f), 0);

				GameObject obj = Instantiate(objectPrefab, position, rotation, transform);
				float randomScale = Random.Range(minScale, maxScale);
				obj.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
			}
		}
	}

	// Deleting objects method
	public void DeleteObjects()
	{
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			DestroyImmediate(transform.GetChild(i).gameObject);
		}
	}

// Editor Button
#if UNITY_EDITOR
	[CustomEditor(typeof(AutoObjectPlacer))]
	public class AutoObjectPlacerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			AutoObjectPlacer placer = (AutoObjectPlacer)target;

			if (GUILayout.Button("Place Objects"))
			{
				placer.PlaceObjects();
			}

			if (GUILayout.Button("Delete Objects"))
			{
				placer.DeleteObjects();
			}
		}
	}
#endif
}
