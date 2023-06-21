/*********************************************************************
Bachelor of Software Engineering
Media Design School
Auckland
New Zealand
(c) 2023 Media Design School
File Name : Pause.cs
Description : Holds pause objects that can be accessed via ButtonMechanics
Author :  Allister Hamilton
Mail : allister.hamilton @mds.ac.nz
**************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Pause : MonoBehaviour
{
    public static Pause global;

    public int StartButtonInt = 0;
    public int OptionButtonInt = 2;
    [Space(10)]
    public Transform StartButtonHolder;
    public Transform OptionButtonHolder;


    private void Awake()
    {
        global = this; //set the only menu to this. No need to destroy any old ones as the menu isnt under DoNotDestroy

    }

    private void Update()
    {
        ButtonInput(StartButtonHolder.gameObject.activeSelf ? StartButtonHolder : OptionButtonHolder, ref (StartButtonHolder.gameObject.activeSelf ? ref StartButtonInt : ref OptionButtonInt));
    }

    void ButtonInput(Transform buttonHolder, ref int buttonInt)
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            buttonInt--;
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            buttonInt++;
        }

        buttonInt = (int)GameManager.ReturnThresholds(buttonInt, buttonHolder.childCount - 1);

        for (int i = 0; i < buttonHolder.childCount; i++)
        {
            buttonHolder.GetChild(i).GetChild(1).gameObject.SetActive(buttonInt == i);
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            buttonHolder.GetChild(buttonInt).GetComponent<ButtonMechanics>().OnPointerDown(null);
        }

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Return))
        {
            buttonHolder.GetChild(buttonInt).GetComponent<ButtonMechanics>().OnPointerClick(null);
        }
    }
}