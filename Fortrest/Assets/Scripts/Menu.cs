/*********************************************************************
Bachelor of Software Engineering
Media Design School
Auckland
New Zealand
(c) 2023 Media Design School
File Name : Menu.cs
Description : Holds menu objects that can be accessed via ButtonMechanics
Author :  Allister Hamilton
Mail : allister.hamilton @mds.ac.nz
**************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Menu : MonoBehaviour
{
    public static Menu global;
    public Transform SignHolderTransform;
    public Transform CameraTransform;
    public int ActiveSignInt;

    [HideInInspector] public Vector2 moveCTRL;
    public AnimationState IntialAnimationState;

    private bool canGo, left, right;
    

    private void Awake()
    {
        global = this; //set the only menu to this. No need to destroy any old ones as the menu isnt under DoNotDestroy
    }

    private void Start()
    {
        // Left stick to move

        GameManager.global.gamepadControls.Controls.Move.performed += context => moveCTRL = context.ReadValue<Vector2>();
        GameManager.global.gamepadControls.Controls.Move.performed += context => ControllerSelection();
        GameManager.global.gamepadControls.Controls.Move.canceled += context => moveCTRL = Vector2.zero;
        GameManager.global.gamepadControls.Controls.Move.canceled += context => canGo = false;

        IntialAnimationState = GameManager.PlayAnimation(GetComponent<Animation>(), "Initial Menu");
        ReturnButton().SelectVoid(true);
    }

    private void ControllerSelection()
    {
        if (moveCTRL.x > 0f && !canGo)
        {
            right = true;
            canGo = true;
        }
        else if (moveCTRL.x < 0f && !canGo)
        {
            left = true;
            canGo = true;
        }
    }

    private void Update()
    {
        PlayerModeHandler.SetMouseActive(false);

        if (!IntialAnimationState || !IntialAnimationState.enabled)
        {
            CameraTransform.position = Vector3.Slerp(CameraTransform.position, ReturnSign().position + (ReturnSign().forward * -10) + Vector3.up, 3 * Time.deltaTime);
            CameraTransform.rotation = Quaternion.RotateTowards(CameraTransform.rotation, ReturnSign().rotation, 20 * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) || left)
            {
                left = false;
                Direction(-1);
            }

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) || right)
            {
                right = false;
                Direction(1);
            }

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                ReturnButton().OnPointerClick(null);
            }
        }
    }

    Transform ReturnSign()
    {
        return SignHolderTransform.GetChild(ActiveSignInt).transform;
    }

    ButtonMechanics ReturnButton()
    {
        return ReturnSign().GetComponent<ButtonMechanics>();
    }

    void Direction(int direction)
    {
        GameManager.global.SoundManager.PlaySound(GameManager.global.MenuSwooshSound);

        ReturnButton().SelectVoid(false);
        ActiveSignInt = (int)GameManager.ReturnThresholds(ActiveSignInt + direction, SignHolderTransform.childCount - 1);

        ReturnButton().SelectVoid(true);
    }
}
