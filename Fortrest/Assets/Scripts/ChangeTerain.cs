using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainTest))]
public class ChangeTerain : Editor
{
    private int selectedTextureIndex = 0;
    private float changeRadius = 5f;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        selectedTextureIndex = EditorGUILayout.IntSlider("Selected Texture Index", selectedTextureIndex, 0, Terrain.activeTerrain.terrainData.terrainLayers.Length - 1);
        changeRadius = EditorGUILayout.FloatField("Change Radius", changeRadius);

        if (GUILayout.Button("Change Terrain Texture"))
        {
            // Access the terrain object
            Terrain terrain = FindObjectOfType<Terrain>();

            // Change the texture within the specified radius of the object's position
            Vector3 objectPosition = ((TerrainTest)target).transform.position;
            ChangeTerrainTexture(terrain, selectedTextureIndex, objectPosition, changeRadius);
        }
    }

    // Change the terrain texture within a given radius of a position
    private void ChangeTerrainTexture(Terrain terrain, int textureIndex, Vector3 position, float radius)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainLocalPos = position - terrain.transform.position;
        Vector2 normalizedPos = new Vector2(terrainLocalPos.x / terrainData.size.x, terrainLocalPos.z / terrainData.size.z);
        float[,,] alphamaps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

        int startSampleX = Mathf.FloorToInt((normalizedPos.x - radius / terrainData.size.x) * terrainData.alphamapWidth);
        int startSampleY = Mathf.FloorToInt((normalizedPos.y - radius / terrainData.size.z) * terrainData.alphamapHeight);
        int endSampleX = Mathf.CeilToInt((normalizedPos.x + radius / terrainData.size.x) * terrainData.alphamapWidth);
        int endSampleY = Mathf.CeilToInt((normalizedPos.y + radius / terrainData.size.z) * terrainData.alphamapHeight);

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
                float distance = Vector2.Distance(normalizedPos, samplePosNormalized);

                if (distance <= radius / terrainData.size.x)
                {
                    for (int i = 0; i < terrainData.terrainLayers.Length; i++)
                    {
                        alphamaps[y, x, i] = (i == textureIndex) ? 1f : 0f;
                    }
                }
            }
        }

        // Apply the changes to the terrain
        terrainData.SetAlphamaps(0, 0, alphamaps);
    }
}
