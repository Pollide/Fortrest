using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HouseAnimationScript : MonoBehaviour
{

    //Components
    public Animator HouseAnimator; //Get the chosen animator of the target

    //Animation Clips
    public AnimationClip ActivateAnimation; //Request an activate animation clip
    public AnimationClip DeactivateAnimation; //Request a deactivate clip

    //Variable Inputs
    public string ActivateAnimationVariable; //Request an activate variable by name
    public string DectivateAnimationVariable; //Request a deactivate variable by name

    private void Update()
    {
        if (GameManager.ReturnInMainMenu())
        {
            enabled = false;
            return;
        }
        if (PlayerModeHandler.global.canInteractWithHouse)
        {
            HouseAnimator.SetBool("activateHouse", true);
            HouseAnimator.SetBool("deactivateHouse", false);
        }
        else
        {
            HouseAnimator.SetBool("deactivateHouse", true);
            HouseAnimator.SetBool("activateHouse", false);
        }
    }

    //// Start is called before the first frame update
    //void OnTriggerEnter(Collider other) //Enter Trigger Box
    //{
    //    if (other.CompareTag("Player") && !PlayerController.global.playerDead) 
    //    { //Checks to see if collider has tag "Player"
    //        HouseAnimator.SetBool("activateHouse", true); //Sets a variable inside the chosen animator
    //        HouseAnimator.SetBool("deactivateHouse", false); //Sets a variable inside the chosen animator
    //    }        
    //}
    //
    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.CompareTag("Player") && PlayerController.global.houseDisplay)
    //    { //Checks to see if collider has tag "Player"
    //        HouseAnimator.SetBool("activateHouse", true); //Sets a variable inside the chosen animator
    //        HouseAnimator.SetBool("deactivateHouse", false); //Sets a variable inside the chosen animator
    //    }
    //}
    //
    //// Update is called once per frame
    //void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Player")) 
    //    { //Checks to see if collider has tag "Player"
    //        HouseAnimator.SetBool("deactivateHouse", true); //Sets a variable inside the chosen animator
    //        HouseAnimator.SetBool("activateHouse", false); //Sets a variable inside the chosen animator
    //    }
    //}
}
