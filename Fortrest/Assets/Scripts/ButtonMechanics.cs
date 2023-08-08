/*********************************************************************
Bachelor of Software Engineering
Media Design School
Auckland
New Zealand
(c) 2023 Media Design School
File Name : ButtonMechanics.cs
Description : used to communicate with the inspector via booleans of what functions should be called on click
Author :  Allister Hamilton
Mail : allister.hamilton @mds.ac.nz
**************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class ButtonMechanics : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{

    [Header("Pause")]

    [Tooltip("quits the exe, does not work in editor")]
    public bool ResumeBool;
    public bool RestartBool;
    public bool MenuBool;
    [Space(10)] //creates a gap in the inspector

    [Header("Menu")]
    public bool PlayBool;
    public bool ExitBool;
    public bool ResetBool;
    [Space(10)]
    public bool AreYouSureBool;
    bool ToggleBool;
    [Space(10)]
    public TMP_Text MenuText;

    void Start()
    {
        if (!MenuText)
        {
            //this finds all images in the button and makes sure only the top is raycastable so the button clicks properly
            List<Image> ImageList = GameManager.FindComponent<Image>(transform);

            //change the childs to not be raycastable so it doesnt interfare with this
            for (int i = 0; i < ImageList.Count; i++)
            {
                ImageList[i].raycastTarget = i == 0;
            }
        }

        if (PlayBool)
        {
            if ((int)GameManager.Pref("Game Started", 0, true) == 1)
            {
                MenuText.text = "Continue\n" + "Day " + (int)GameManager.Pref("Day", 0, true);
            }
            else
            {
                MenuText.text = "New Game";
            }
        }
    }

    //checks to see if the pointer has exited the button
    public void OnPointerExit(PointerEventData eventData)
    {
        ChangeSizeVoid(1f);
        //ChangeColourVoid(new Color(164.0f / 255.0f, 164.0f / 255.0f, 164.0f / 255.0f));
    }

    //checks to see if the pointer has entered the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        //GameManager.global.SoundManager.PlaySound(GameManager.global.MenuClick1Sound);
        ChangeSizeVoid(1);
        //ChangeColourVoid(Color.white);
    }

    //checks to see if the mouse was clicked ontop of the button
    public void OnPointerDown(PointerEventData eventData)
    {
        ChangeSizeVoid(0.9f);
    }

    //checks to see if the mouse was let go ontop of the button
    public void OnPointerClick(PointerEventData eventData)
    {
        OnPointerEnter(eventData);

        if (ResumeBool)
        {
            PlayerController.global.PauseVoid(false);
        }

        if (RestartBool)
        {
            Time.timeScale = 1f;
            GameManager.global.NextScene(1);
        }

        if (MenuBool)
        {
            Time.timeScale = 1f;
            GameManager.global.NextScene(0);
        }

        if (MenuText)
            GameManager.PlayAnimation(GetComponent<Animation>(), "Sign Accepted");

        if (AreYouSureBool)
        {
            ToggleBool = !ToggleBool;

            if (ToggleBool)
            {
                MenuText.text = "Are You Sure?";
                return;
            }
        }

        if (PlayBool)
        {
            GameManager.global.NextScene(1);
        }

        if (ResetBool)
        {
            PlayerPrefs.DeleteAll();

            GameManager.global.NextScene(0);
            GameManager.global.transform.SetParent(transform);
            GameManager.global = null;
        }

        if (ExitBool)
        {
            Application.Quit();
            GameManager.global.NextScene(0);
        }
    }

    public void SelectVoid(bool select)
    {
        if (select)
        {
            GameManager.PlayAnimation(GetComponent<Animation>(), "Sign Selected");
            GameManager.PlayAnimation(GetComponent<Animation>(), "Sign Loop");
        }
        else
        {
            GetComponent<Animation>().Stop();
            GameManager.PlayAnimation(GetComponent<Animation>(), "Sign Stop");
        }
    }

    //changes the size of the button for visual feedback
    void ChangeSizeVoid(float shrinkFloat)
    {
        transform.localScale = new Vector3(shrinkFloat, shrinkFloat, shrinkFloat);
    }

    void Update()
    {

    }
}
