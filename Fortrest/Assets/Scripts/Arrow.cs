using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [HideInInspector] public bool singleHit;
    [HideInInspector] public bool secondHit;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != PlayerController.global.gameObject)
        {
            Destroy(gameObject);
        }       
    }

    private void Start()
    {
        StartCoroutine(SelfDestruct());
    }

    private IEnumerator SelfDestruct()
    {      
        yield return new WaitForSeconds(1.6f);
        Destroy(gameObject);
    }
}