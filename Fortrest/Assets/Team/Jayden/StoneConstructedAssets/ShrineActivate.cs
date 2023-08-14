using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShrineActivate : MonoBehaviour
{
    
    public Animator PropAnimator;
    public string AnimationTriggerName;



    public TMP_Text interactText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerController.global.gameObject)
        {
            PropAnimator.SetTrigger(AnimationTriggerName);
            PlayerController.global.canTeleport = true;
            PlayerController.global.needInteraction = true;
            //LevelManager.FloatingTextChange(interactText.gameObject, true);
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerController.global.gameObject)
        {

            PropAnimator.ResetTrigger(AnimationTriggerName);
            PlayerController.global.canTeleport = false;
            PlayerController.global.needInteraction = false;
            //LevelManager.FloatingTextChange(interactText.gameObject, false);
            
        }
    }

    private void Update()
    {
        
        if (!PlayerController.global.canTeleport && gameObject.GetComponent<MeshRenderer>().enabled == false)
        {
            
            //gameObject.GetComponent<MeshRenderer>().enabled = true;
            //LevelManager.FloatingTextChange(interactText.gameObject, false);
        }
    }
}