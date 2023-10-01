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

public class Buttons : MonoBehaviour
{
    public Transform ButtonHolder;
    //[HideInInspector]
    public List<int> MenuList = new List<int>();
    public bool pressingDown;
    public bool AllowControllerToNavigate = true;
    private void OnEnable()
    {
        Start();
    }

    private void Start()
    {
        MenuList = new List<int>();

        for (int i = 0; i < ButtonHolder.childCount; i++)
        {
            MenuList.Add(0);
        }
    }

    private void Update()
    {
        if (MenuList.Count > 0 && ButtonHolder.gameObject.activeInHierarchy)
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
        int menu = ReturnIndex();

        if (ButtonHolder.GetChild(menu).gameObject.activeInHierarchy)
        {
            int direction = 0;

            if (AllowControllerToNavigate)
            {
                if (GameManager.global.upCTRL)
                {
                    GameManager.global.upCTRL = false;
                    direction = -1;
                }

                if (GameManager.global.downCTRL)
                {
                    GameManager.global.downCTRL = false;
                    direction = 1;
                }
            }

            MenuList[menu] += direction;

            MenuList[menu] = (int)GameManager.ReturnThresholds(MenuList[menu], ButtonHolder.GetChild(menu).childCount - 1);

            for (int i = 0; i < ButtonHolder.GetChild(menu).childCount; i++)
            {
                Transform button = ButtonHolder.GetChild(menu).GetChild(i);


                ButtonMechanics buttonMechanics = button.GetComponent<ButtonMechanics>();

                bool selected = MenuList[menu] == i;

                if (!button.gameObject.activeSelf && selected) //skips inactive
                {
                    // Debug.Log(MenuList[menu] + "direction: " + direction);
                    MenuList[menu] += direction != 0 ? direction : 1;
                    ButtonInput();
                    return;
                }

                if (buttonMechanics.SelectedGameObject)
                    buttonMechanics.SelectedGameObject.SetActive(selected);

                float shrinkScale = selected ? (pressingDown ? 0.95f : 1.05f) : 1;

                button.localScale = new Vector3(shrinkScale, shrinkScale, shrinkScale);
                buttonMechanics.Start(); //refreshes text
            }

            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || GameManager.global.selectCTRL)
            {
                GameManager.global.selectCTRL = false;
                ButtonHolder.GetChild(menu).GetChild(MenuList[menu]).GetComponent<ButtonMechanics>().SelectVoid();
            }
        }
    }
}