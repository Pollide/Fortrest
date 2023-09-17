using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossTelegraph : MonoBehaviour
{
    [SerializeField] private SlamState state;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.SetParent(null);
        transform.localScale = Vector3.zero;       
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(state.SlamRadius * 2, state.SlamRadius * 2, 1), state.SlamWaitTime / state.SlamDuration);
    }

    public void DoSlamDamage()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, state.SlamRadius);

        foreach (var collider in colliders)
        {
            if (collider.GetComponent<PlayerController>())
            {
                collider.GetComponent<PlayerController>().TakeDamage(state.Damage, true);
            }
        }

        ScreenShake.global.shake = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, state.SlamRadius);
    }
}
