using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatEvents : MonoBehaviour
{
    public GameObject bolt;
    public GameObject spawnedBolt;
    public Transform spawnedBoltPoint;

    public void BoltDisapear()
    {
        bolt.SetActive(false);
    }

    public void BoltApear()
    {
        bolt.SetActive(true);
    }

    public void SpawnBolt()
    {
        Instantiate(spawnedBolt, spawnedBoltPoint);
        GameManager.global.SoundManager.PlaySound(GameManager.global.TurretShootSound);
    }
}
