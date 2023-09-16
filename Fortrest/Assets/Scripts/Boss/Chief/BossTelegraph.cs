using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossTelegraph : MonoBehaviour
{
    [SerializeField] private Image telegraphImage;
    [SerializeField] private SlamState state;

    // Start is called before the first frame update
    void Start()
    {
        telegraphImage.rectTransform.localScale = Vector3.zero;
        gameObject.transform.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
        telegraphImage.rectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, state.SlamWaitTime / state.SlamDuration);
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
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, state.SlamRadius);
    }
}
