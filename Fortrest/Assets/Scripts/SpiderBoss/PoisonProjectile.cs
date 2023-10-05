using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonProjectile : MonoBehaviour
{
    private bool hitOnce;

    private void Start()
    {
        StartCoroutine(SelfDestruct());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == PlayerController.global.gameObject && !hitOnce && !PlayerController.global.evading)
        {
            PlayerController.global.poisoned = true;
            hitOnce = true;
        }
        Destroy(gameObject);
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(1.6f);
        Destroy(gameObject);
    }
}