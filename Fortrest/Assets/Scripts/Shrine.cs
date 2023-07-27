using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : MonoBehaviour
{
    public GameObject shrineObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerController.global.gameObject)
        {
            GameManager.PlayAnimation(shrineObject.GetComponent<Animation>(), "ShrineSpin");
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            PlayerController.global.canTeleport = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerController.global.gameObject)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            PlayerController.global.canTeleport = false;
        }
    }

    private void Update()
    {
        if (!PlayerController.global.canTeleport && gameObject.GetComponent<MeshRenderer>().enabled == false)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }
}