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

    public Transform ButtonHolder;

    [HideInInspector]
    public List<int> SelectedList = new List<int>();
    private void Awake()
    {
        global = this; //set the only menu to this. No need to destroy any old ones as the menu isnt under DoNotDestroy


    }

    private void Update()
    {
        PlayerModeHandler.SetMouseActive(true);
        ButtonInput();
    }

    public int ReturnIndex()
    {
        for (int i = 0; i < ButtonHolder.childCount; i++)
        {
            if (ButtonHolder.GetChild(i).gameObject.activeSelf)
            {
                return i;
            }
        }
        return 0;
    }

    void ButtonInput()
    {
        int index = ReturnIndex();

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || PlayerController.global.upCTRL)
        {
            PlayerController.global.upCTRL = false;
            SelectedList[index]--;
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || PlayerController.global.downCTRL)
        {
            PlayerController.global.downCTRL = false;
            SelectedList[index]++;
        }

        SelectedList[index] = (int)GameManager.ReturnThresholds(SelectedList[index], ButtonHolder.GetChild(index).childCount - 1);

        for (int i = 0; i < ButtonHolder.GetChild(index).childCount; i++)
        {
            ButtonHolder.GetChild(index).GetChild(i).GetChild(1).gameObject.SetActive(SelectedList[index] == i);
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || PlayerController.global.pauseSelectCTRL)
        {
            PlayerController.global.pauseSelectCTRL = false;
            ButtonHolder.GetChild(index).GetChild(SelectedList[index]).GetComponent<ButtonMechanics>().SelectVoid();
        }
    }
}