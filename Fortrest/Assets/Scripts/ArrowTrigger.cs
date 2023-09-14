using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrigger : MonoBehaviour
{
    [HideInInspector] public bool singleHit;
    [HideInInspector] public bool hitFirstEnemy;
    [HideInInspector] public bool hitSecondEnemy;
    GameObject temp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && !hitFirstEnemy)
        {
            hitFirstEnemy = true;
            temp = other.gameObject;
        }
        if (other.gameObject.tag == "Enemy" && other.gameObject != temp && hitFirstEnemy)
        {
            hitSecondEnemy = true;
            singleHit = false;
        }
    }
}