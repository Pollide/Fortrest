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
        //I made it that the arrow is instantiated on PlayerController.global.transform.position and not the bow itself as it makes the arrow perfectly land on the cursor
        GameObject arrow = Instantiate(arrowObject, PlayerController.global.transform.position, Quaternion.Euler(90f, PlayerController.global.transform.eulerAngles.y, 0f));
        arrow.GetComponent<Rigidbody>().AddForce(PlayerController.global.transform.forward * fireForce, ForceMode.Impulse);
    }
}
