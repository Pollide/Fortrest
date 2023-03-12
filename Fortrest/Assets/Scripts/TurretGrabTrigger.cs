using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretGrabTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        gameObject.GetComponentInChildren<TurretShooting>().RunTrigger(other);
    }
}
