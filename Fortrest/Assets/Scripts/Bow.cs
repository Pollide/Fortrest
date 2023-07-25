using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public GameObject arrowObject;
    public Transform firePoint;
    public float fireForce = 20.0f;

    public void Shoot()
    {
        GameObject arrow = Instantiate(arrowObject, firePoint.position, Quaternion.Euler(90f, PlayerController.global.transform.eulerAngles.y, 0));
        arrow.GetComponent<Rigidbody>().AddForce(PlayerController.global.transform.forward * fireForce, ForceMode.Impulse);
    }
}
