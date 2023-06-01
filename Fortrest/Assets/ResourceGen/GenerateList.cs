using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class GenerateList
{
    public GameObject resourcePrefab; // The prefab of the resource objects
    public int numberOfResources = 10; // The number of resources to generate
    public float rangeWidth = 100;
    public float rangeHeight = 100;
    public float minY = 0;
    public float maxY = 20;


    public bool GenerateResources()
    {

        // Clear the existing list

        Terrain terrain = GameObject.FindObjectOfType<Terrain>();
        Transform resourceHolderTransform = GameObject.FindGameObjectWithTag("SceneObjects").transform;

        int stackOverflow = 100;

        for (int i = 0; i < numberOfResources; i++)
        {
            Vector3 randomPosition = terrain.transform.position + new Vector3(Random.Range(0f, terrain.terrainData.size.x), 0f, Random.Range(0f, terrain.terrainData.size.z));
            Vector3 raycastOrigin = new Vector3(randomPosition.x, terrain.terrainData.size.y, randomPosition.z);

            if (Physics.Raycast(raycastOrigin, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
            {
                if (hit.point.y < minY || hit.point.y > maxY)
                {
                    stackOverflow--;
                    if (stackOverflow <= 0)
                    {
                        // Debug.LogWarning("No terrain with parameters was found");
                        return false;
                    }
                    i--;
                    continue;
                }
                GameObject resource = PrefabUtility.InstantiatePrefab(resourcePrefab) as GameObject;

                resource.transform.position = hit.point;
                Vector3 eulerVector = hit.normal;
                eulerVector.y += Random.Range(0f, 360f);
                resource.transform.rotation = Quaternion.Euler(eulerVector);

                resource.transform.SetParent(resourceHolderTransform);

                PrefabUtility.RecordPrefabInstancePropertyModifications(resource);
            }
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        return true;
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

#endif