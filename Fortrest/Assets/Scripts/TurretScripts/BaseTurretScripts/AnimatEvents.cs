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
        GameObject bolt = Instantiate(spawnedBolt, spawnedBoltPoint);
        TurretShooting turret = GetComponentInParent<TurretShooting>();
        U_Turret uTurret = GetComponentInParent<U_Turret>();
        BoltScript boltScript = bolt.GetComponent<BoltScript>();

        if (uTurret.isMultiShotActive)
        {
            float range = Random.Range(0f, 101);
            if (range <= uTurret.multiShotPercentage)
            {
                GameObject bolt2 = Instantiate(spawnedBolt, spawnedBoltPoint);
                bolt2.GetComponent<BoltScript>().SetDamage(turret.damage / 2f);
                bolt2.transform.Rotate(new Vector3(0, 25, 0));
                GameObject bolt3 = Instantiate(spawnedBolt, spawnedBoltPoint);
                bolt3.GetComponent<BoltScript>().SetDamage(turret.damage / 2f);
                bolt3.transform.Rotate(new Vector3(0, -25, 0));
            }
        }

        GameManager.global.SoundManager.PlaySound(GameManager.global.TurretShootSound);
        turret.animController.SetBool("isAttacking", false);
        boltScript.SetDamage(turret.damage);
    }
}
