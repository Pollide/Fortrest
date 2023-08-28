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
        Vector3 direction = PlayerController.global.lookDirection;
        direction.Normalize();
        direction.y = 0;
        GameObject arrow = Instantiate(arrowObject, transform.position, Quaternion.Euler(90f, PlayerController.global.transform.eulerAngles.y, 0f));
        arrow.GetComponent<Rigidbody>().AddForce(PlayerController.global.transform.forward * fireForce, ForceMode.Impulse);
    }
}
