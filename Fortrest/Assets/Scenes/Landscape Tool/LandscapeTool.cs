using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LandscapeTool : MonoBehaviour
{
    public Texture2D terrainTexture;
    public float textureThreshold = 0.5f;

    private Terrain terrain;
    private TerrainData terrainData;
    private float[,] heightMap;
    private int textureWidth;
    private int textureHeight;
    private float terrainWidth;
    private float terrainLength;
    private float yStepSize;
    private float xStepSize;
    public bool UpdateTerrainBool;

    private void Start()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
        heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);

        textureWidth = terrainTexture.width;
        textureHeight = terrainTexture.height;
        terrainWidth = terrainData.size.x;
        terrainLength = terrainData.size.z;
        yStepSize = terrainLength / (float)textureHeight;
        xStepSize = terrainWidth / (float)textureWidth;
    }

    private void Update()
    {
        if (UpdateTerrainBool)
        {
            UpdateTerrainBool = false;
            // ApplyTextureBasedOnYAxis();
            Debug.Log("TERRAIN UPDATED");
        }
    }

    private void ApplyTextureBasedOnYAxis()
    {
        float[,,] splatmapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapResolution, terrainData.alphamapResolution);

        int splatmapWidth = splatmapData.GetLength(1);
        int splatmapHeight = splatmapData.GetLength(0);
        Debug.Log(textureHeight);
        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                float worldPosX = x * xStepSize;
                float worldPosZ = y * yStepSize;

                int heightMapX = (int)((worldPosX / terrainWidth) * terrainData.heightmapResolution);
                int heightMapY = (int)((worldPosZ / terrainLength) * terrainData.heightmapResolution);

                // Check if the indices are within the valid range
                Debug.Log(((heightMapX >= 0) + " && " + (heightMapX < splatmapWidth) + " && " + (heightMapY >= 0) + " && " + (heightMapY < splatmapHeight)));

                if (heightMapX >= 0 && heightMapX < splatmapWidth && heightMapY >= 0 && heightMapY < splatmapHeight)
                {
                    float terrainHeight = heightMap[heightMapY, heightMapX];
                    Debug.Log(terrain + " < " + textureThreshold);

                    if (terrainHeight < textureThreshold)
                    {
                        float[] splatWeights = new float[terrainData.alphamapLayers];
                        for (int i = 0; i < splatWeights.Length; i++)
                        {
                            if (i == 0)
                                splatWeights[i] = 1f;
                            else
                                splatWeights[i] = 0f;
                        }

                        for (int layerIndex = 0; layerIndex < terrainData.alphamapLayers; layerIndex++)
                        {
                            // Set the splat weights only if the layer index is within the valid range
                            if (layerIndex >= 0 && layerIndex < splatmapData.GetLength(2))
                            {
                                Debug.Log(1);
                                splatmapData[heightMapY, heightMapX, layerIndex] = splatWeights[layerIndex];
                            }
                        }
                    }
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, splatmapData);
        terrain.Flush();
    }
}
