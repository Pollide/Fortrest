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
    public Vector2 positionOnTerrain;

    public Vector3 CalculatePosition()
    {
        Terrain terrain = GameObject.FindObjectOfType<Terrain>();
        Vector3 modifiedTerrainDetails = new Vector3((terrain.GetPosition().x + (terrain.terrainData.size.x / 2)) + positionOnTerrain.x, terrain.GetPosition().y, (terrain.GetPosition().z + (terrain.terrainData.size.z / 2)) + positionOnTerrain.y);
        Vector3 raycastOrigin = new Vector3(modifiedTerrainDetails.x, terrain.terrainData.size.y, modifiedTerrainDetails.z);
        if (Physics.Raycast(raycastOrigin, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
        {
            modifiedTerrainDetails = new Vector3(modifiedTerrainDetails.x, hit.point.y, modifiedTerrainDetails.z);
        }

        return modifiedTerrainDetails;
    }

    public bool GenerateResources()
    {
        Terrain terrain = GameObject.FindObjectOfType<Terrain>();
        Transform resourceHolderTransform = GameObject.FindGameObjectWithTag("SceneObjects").transform;

        int stackOverflow = 100;

        for (int i = 0; i < numberOfResources; i++)
        {
            Vector3 randomRange = new Vector3(Random.Range(-rangeWidth / 2, rangeWidth / 2), 0f, Random.Range(-rangeHeight / 2, rangeHeight / 2));
            Vector3 randomPosition = CalculatePosition() + randomRange;

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

                if (!CheckMinDistance(randomPosition))
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

     //unity uses bit shifting to properly detect what layers should be masked, if the array is null, it will only see the "Default" layer.
    public static int ReturnBitShift(string[] layerNameArray = null)
    {
        //array
        if (layerNameArray == null)
        {
            layerNameArray = new string[] { "Default" };
        }

        int layerMaskInt = 0;

        for (int i = 0; i < layerNameArray.Length; i++)
        {
            //bit shifting layers
            layerMaskInt = layerMaskInt | 1 << LayerMask.NameToLayer(layerNameArray[i]);
        }

        return layerMaskInt;
    }

    private bool CheckCollision(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, resourcePrefab.GetComponent<Collider>().bounds.size.magnitude);

        foreach (Collider collider in colliders)
        {
            // Check if the collider belongs to the desired resource(s)
            if (collider.CompareTag("Resource") || collider.gameObject.layer == LayerMask.NameToLayer("Resource"))
            {
                Debug.Log("Hit");
                return true; // Collision detected with a resource
            }
        }

        return false; // No collision with resources detected
    }

    private bool CheckMinDistance(Vector3 position)
    {
        GameObject[] spawnedObjects = GameObject.FindGameObjectsWithTag("Resource"); // Change the tag if necessary

        foreach (GameObject spawnedObject in spawnedObjects)
        {
            float distance = Vector3.Distance(position, spawnedObject.transform.position);

            if (distance < spawnedObject.GetComponent<SphereCollider>().bounds.size.magnitude / 2)
            {
                return false; // Distance is too close to an existing object
            }
        }

        return true; // Minimum distance check passed
    }
}

#endif