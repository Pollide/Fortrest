using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShrineActivate : MonoBehaviour
{
    
    public Animator PropAnimator;
    public string AnimationTriggerName;
    public TMP_Text interactText;
    bool textDisplayed;
    bool notGood;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Boar.global.gameObject && !Boar.global.mounted)
        {
            PlayerController.global.canTeleport = false;
            notGood = true;
        }

        if (other.gameObject == PlayerController.global.gameObject && !notGood)
        {
            if (!textDisplayed)
            {
                LevelManager.FloatingTextChange(interactText.gameObject, true);
                textDisplayed = true;
            }
            PropAnimator.ResetTrigger(AnimationTriggerName);
            PropAnimator.SetTrigger(AnimationTriggerName);
            PlayerController.global.canTeleport = true;
            PlayerController.global.needInteraction = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == Boar.global.gameObject && !Boar.global.mounted)
        {
            PlayerController.global.canTeleport = false;
            notGood = true;
        }

        if (other.gameObject == PlayerController.global.gameObject && !notGood)
        {
            if (!textDisplayed)
            {
                LevelManager.FloatingTextChange(interactText.gameObject, true);
                textDisplayed = true;
            }
            PlayerController.global.canTeleport = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerController.global.gameObject)
        {
            if (textDisplayed)
            {
                LevelManager.FloatingTextChange(interactText.gameObject, false);
                textDisplayed = false;
            }
            PlayerController.global.canTeleport = false;
            PlayerController.global.needInteraction = false;
        }
    }

    private void Update()
    {
        if (!PlayerController.global.canTeleport)
        {
            if (textDisplayed)
            {
                LevelManager.FloatingTextChange(interactText.gameObject, false);
                textDisplayed = false;
            }
        }

        if (Boar.global.mounted)
        {
            notGood = false;
        }
    }
}