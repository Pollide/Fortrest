using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Shrine : MonoBehaviour
{
    public GameObject shrineObject;

    //public TMP_Text interactText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerController.global.gameObject)
        {
            GameManager.PlayAnimation(shrineObject.GetComponent<Animation>(), "ShrineSpin");
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            PlayerController.global.canTeleport = true;
            PlayerController.global.needInteraction = true;
            //LevelManager.FloatingTextChange(interactText.gameObject, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerController.global.gameObject)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            PlayerController.global.canTeleport = false;
            PlayerController.global.needInteraction = false;
            //LevelManager.FloatingTextChange(interactText.gameObject, false);
        }
    }

    private void Update()
    {
        if (!PlayerController.global.canTeleport && gameObject.GetComponent<MeshRenderer>().enabled == false)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            //LevelManager.FloatingTextChange(interactText.gameObject, false);
        }
    }
}