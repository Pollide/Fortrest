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

    private void Start()
    {
        HouseAnimator.SetBool("activateHouse", false);
    }

    private void Update()
    {
        if (GameManager.ReturnInMainMenu())
        {
            enabled = false;
            return;
        }
        if (PlayerModeHandler.global.canInteractWithHouse || PlayerModeHandler.global.inTheFortress)
        {
            HouseAnimator.SetBool("activateHouse", true);
        }
        else
        {
            HouseAnimator.SetBool("activateHouse", false);
        }
    }
}
