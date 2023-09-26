using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossTelegraph : MonoBehaviour
{
    [SerializeField] private ThirdAttackState attackState;
    [SerializeField] private Sprite sprite;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.SetParent(null);
        transform.localScale = Vector3.zero;       
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(attackState.SlamRadius * 2, attackState.SlamRadius * 2, 1), attackState.SlamWaitTime / attackState.SlamDuration);
    }


    public IEnumerator DoSlamDamage(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackState.SlamRadius);

        foreach (var collider in colliders)
        {
            if (collider.GetComponent<PlayerController>())
            {
                collider.GetComponent<PlayerController>().TakeDamage(attackState.Damage, true);
            }
        }

        ScreenShake.global.shake = true;

        attackState.StateMachine.ChangeState(attackState.StateAttack);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackState.SlamRadius);
    }
}
