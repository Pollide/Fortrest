using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BoarPushBack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (GetComponentInParent<Boar>().currentSpeed > 60.0f)
            {              
                NavMeshAgent enemyAgent = other.GetComponent<NavMeshAgent>();
                EnemyController enemyController = other.GetComponent<EnemyController>();
                Vector3 direction = (enemyAgent.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(Boar.global.transform.forward, direction);
                if (enemyController.canBeDamagedByBoar)
                {
                    enemyController.Damaged(GetComponentInParent<Boar>().currentSpeed / 300.0f);
                    enemyController.canBeDamagedByBoar = false;
                    StartCoroutine(ResetBoarDamage(enemyController));
                }              
                enemyAgent.velocity = (direction + (Boar.global.transform.right * (angle / 60.0f))) * (Boar.global.currentSpeed / 5.0f);
                StartCoroutine(enemyController.BoarKnockEffects());
            } 
        }
    }

    public IEnumerator ResetBoarDamage(EnemyController enemy)
    {
        yield return new WaitForSeconds(2.0f);
        enemy.canBeDamagedByBoar = true;
    }
}
