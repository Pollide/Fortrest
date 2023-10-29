using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CampSpawner : MonoBehaviour
{
    public static CampSpawner global;

    [Header("Camp Spawning")]
    [HideInInspector]
    public int currentDay;
    private int lastAmount;
    private int campInt;

    private bool setTime = true;
    private bool spawnCamp = true;
    private bool onlyOnce = true;

    private float randomTime;
    private float edge = 30.0f;

    private Vector3 spawnPosition;

    public GameObject campPrefab;
    public MeshRenderer mesh;
    public GameObject campUI;
    public TMP_Text campText;
    [Space]
    [SerializeField] private int campsToSpawnPerDay = 2;
    [SerializeField] public int campsSpawnedPerDay = 0;
    [Space]
    public float goblinCampPercent = 2;
    public float snakegoblinCampPercent = 2;
    public float spidergoblinCampPercent = 2;
    public float wolfgoblinCampPercents = 2;
    public float lavagoblinCampPercent = 2;

    [Header("Enemy Spawning")]
    public int spawnMaxMarsh = 10;
    public int spawnMaxTussuck = 10;
    public int spawnMaxCoast = 8;
    public int spawnMaxTaiga = 5;
    public int spawnMaxVolcanic = 8;

    public int spawnCurrentMarsh = 0;
    public int spawnCurrentTussuck = 0;
    public int spawnCurrentCoast = 0;
    public int spawnCurrentTaiga = 0;
    public int spawnCurrentVolcanic = 0;

    [Space]
    public bool spawnEnemies = true;

    private void Start()
    {
        global = this;
        currentDay = LevelManager.global.day;
        lastAmount = 0;
    }

    public void SpawnEnemies(Terrain terrain, GameObject prefab, int amountMax, ref int amountCurrent)
    {
        LevelManager manager = LevelManager.global;

        if (!spawnEnemies)
        {
            while (amountCurrent < amountMax)
            {
                spawnPosition.x = terrain.transform.position.x + Random.Range(edge, terrain.terrainData.size.x - edge);

                spawnPosition.z = terrain.transform.position.z + Random.Range(edge, terrain.terrainData.size.z - edge);

                spawnPosition.y = (mesh.bounds.size.y / 2.0f) - 4;

                float distance = Vector3.Distance(PlayerController.global.transform.position, spawnPosition);

                if (distance > 40.0f)
                {
                    //Define the positions for the four raycasts
                    Vector3[] raycastPositions = new Vector3[4];
                    raycastPositions[0] = spawnPosition;
                    float size = 1;
                    raycastPositions[1] = spawnPosition + new Vector3(size, 0f, 0f); // Shift in positive X direction
                    raycastPositions[2] = spawnPosition - new Vector3(size, 0f, 0f); // Shift in negative X direction
                    raycastPositions[3] = spawnPosition + new Vector3(0f, 0f, size); // Shift in positive Z direction

                    //Loop through each position and cast a ray
                    bool isSafe = true; //Assuming it's safe until proven otherwise

                    foreach (Vector3 position in raycastPositions)
                    {
                        if (!Physics.Raycast(position, Vector3.down, Mathf.Infinity, LayerMask.GetMask("Terrain")))
                        {
                            isSafe = false;
                            break; //Exit the loop if any of the raycasts hit nothing
                        }
                    }

                    if (isSafe) //Raycast prevents camp spawning somewhere there isnt terrain has island has unique shape
                    {
                        Collider[] colliders = Physics.OverlapSphere(spawnPosition, mesh.bounds.size.x / 2.0f, GameManager.ReturnBitShift(new string[] { "Resource", "Building", "Boar" }));

                        for (int i = 0; i < colliders.Length; i++)
                        {
                            if (colliders[i].tag == "Shrine" || colliders[i].tag == "Boar" || colliders[i].tag == "Camp")
                            {
                                return;
                            }
                            else if (colliders[i].tag == "Resource")
                            {
                                colliders[i].gameObject.SetActive(false); //dont destroy resources just deactivate like dis thanks
                                                                          // Destroy(colliders[i].gameObject);
                            }
                        }

                        GameObject enemy = Instantiate(prefab, spawnPosition, Quaternion.identity);

                        EnemyController enemyController = enemy.GetComponent<EnemyController>();

                        enemyController.addIndicator = false;

                        amountCurrent++;

                        if (amountCurrent == amountMax)
                        {
                            spawnEnemies = true;
                        }
                    }
                }
            }
        }
    }

    private void SpawnCamps()
    {
        LevelManager manager = LevelManager.global;

        if (!spawnCamp && manager.goblinSpawnable && campsSpawnedPerDay < campsToSpawnPerDay)
        {
            Terrain terrain = LevelManager.global.terrainDataList[1].terrain;
            campInt = 1;

            if (manager.snakeSpawnable)
            {
                int rand = Random.Range(1, 3);
                campInt = rand;
                terrain = LevelManager.global.terrainDataList[rand].terrain;
            }

            if (manager.wolfSpawnable)
            {
                int rand = Random.Range(1, 4);
                campInt = rand;
                terrain = LevelManager.global.terrainDataList[rand].terrain;
            }

            if (manager.spiderSpawnable)
            {
                int rand = Random.Range(1, 5);
                campInt = rand;
                terrain = LevelManager.global.terrainDataList[rand].terrain;
            }

            if (manager.lavaSpawnable)
            {
                int rand = Random.Range(1, 6);
                campInt = rand;
                terrain = LevelManager.global.terrainDataList[rand].terrain;
            }

            spawnPosition.x = terrain.transform.position.x + Random.Range(edge, terrain.terrainData.size.x - edge);

            spawnPosition.z = terrain.transform.position.z + Random.Range(edge, terrain.terrainData.size.z - edge);

            spawnPosition.y = (mesh.bounds.size.y / 2.0f) - 4;

            float distance = Vector3.Distance(PlayerController.global.transform.position, spawnPosition);

            if (distance > 40.0f)
            {
                //Define the positions for the four raycasts
                Vector3[] raycastPositions = new Vector3[4];
                raycastPositions[0] = spawnPosition;
                float size = 5;
                raycastPositions[1] = spawnPosition + new Vector3(size, 0f, 0f); // Shift in positive X direction
                raycastPositions[2] = spawnPosition - new Vector3(size, 0f, 0f); // Shift in negative X direction
                raycastPositions[3] = spawnPosition + new Vector3(0f, 0f, size); // Shift in positive Z direction

                //Loop through each position and cast a ray
                bool isSafe = true; //Assuming it's safe until proven otherwise

                foreach (Vector3 position in raycastPositions)
                {
                    if (!Physics.Raycast(position, Vector3.down, Mathf.Infinity, LayerMask.GetMask("Terrain")))
                    {
                        isSafe = false;
                        break; //Exit the loop if any of the raycasts hit nothing
                    }
                }

                if (isSafe) //Raycast prevents camp spawning somewhere there isnt terrain has island has unique shape
                {
                    Collider[] colliders = Physics.OverlapSphere(spawnPosition, mesh.bounds.size.x / 2.0f, GameManager.ReturnBitShift(new string[] { "Resource", "Building", "Boar" }));

                    for (int i = 0; i < colliders.Length; i++)
                    {
                        if (colliders[i].tag == "Shrine" || colliders[i].tag == "Boar" || colliders[i].tag == "Camp")
                        {
                            return;
                        }
                        else if (colliders[i].tag == "Resource")
                        {
                            colliders[i].gameObject.SetActive(false); //dont destroy resources just deactivate like dis thanks
                                                                      // Destroy(colliders[i].gameObject);
                        }
                    }
                    GameObject camp = Instantiate(campPrefab, spawnPosition, Quaternion.identity);

                    camp.GetComponent<Camp>().campType = (Camp.CAMPTYPE)campInt;

                    campsSpawnedPerDay++;

                    if (campsSpawnedPerDay == campsToSpawnPerDay)
                    {
                        campsSpawnedPerDay = 0;
                        spawnCamp = true;
                    }
                }
            }
            else
            {
                return;
            }
        }
    }

    void Update()
    {
        if (lastAmount != LevelManager.global.campsCount)
        {
            GameManager.PlayAnimation(campText.GetComponent<Animation>(), "EnemyAmount");
            lastAmount = LevelManager.global.campsCount;
        }
        campText.text = LevelManager.global.campsCount.ToString();

        if (LevelManager.global.campsCount > 0 && !campUI.activeSelf)
        {
            GameManager.PlayAnimation(PlayerController.global.UIAnimation, "Camps Appear");
        }
        else if (LevelManager.global.campsCount == 0 && campUI.activeSelf)
        {
            GameManager.PlayAnimation(PlayerController.global.UIAnimation, "Camps Appear", false);
        }

        if (LevelManager.global.campsCount < 30)
        {
            if (currentDay != LevelManager.global.day)
            {
                currentDay = LevelManager.global.day;
                setTime = false;
            }

            if (!setTime)
            {
                randomTime = Random.Range(0.0f, 180.0f);
                setTime = true;
                onlyOnce = false;
            }

            if (LevelManager.global.daylightTimer > randomTime && !onlyOnce)
            {
                onlyOnce = true;
                spawnCamp = false;
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.M))
            {
                spawnCamp = false;
            }
#endif

            SpawnCamps();


        }
    }
}