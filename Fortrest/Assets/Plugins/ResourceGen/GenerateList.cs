using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class GenerateList
{

    public GameObject resourcePrefab; // The prefab of the resource objects
    public int numberOfResources = 10; // The number of resources to generate
    public float rangeWidth = 100; //the range a resource will spawn to
    public float rangeHeight = 100; //the range a resource will spawn to
    public float minY = 0; //minimum Y axis spawn
    public float maxY = 20;  //maximum Y axis spawn
    public float minDistance = 0; //minimum distance between object
    public Vector2 positionOnTerrain; //position on the terrain the resources generate at
    public List<Texture> SelectTexturesList = new List<Texture>(); //the textures selected that the resource can only generate one, if its empty it can just generate anywhere
    public float AreaOfDenialRadius = 2; //the radius which the area of denial makes
    public int TerrainTextureDenial = 1; // the index of the terrain layer that will be created on the terrain beneath the resource

    //saves the splatmap (alpha map) so doesnt rerun multiple times
    public float[,,] CachedSplatmapData;

    ///<summary>
    ///returns the new position of the terrain
    ///</summary>
    public Vector3 CalculatePosition()
    {
        Terrain terrain = Terrain.activeTerrain;
        Vector3 modifiedTerrainDetails = new Vector3((terrain.GetPosition().x + (terrain.terrainData.size.x / 2)) + positionOnTerrain.x, terrain.GetPosition().y, (terrain.GetPosition().z + (terrain.terrainData.size.z / 2)) + positionOnTerrain.y);
        Vector3 raycastOrigin = new Vector3(modifiedTerrainDetails.x, terrain.terrainData.size.y, modifiedTerrainDetails.z);

        //raycast down to find the center point
        if (Physics.Raycast(raycastOrigin, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
        {
            modifiedTerrainDetails = new Vector3(modifiedTerrainDetails.x, hit.point.y, modifiedTerrainDetails.z);
        }

        return modifiedTerrainDetails;
    }

    ///<summary>
    ///The main function for generating resources onto the terrain
    ///</summary>
    public bool GenerateResources()
    {
        Terrain terrain = Terrain.activeTerrain;

        //I did an extra 1000 to prevent the stack overflow, the number is mostly arbitrary just long as it doesnt crash unity
        int stackOverflow = numberOfResources + 1000;

        //get the radius
        float halfWidth = resourcePrefab.GetComponent<SphereCollider>().radius * resourcePrefab.transform.localScale.x;

        //runs the amount of resources given, however this loop can run much longer 
        for (int i = 0; i < numberOfResources; i++)
        {
            //randomly finds a spot in a 2D area that is inside the paramters
            Vector3 randomRange = new Vector3(Random.Range((-rangeWidth / 2) + halfWidth, (rangeWidth / 2) - halfWidth), 0f, Random.Range((-rangeHeight / 2) + halfWidth, (rangeHeight / 2) - halfWidth));

            //random position with given values
            Vector3 randomPosition = CalculatePosition() + randomRange;


            //where the raycast starts, it uses size.y so it only needs to go so high to reach the terrain limit
            Vector3 raycastOrigin = new Vector3(randomPosition.x, terrain.terrainData.size.y, randomPosition.z);


            //raycast from above and land down on the terrain, using a specific layermask so it isnt interfered with
            if (Physics.Raycast(raycastOrigin, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
            {
                //first it checks if the raycast hit is in between the y axis and it checks if there is no other resources around it and finally it checks if the texture hit was a texture selected on the terrain
                if (hit.point.y < minY || hit.point.y > maxY || !CheckMinDistance(randomPosition) || !ReturnOnTexture(hit))
                {
                    stackOverflow--;
                    if (stackOverflow <= 0) //this prevents the game running infinitely and causing a stack overflow when dealing with i--
                    {
                        // Debug.LogWarning("No terrain with parameters was found");
                        return false; //returns false as it failed to generate all the resources
                    }
                    i--;//the parameters are true which means this spot that was raycasted is not suitable for this resource, so it is not counted, so i-- just canels out the loops i++
                    continue;
                }

                //create the prefab from the selected game object
                GameObject resource = PrefabUtility.InstantiatePrefab(resourcePrefab) as GameObject;

                //position of where the raycast hit
                resource.transform.position = hit.point;
                Vector3 eulerVector = hit.normal; //so the resource is tanget to the surface

                //randomly rotate the resource
                eulerVector.y += Random.Range(0f, 360f);

                //convert euler to quaternion
                resource.transform.rotation = Quaternion.Euler(eulerVector);

                //for tidy reasons, set the resource as a child of the terrain
                resource.transform.SetParent(terrain.transform);

                //this is for area of denial, if index isnt -1 that means the user has chosen a texture
                if (TerrainTextureDenial != -1)
                    ChangeTerrainTexture(resource.transform.position); //then it sets the texture below the resource generated

                //makes the resource dirty so it can be saved
                PrefabUtility.RecordPrefabInstancePropertyModifications(resource);
            }
        }

        //makes the scene saveable of the changes applied
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        //success, all resources generated properly
        return true;
    }

    ///<summary>
    ///this is to prevent resources generating on terrain that is not selected
    ///</summary>
    public bool ReturnOnTexture(RaycastHit hit)
    {
        //returns the texture found at that point
        Texture2D texture = ReturnTerrainTexture(Terrain.activeTerrain, hit.point);

        //if none are selected, then it assumes it can spawn on any terrain, otherwise must contain the texture
        return SelectTexturesList.Count == 0 || SelectTexturesList.Contains(texture);
    }

    ///<summary>
    ///Change the texture at a coordinate
    ///</summary>
    public void ChangeTerrainTexture(Vector3 position)
    {
        TerrainData terrainData = Terrain.activeTerrain.terrainData;
        Vector3 terrainLocalPos = position - Terrain.activeTerrain.transform.position;
        Vector2 normalizedPos = new Vector2(terrainLocalPos.x / terrainData.size.x, terrainLocalPos.z / terrainData.size.z);

        //grab active map, cant do cache as this can be different
        float[,,] alphamaps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

        //floor will return the largest intger smaller or equal to the input. it then scales to find the correct positions
        int startSampleX = Mathf.FloorToInt((normalizedPos.x - AreaOfDenialRadius / terrainData.size.x) * terrainData.alphamapWidth);
        int startSampleY = Mathf.FloorToInt((normalizedPos.y - AreaOfDenialRadius / terrainData.size.z) * terrainData.alphamapHeight);
        int endSampleX = Mathf.CeilToInt((normalizedPos.x + AreaOfDenialRadius / terrainData.size.x) * terrainData.alphamapWidth);
        int endSampleY = Mathf.CeilToInt((normalizedPos.y + AreaOfDenialRadius / terrainData.size.z) * terrainData.alphamapHeight);

        //clamp the integer between the two values
        startSampleX = Mathf.Clamp(startSampleX, 0, terrainData.alphamapWidth - 1);
        startSampleY = Mathf.Clamp(startSampleY, 0, terrainData.alphamapHeight - 1);
        endSampleX = Mathf.Clamp(endSampleX, 0, terrainData.alphamapWidth - 1);
        endSampleY = Mathf.Clamp(endSampleY, 0, terrainData.alphamapHeight - 1);

        // Set the weight of the texture to 1 within the specified radius
        for (int y = startSampleY; y <= endSampleY; y++)
        {
            for (int x = startSampleX; x <= endSampleX; x++)
            {
                float samplePosX = (float)x / terrainData.alphamapWidth;
                float samplePosY = (float)y / terrainData.alphamapHeight;
                Vector2 samplePosNormalized = new Vector2(samplePosX, samplePosY);

                //distance between the sampled and normilized
                float distance = Vector2.Distance(normalizedPos, samplePosNormalized);

                if (distance <= AreaOfDenialRadius / terrainData.size.x)
                {
                    for (int i = 0; i < terrainData.terrainLayers.Length; i++)
                    {
                        //set the splatmap terrain indentifier
                        alphamaps[y, x, i] = (i == TerrainTextureDenial) ? 1f : 0f;
                    }
                }
            }
        }

        // Apply the changes to the terrain
        terrainData.SetAlphamaps(0, 0, alphamaps);
    }

    ///<summary>
    ///aligns the hit point to the grid of the splatmap
    ///</summary>
    static Vector3 ConvertToSplatMapCoordinate(Vector3 hitPositionVector)
    {
        Vector3 vecRet = new Vector3();

        //active terrain is a static global reference
        Terrain ter = Terrain.activeTerrain;
        Vector3 terPosition = ter.transform.position;

        //converts to a relative position then scaling it to the size of the terrain
        vecRet.x = ((hitPositionVector.x - terPosition.x) / ter.terrainData.size.x) * ter.terrainData.alphamapWidth;
        vecRet.z = ((hitPositionVector.z - terPosition.z) / ter.terrainData.size.z) * ter.terrainData.alphamapHeight;

        //return the new position found
        return vecRet;
    }

    ///<summary>
    ///returns the texture of the terrain at the splatmap coordinates
    ///</summary>
    Texture2D ReturnTerrainTexture(Terrain terrain, Vector3 hitPositionVector)
    {
        TerrainData mTerrainData = terrain.terrainData;

        //GetAlphamaps is an expensive function to run so its best to cache it
        if (CachedSplatmapData == default)
        {
            CachedSplatmapData = mTerrainData.GetAlphamaps(0, 0, mTerrainData.alphamapWidth, mTerrainData.alphamapHeight);
        }

        //how many textures are there
        int mNumTextures = CachedSplatmapData.Length / (mTerrainData.alphamapWidth * mTerrainData.alphamapHeight);

        //converts the hit to a position the terrain can understand
        Vector3 TerrainCord = ConvertToSplatMapCoordinate(hitPositionVector);

        int ret = 0; //terrain layer index


        for (int i = 0; i < mNumTextures; i++)
        {
            //finds the texture via the coords
            if (0 < CachedSplatmapData[(int)TerrainCord.z, (int)TerrainCord.x, i])
            {
                ret = i; //found the texture
            }
        }

        return mTerrainData.terrainLayers.Length > ret ? mTerrainData.terrainLayers[ret].diffuseTexture : null;
    }

    ///<summary>
    ///Destroys all resources of that kind
    ///</summary>
    public void ClearResourceList()
    {     //search for all resources in the scene
        GameObject[] resources = GameObject.FindGameObjectsWithTag("Resource");

        // Destroy all game objects in the list
        foreach (GameObject resource in resources)
        {
            //because resources are instantiated, need to remove the name so it equals the original prefab name else it wont be able to find it
            if (resource.name.Replace("(Clone)", "") == resourcePrefab.name)
                GameObject.DestroyImmediate(resource); //immediate is good in unity editor as the normla destroy is a coroutine which is not supported when not at runtime
        }
    }

    ///<summary>
    ///Checks that object is far enough from all other resources
    ///</summary>
    private bool CheckMinDistance(Vector3 position)
    {
        //search for all resources in the scene
        GameObject[] spawnedObjects = GameObject.FindGameObjectsWithTag("Resource");

        //comparing for all resources found
        foreach (GameObject spawnedObject in spawnedObjects)
        {
            //return the distance between the objects
            float distance = Vector3.Distance(position, spawnedObject.transform.position);

            //if the object is too close, it is a fail and the resource cannot generate in the given position
            if (distance < (spawnedObject.GetComponent<SphereCollider>().radius * resourcePrefab.transform.localScale.x) * 1.9 + minDistance)
            {
                return false; // Distance is too close to an existing object
            }
        }

        return true; // Minimum distance check passed
    }
}
#endif //we dont want the script compiling into the build as it will throw errors that UnityEditor doesnt exist