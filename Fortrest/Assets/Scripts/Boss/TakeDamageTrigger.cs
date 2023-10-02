using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageTrigger : MonoBehaviour
{
    private PlayerController player;
    private BossStateMachine stateMachine;

    private void Start()
    {
        player = PlayerController.global;
        stateMachine = transform.parent.GetComponent<BossStateMachine>();
    }

    private void  OnTriggerStay(Collider other)
    {
        if (other.gameObject == player.SwordGameObject)
        {
            player.cursorNearEnemy = true;

            if (player.attacking && stateMachine.CanBeDamaged && player.damageEnemy)
            {
                stateMachine.TakeDamage(player.attackDamage);
            }
            
        }
    }
}