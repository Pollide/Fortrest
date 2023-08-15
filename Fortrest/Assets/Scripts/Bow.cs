using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public static Bow global;
    public GameObject arrowObject;
    private float fireForce = 30.0f;

    private void Start()
    {
        global = this;
    }

    public void Shoot()
    {
        GameObject arrow = Instantiate(arrowObject, transform.position, Quaternion.Euler(90f, transform.eulerAngles.y, 0f));
        arrow.GetComponent<Rigidbody>().AddForce(transform.forward * fireForce, ForceMode.Impulse);
    }
}
