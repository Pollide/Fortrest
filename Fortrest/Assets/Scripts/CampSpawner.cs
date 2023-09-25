using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CampSpawner : MonoBehaviour
{
    private int currentDay;
    private bool setTime = true;
    private bool spawnCamp = true;
    private float randomTime;
    private bool onlyOnce = true;
    public GameObject campPrefab;
    Vector3 spawnPosition;
    public MeshRenderer mesh;
    private float edge = 30.0f;
    private float buildZone = 85.0f;
    public GameObject campUI;
    public TMP_Text campText;
    int lastAmount;

    private void Start()
    {
        currentDay = LevelManager.global.day;
        lastAmount = 0;
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

            if (LevelManager.global.daylightTimer > randomTime && LevelManager.global.daylightTimer < randomTime + 1.0f && !onlyOnce)
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

            if (!spawnCamp)
            {
                spawnPosition.x = LevelManager.global.terrainList[0].transform.position.x + Random.Range(edge, LevelManager.global.terrainList[0].terrainData.size.x - edge);

                if (spawnPosition.x >= buildZone)
                {
                    spawnPosition.z = LevelManager.global.terrainList[0].transform.position.z + Random.Range(edge, LevelManager.global.terrainList[0].terrainData.size.z - edge);
                }
                else
                {
                    spawnPosition.z = LevelManager.global.terrainList[0].transform.position.z + Random.Range(buildZone, LevelManager.global.terrainList[0].terrainData.size.z - edge);
                }

                spawnPosition.y = mesh.bounds.size.y / 2.0f;

                float distance = Vector3.Distance(PlayerController.global.transform.position, spawnPosition);

                if (distance > 40.0f)
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
                            Destroy(colliders[i].gameObject);
                        }
                    }
                    GameObject camp = Instantiate(campPrefab, spawnPosition, Quaternion.identity);
                    spawnCamp = true;
                }
                else
                {
                    return;
                }
            }
        }
    }
}