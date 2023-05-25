using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GenerateList
{
    public GameObject resourcePrefab; // The prefab of the resource objects
    public int numberOfResources; // The number of resources to generate
    public float rangeWidth = 100;
    public float rangeHeight = 100;

    public void GenerateResources()
    {
#if UNITY_EDITOR
        // Clear the existing list

        Terrain terrain = GameObject.FindObjectOfType<Terrain>();
        Transform resourceHolderTransform = GameObject.FindGameObjectWithTag("SceneObjects").transform;

        for (int i = 0; i < numberOfResources; i++)
        {
            Vector3 randomPosition = terrain.transform.position + new Vector3(Random.Range(0f, terrain.terrainData.size.x), 0f, Random.Range(0f, terrain.terrainData.size.z));
            Vector3 raycastOrigin = new Vector3(randomPosition.x, terrain.terrainData.size.y, randomPosition.z);

            if (Physics.Raycast(raycastOrigin, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
            {
                if (hit.point.y < 0)
                {
                    //prevents objects spawning in water
                    i--;
                    continue;
                }
                GameObject resource = PrefabUtility.InstantiatePrefab(resourcePrefab) as GameObject;

                resource.transform.position = hit.point;

                resource.transform.SetParent(resourceHolderTransform);
            }
        }
#endif
    }

    public void ClearResourceList()
    {
        GameObject[] resources = GameObject.FindGameObjectsWithTag("Resource");

        // Destroy all game objects in the list
        foreach (GameObject resource in resources)
        {
            if (resource.name.Replace("(Clone)", "") == resourcePrefab.name)
                GameObject.DestroyImmediate(resource);
        }
    }
}
