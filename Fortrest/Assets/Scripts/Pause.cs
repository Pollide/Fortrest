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
    public bool TurretMenu;
    public Transform ButtonHolder;

    //[HideInInspector]
    public List<int> SelectedList = new List<int>();
    private void Awake()
    {
        if (!TurretMenu)
            global = this; //set the only menu to this. No need to destroy any old ones as the menu isnt under DoNotDestroy
    }

    public void Reset()
    {
        SelectedList = new List<int>();

        for (int i = 0; i < ButtonHolder.childCount; i++)
        {
            SelectedList.Add(0);
        }
    }

    private void Update()
    {
        // Debug.Log(SelectedList.Count + " > 0 && " + ButtonHolder.gameObject.activeInHierarchy);
        if (SelectedList.Count > 0 && ButtonHolder.gameObject.activeInHierarchy)
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

    public bool ChangeMenu(int open)
    {
        bool anotherWasOpen = false;
        for (int i = 0; i < ButtonHolder.childCount; i++)
        {
            GameObject menu = ButtonHolder.GetChild(i).gameObject;
            if (menu.activeSelf && i != open)
            {
                anotherWasOpen = true;
            }
            menu.SetActive(false);
        }
        ButtonHolder.GetChild(open).gameObject.SetActive(true);

        return anotherWasOpen;
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

        // Debug.Log(index + " " + SelectedList[index] + " -> " + ButtonHolder.GetChild(index).childCount);

        SelectedList[index] = (int)GameManager.ReturnThresholds(SelectedList[index], ButtonHolder.GetChild(index).childCount - 1);

        for (int i = 0; i < ButtonHolder.GetChild(index).childCount; i++)
        {
            Transform button = ButtonHolder.GetChild(index).GetChild(i);

            bool selected = SelectedList[index] == i;
            button.GetChild(1).gameObject.SetActive(selected);

            float shrinkScale = selected ? 1.1f : 1;

            button.localScale = new Vector3(shrinkScale, shrinkScale, shrinkScale);
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || PlayerController.global.pauseSelectCTRL)
        {
            PlayerController.global.pauseSelectCTRL = false;
            ButtonHolder.GetChild(index).GetChild(SelectedList[index]).GetComponent<ButtonMechanics>().SelectVoid();
        }
    }
}