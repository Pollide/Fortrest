using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GenerateList : MonoBehaviour
{
    public GameObject resourcePrefab; // The prefab of the resource objects
    public int numberOfResources; // The number of resources to generate
    public float rangeWidth = 100;
    public float rangeHeight = 100;
    public List<GameObject> resourceList = new(); // The list to hold the generated resources

    public void GenerateResources()
    {
#if UNITY_EDITOR
        // Clear the existing list
        ClearResourceList();

        Terrain terrain = FindObjectOfType<Terrain>();

        for (int i = 0; i < numberOfResources; i++)
        {
           

            Vector3 randomPosition = new Vector3(Random.Range(0f, terrain.terrainData.size.x), 0f, Random.Range(0f, terrain.terrainData.size.z));
            Vector3 raycastOrigin = new Vector3(randomPosition.x, terrain.terrainData.size.y, randomPosition.z);

            RaycastHit hit;
            if (Physics.Raycast(raycastOrigin, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
            {
                Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                GameObject resource = PrefabUtility.InstantiatePrefab(resourcePrefab) as GameObject;
                resource.transform.position = hit.point;
                resource.transform.rotation = rotation;
                resourceList.Add(resource);
            }
        }
#endif
    }

    void ClearResourceList()
    {
        if (resourceList != null)
        {
            // Destroy all game objects in the list
            foreach (GameObject resource in resourceList)
            {
                DestroyImmediate(resource);
            }

            // Clear the list
            resourceList.Clear();
        }
    }
}
