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
    public float minDistance = 0;
    public Vector2 positionOnTerrain;

    public Vector3 CalculatePosition()
    {
        Terrain terrain = Terrain.activeTerrain;
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
        Terrain terrain = Terrain.activeTerrain;
        Transform resourceHolderTransform = GameObject.FindGameObjectWithTag("SceneObjects").transform;
        int stackOverflow = 1000;

        float halfWidth = resourcePrefab.GetComponent<SphereCollider>().radius * resourcePrefab.transform.localScale.x;


        for (int i = 0; i < terrain.terrainData.terrainLayers.Length; i++)
        {
            Debug.Log("i: " + terrain.terrainData.terrainLayers[i]);
        }

        for (int i = 0; i < numberOfResources; i++)
        {
            Vector3 randomRange = new Vector3(Random.Range((-rangeWidth / 2) + halfWidth, (rangeWidth / 2) - halfWidth), 0f, Random.Range((-rangeHeight / 2) + halfWidth, (rangeHeight / 2) - halfWidth));
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

                TerrainVoid(hit);
            }
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        return true;
    }

    public void TerrainVoid(RaycastHit hit)
    {
        Terrain terrain = hit.transform.GetComponent<Terrain>();

        if (terrain)
        {
            //   Vector3 terrainPosition = hit.point - terrain.transform.position;
            //Vector3 mapPosition = new Vector3
            //  (terrainPosition.x / terrain.terrainData.size.x, 0,
            //  terrainPosition.z / terrain.terrainData.size.z);
            //float xCoord = mapPosition.x * terrain.terrainData.alphamapWidth;
            //float zCoord = mapPosition.z * terrain.terrainData.alphamapHeight;

            string materialNameString = ReturnTerrainTexture(terrain, hit.point).name;

            Debug.Log(materialNameString);
        }
    }


    static Vector3 ConvertToSplatMapCoordinate(Vector3 hitPositionVector)
    {
        Vector3 vecRet = new Vector3();
        Terrain ter = Terrain.activeTerrain;
        Vector3 terPosition = ter.transform.position;
        vecRet.x = ((hitPositionVector.x - terPosition.x) / ter.terrainData.size.x) * ter.terrainData.alphamapWidth;
        vecRet.z = ((hitPositionVector.z - terPosition.z) / ter.terrainData.size.z) * ter.terrainData.alphamapHeight;
        return vecRet;
    }

    public float[,,] CachedSplatmapData;
    Texture2D ReturnTerrainTexture(Terrain terrain, Vector3 hitPositionVector)
    {
        TerrainData mTerrainData = terrain.terrainData;

        if (CachedSplatmapData == default)
        {
            //GetAlphamaps is very high on performance so cache it
            CachedSplatmapData = mTerrainData.GetAlphamaps(0, 0, mTerrainData.alphamapWidth, mTerrainData.alphamapHeight);
        }

        int mNumTextures = CachedSplatmapData.Length / (mTerrainData.alphamapWidth * mTerrainData.alphamapHeight);

        Vector3 TerrainCord = ConvertToSplatMapCoordinate(hitPositionVector);
        int ret = 0;
        float comp = 0f;
        for (int i = 0; i < mNumTextures; i++)
        {
            if (comp < CachedSplatmapData[(int)TerrainCord.z, (int)TerrainCord.x, i])
            {
                ret = i;
            }
        }

        return mTerrainData.terrainLayers[ret].diffuseTexture;
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

    private bool CheckMinDistance(Vector3 position)
    {
        GameObject[] spawnedObjects = GameObject.FindGameObjectsWithTag("Resource");

        foreach (GameObject spawnedObject in spawnedObjects)
        {
            float distance = Vector3.Distance(position, spawnedObject.transform.position);
            if (distance < (spawnedObject.GetComponent<SphereCollider>().radius * resourcePrefab.transform.localScale.x) * 1.9 + minDistance)
            {
                return false; // Distance is too close to an existing object
            }
        }

        return true; // Minimum distance check passed
    }
}
#endif