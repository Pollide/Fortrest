using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniTurretEvents : MonoBehaviour
{
    public GameObject boltObject;
    public GameObject projectile;
    public Transform spawnPoint;

    private void BoltAppear()
    {
        boltObject.SetActive(false);
    }

    private void BoltDisappear()
    {
        boltObject.SetActive(false);
    }
    public void SpawnBolt()
    {
        GameObject bolt = Instantiate(projectile, spawnPoint);        
        BoltScript boltScript = bolt.GetComponent<BoltScript>();
        boltScript.mini = true;
        MiniTurret turret = GetComponentInParent<MiniTurret>();
        GameManager.global.SoundManager.PlaySound(GameManager.global.TurretShootSound);
        turret.animator.SetBool("isAttacking", false);
        boltScript.SetDamage(turret.damage);
    }
}
