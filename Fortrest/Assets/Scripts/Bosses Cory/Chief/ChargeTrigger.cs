using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeTrigger : MonoBehaviour
{
    private PhaseTwoAttack state;

    private void Start()
    {
        state = transform.parent.GetComponent<PhaseTwoAttack>();
        GetComponent<BoxCollider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !state.PlayerHit)
        {
            state.PlayerHit = true;
            PlayerController player = other.GetComponent<PlayerController>();
            player.TakeDamage(state.Damage);
            Vector3 pushDirection = state.PlayerTransform.position - transform.position;
            float angle = Vector3.Angle(pushDirection, player.transform.position - transform.position);
            pushDirection = Quaternion.Euler(0f, angle, 0f) * pushDirection;
            player.SetPushDirection(pushDirection, state.ChargePushForce);
            StartCoroutine(player.PushPlayer(state.ChargePushDuration));
        }
    }
}
