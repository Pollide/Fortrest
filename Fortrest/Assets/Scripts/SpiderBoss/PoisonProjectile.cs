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
            int randomInt = Random.Range(0, 3);
            AudioClip temp = null;
            switch (randomInt)
            {
                case 0:
                    temp = GameManager.global.PlayerHit1Sound;
                    break;
                case 1:
                    temp = GameManager.global.PlayerHit2Sound;
                    break;
                case 2:
                    temp = GameManager.global.PlayerHit3Sound;
                    break;
                default:
                    break;
            }
        }
        Destroy(gameObject);
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(1.6f);
        Destroy(gameObject);
    }
}