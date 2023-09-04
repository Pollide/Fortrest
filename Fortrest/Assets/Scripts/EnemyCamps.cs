using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCamps : MonoBehaviour
{
    private int currentDay;
    private bool spawnCamp;
    private float randomTime;
    private bool onlyOnce;
    public GameObject campPrefab;
    Vector3 spawnPosition;
    public Mesh mesh;

    private void Start()
    {
        currentDay = LevelManager.global.day;
        mesh = GetComponent<MeshFilter>().sharedMesh;
    }

    void Update()
    {
        if (currentDay != LevelManager.global.day)
        {
            onlyOnce = false;
            currentDay = LevelManager.global.day;
            spawnCamp = true;
        }

        if (spawnCamp)
        {
            randomTime = Random.Range(0.0f, 180.0f);
            spawnPosition = LevelManager.global.terrainList[0].transform.position + new Vector3(Random.Range(80, LevelManager.global.terrainList[0].terrainData.size.x), 0, Random.Range(80, LevelManager.global.terrainList[0].terrainData.size.z));
            spawnCamp = false;
        }

        Collider[] colliders = Physics.OverlapSphere(spawnPosition, mesh.bounds.size.x / 2.0f, GameManager.ReturnBitShift(new string[] { "Building", "Resource" }));

        if (LevelManager.global.daylightTimer > randomTime && LevelManager.global.daylightTimer < randomTime + 1.0f && !onlyOnce)
        {
            GameObject camp = Instantiate(campPrefab, spawnPosition, Quaternion.identity);
            onlyOnce = true;
        }
    }
}