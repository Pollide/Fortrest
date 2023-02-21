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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class ButtonMechanics : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{

    [Header("Button Properties")]

    [Tooltip("quits the exe, does not work in editor")]
    public bool PlayGameBool;
    public bool RestartBool;
    public bool OptionsBool;
    public bool QuitGameBool;
    public bool BackBool;
    public bool MenuBool;

    public bool SpeedBool;

    void Start()
    {
        //this finds all images in the button and makes sure only the top is raycastable so the button clicks properly
        List<Image> ImageList = GameManager.FindComponent<Image>(transform);

        for (int i = 0; i < ImageList.Count; i++)
        {
            ImageList[i].raycastTarget = i == 0;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ChangeSizeVoid(1f);
        ChangeColourVoid(new Color(164.0f / 255.0f, 164.0f / 255.0f, 164.0f / 255.0f));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.global.SoundManager.PlaySound(GameManager.global.HoverClick);
        ChangeSizeVoid(1.1f);
        ChangeColourVoid(Color.white);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameManager.global.SoundManager.PlaySound(GameManager.global.Click);
        ChangeSizeVoid(0.9f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnPointerEnter(eventData);

        if (QuitGameBool)
        {
            Application.Quit();
        }
        if (PlayGameBool)
        {
            // GameManager.global.SoundManager.PlaySound(GameManager.global.GameStart);
            // GameManager.global.MusicManager.StopSelectedSound(GameManager.global.MenuMusic);

            GameManager.global.NextScene(1);
            // PlayerPrefs.SetInt("Skip Introduction")
        }

        if (SpeedBool)
        {
            Time.timeScale = Time.timeScale == 2 ? 1 : 2;
            // Debug.Log(transform.GetChild(0).GetComponent<TextMeshPro>());
            GetComponent<Image>().color = Time.timeScale == 2 ? Color.red : Color.white;
            transform.GetChild(0).GetComponent<TMP_Text>().text = Time.timeScale == 2 ? ">>" : ">";
        }

        if (OptionsBool)
        {
            Menu.global.OptionsCanvas.SetActive(true);
            Menu.global.MenuCanvas.SetActive(false);
        }
        if (RestartBool)
        {
            Time.timeScale = 1f;
            //Debug.Log(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            GameManager.global.NextScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }

        if (BackBool)
        {
            Menu.global.OptionsCanvas.SetActive(false);
            Menu.global.MenuCanvas.SetActive(true);
        }
        if (MenuBool)
        {
            Time.timeScale = 1f;
            //   GameManager.global.MusicManager.StopSelectedSound(GameManager.global.GameMusic);


            GameManager.global.NextScene(0);
        }
    }

    //changes the size of the button for visual feedback
    void ChangeSizeVoid(float shrinkFloat)
    {
        transform.localScale = new Vector3(shrinkFloat, shrinkFloat, shrinkFloat);
    }

    void ChangeColourVoid(Color _color)
    {
        GetComponent<Image>().color = _color;
    }
}
