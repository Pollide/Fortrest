using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageTrigger : MonoBehaviour
{
    private PlayerController player;
    public BossStateMachine stateMachine;

    private void Start()
    {
        player = PlayerController.global;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == player.SwordGameObject)
        {
            if (player.attacking && stateMachine.CanBeDamaged && player.damageEnemy)
            {
                stateMachine.CanBeDamaged = false;
                stateMachine.TakeDamage(player.attackDamage);
            }
        }
        if (other.gameObject.tag == "Arrow" && other.GetComponent<ArrowTrigger>())
        {
            if (!other.GetComponent<ArrowTrigger>().singleHit)
            {
                other.GetComponent<ArrowTrigger>().singleHit = true;
                stateMachine.TakeDamage(PlayerController.global.bowDamage);
                if (!PlayerController.global.upgradedBow || other.GetComponent<ArrowTrigger>().hitSecondEnemy)
                {
                    Destroy(other.gameObject.transform.parent.gameObject);
                }
            }
        }
    }
}