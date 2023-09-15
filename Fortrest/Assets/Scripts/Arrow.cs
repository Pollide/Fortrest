using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{  
    private void OnCollisionEnter(Collision collision)
    {
        GameManager.global.SoundManager.PlaySound(GameManager.global.ArrowHitBuildingSound, 1.0f, true, 0, false, transform);
        Destroy(gameObject);
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