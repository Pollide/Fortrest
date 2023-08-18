using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public static Bow global;
    public GameObject arrowObject;
    private float fireForce = 40.0f;
    public GameObject bowMeshObject;

    private void Start()
    {
        global = this;
    }

    public void Shoot()
    {
        GameObject arrow = Instantiate(arrowObject, transform.position, Quaternion.Euler(90f, transform.eulerAngles.y - 90.0f, 0f));
        arrow.GetComponent<Rigidbody>().AddForce(-transform.right * fireForce, ForceMode.Impulse);
    }
}
