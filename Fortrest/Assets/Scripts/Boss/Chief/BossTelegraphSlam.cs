using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossTelegraphSlam : MonoBehaviour
{
    [SerializeField] private PhaseThreeAttack slamState;
    [SerializeField] private AttackManagerState attackManagerState;
    public GameObject outer;
    public bool attack = false;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.SetParent(null);
        transform.localScale = Vector3.zero;       
    }

    // Update is called once per frame
    void Update()
    {
        outer.transform.position = transform.position;

        if (!attack)
        {
            Vector3 newScale = new(slamState.SlamRadius * 2, slamState.SlamRadius * 2, 1);
            transform.localScale = Vector3.Lerp(Vector3.zero, newScale, slamState.SlamWaitTime / slamState.SlamDuration);
            outer.transform.localScale = newScale / 3.8f;
        }
        else
        {
            Vector3 newScale = new Vector3(attackManagerState.attackRadius * 2, attackManagerState.attackRadius * 2, 1);
            transform.localScale = Vector3.Lerp(Vector3.zero, newScale, attackManagerState.attackTime / attackManagerState.attackDuration);
            outer.transform.localScale = newScale / 3.8f;
        }
    }


    public IEnumerator DoSlamDamage(float waitTime, float damage, float radius)
    {
        yield return new WaitForSeconds(waitTime);
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (var collider in colliders)
        {
            if (collider.GetComponent<PlayerController>())
            {
                PlayerController player = collider.GetComponent<PlayerController>();
                if (!attack)
                {
                    player.TakeDamage(slamState.Damage, true);
                }
                else
                {
                    player.TakeDamage(attackManagerState.Damage, true);
                }
                Vector3 pushDirection = player.transform.position - transform.position;
                float angle = Vector3.Angle(pushDirection, player.transform.position - transform.position);
                pushDirection = Quaternion.Euler(0f, angle, 0f) * pushDirection;
                player.SetPushDirection(pushDirection, 1);
                StartCoroutine(player.PushPlayer(0.5f));
            }
        }



        if (!attack)
        {
            ScreenShake.global.shake = true;
            slamState.StateMachine.ChangeState(slamState.StateAttack);
        }
        else
        {
            yield return new WaitForSeconds(0.2f);

            attackManagerState.IsAttacking = false;
        }
    }
}
