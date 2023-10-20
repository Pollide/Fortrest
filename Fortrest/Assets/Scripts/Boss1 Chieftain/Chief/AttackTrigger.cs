using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    [SerializeField] private AttackManagerState attackState;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && attackState.IsAttacking)
        {
            attackState.ApplyDamageToTarget(attackState.Damage);
        }  
    }
}