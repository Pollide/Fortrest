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
    }
}