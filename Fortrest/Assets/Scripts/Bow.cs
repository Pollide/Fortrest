using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public GameObject arrowObject;
    public Transform firePoint;
    private float fireForce = 30.0f;

    public void Shoot()
    {
        GameObject arrow = Instantiate(arrowObject, firePoint.position, Quaternion.Euler(0f, transform.eulerAngles.y, -90f));
        arrow.GetComponent<Rigidbody>().AddForce(PlayerController.global.transform.forward * fireForce, ForceMode.Impulse);
    }
}
