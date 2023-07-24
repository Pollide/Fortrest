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
        GameObject arrow = Instantiate(arrowObject, firePoint.position, PlayerController.global.transform.rotation);
        Vector3 directionToPlayer = firePoint.position - arrow.transform.position;
        float rotationAngle = Mathf.Atan2(directionToPlayer.z, directionToPlayer.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(90f, rotationAngle, 0);
        //arrow.GetComponent<Rigidbody>().AddForce(PlayerController.global.transform.forward * fireForce, ForceMode.Impulse);
    }
}
