using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossTelegraphSlam : MonoBehaviour
{
    [SerializeField] private PhaseThreeAttack slamState;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.SetParent(null);
        transform.localScale = Vector3.zero;       
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(slamState.SlamRadius * 2, slamState.SlamRadius * 2, 1), slamState.SlamWaitTime / slamState.SlamDuration);
    }


    public IEnumerator DoSlamDamage(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, slamState.SlamRadius);

        foreach (var collider in colliders)
        {
            if (collider.GetComponent<PlayerController>())
            {
                collider.GetComponent<PlayerController>().TakeDamage(slamState.Damage, true);
            }
        }

        ScreenShake.global.shake = true;

        slamState.StateMachine.ChangeState(slamState.StateAttack);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, slamState.SlamRadius);
    }
}
