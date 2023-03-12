using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretShooting : MonoBehaviour
{
    public List<GameObject> enemies = new();
    public float turn_speed;
    public Animator animController;

    private void Update()
    {
        if (enemies.Count > 0)
        {
            Attack();
        }
    }

    public void RemoveFromList()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].GetComponent<EnemyController>().isDead)
            {
                enemies.RemoveAt(i);
                animController.SetBool("isAttacking", false);
            }
        }
    }

    private void Attack()
    {
        Quaternion _lookRotation = Quaternion.LookRotation((new Vector3(enemies[0].transform.position.x, transform.position.y, enemies[0].transform.position.z) - transform.position).normalized);

        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * turn_speed);

        animController.SetBool("isAttacking", true);

        GameManager.global.SoundManager.PlaySound(GameManager.global.TurretShootSound);
    }

    public void RunTrigger(Collider other)
    { 
        if (other.CompareTag("Enemy"))
        {
            enemies.Add(other.gameObject);
            other.GetComponent<EnemyController>().isInTurretRange = true;
            other.GetComponent<EnemyController>().turrets.Add(gameObject);
        }
    }
}
