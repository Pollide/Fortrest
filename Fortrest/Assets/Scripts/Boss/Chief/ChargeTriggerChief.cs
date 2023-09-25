using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeTriggerChief : MonoBehaviour
{
    private ChargeStateChief state;

    private void Start()
    {
        state = transform.parent.GetComponent<ChargeStateChief>();
        GetComponent<BoxCollider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !state.PlayerHit)
        {
            state.PlayerHit = true;
            PlayerController player = other.GetComponent<PlayerController>();
            player.TakeDamage(state.Damage, true);
            Vector3 pushDirection = state.PlayerTransform.position - transform.position;
            float angle = Vector3.Angle(pushDirection, player.transform.position - transform.position);
            pushDirection = Quaternion.Euler(0f, angle, 0f) * pushDirection;
            player.SetPushDirection(pushDirection, state.ChargePushForce);
            StartCoroutine(player.PushPlayer(state.ChargePushDuration));
        }
    }
}
