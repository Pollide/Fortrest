using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TelegraphCircle : MonoBehaviour
{
    [SerializeField] private PhaseThreeAttack slamState;
    [SerializeField] private AttackManagerState phaseOneState;
    public GameObject outer;
    public GameObject inner;
    public bool isAttack = false;

    // Start is called before the first frame update
    void Start()
    {
        if (inner)
        {
            inner.gameObject.transform.SetParent(null);
            inner.transform.localScale = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (outer)
        {
            outer.transform.position = transform.position;

            if (!isAttack)
            {
                Vector3 newScale = new(slamState.SlamRadius * 2, slamState.SlamRadius * 2, 1);
                inner.transform.localScale = Vector3.Lerp(Vector3.zero, newScale, slamState.SlamWaitTime / slamState.SlamDuration);
                outer.transform.localScale = newScale / 3.8f;
            }
            else
            {
                Vector3 newScale = new(phaseOneState.attackRadius * 2, phaseOneState.attackRadius * 2, 1);
                inner.transform.localScale = Vector3.Lerp(Vector3.zero, newScale, phaseOneState.attackTime / phaseOneState.attackDuration);
                outer.transform.localScale = newScale / 3.8f;
            }
        }
    }

    public IEnumerator DoAreaDamage(float waitTime, float damage, float radius)
    {
        yield return new WaitForSeconds(waitTime);

        Collider[] colliders = Physics.OverlapSphere(inner.transform.position, radius);

        foreach (var collider in colliders)
        {
            if (collider.GetComponent<PlayerController>())
            {
                PlayerController player = collider.GetComponent<PlayerController>();

                player.TakeDamage(damage);

                Vector3 pushDirection = player.transform.position - inner.transform.position;
                float angle = Vector3.Angle(pushDirection, player.transform.position - inner.transform.position);
                pushDirection = Quaternion.Euler(0f, angle, 0f) * pushDirection;
                player.SetPushDirection(pushDirection, 1);
                StartCoroutine(player.PushPlayer(0.5f));
            }
        }



        if (!isAttack)
        {
            ScreenShake.global.ShakeScreen(0.3f);
            slamState.StateMachine.ChangeState(slamState.StateAttack);
        }
        else
        {
            yield return new WaitForSeconds(0.2f);

            phaseOneState.IsAttacking = false;
        }

        outer.SetActive(false);
    }
}
