using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageTrigger : MonoBehaviour
{
    private void  OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerController.global.SwordGameObject)
        {
            transform.parent.GetComponent<BossStateMachine>().TakeDamage(PlayerController.global.attackDamage * 10);
        }
    }
}
