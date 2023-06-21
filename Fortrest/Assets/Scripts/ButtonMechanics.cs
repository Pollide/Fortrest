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
    [Space(10)] //creates a gap in the inspector
    public bool SpeedBool;
    public bool NextScene;
    public Sprite play;
    public Sprite fastForward;

    void Start()
    {
        //this finds all images in the button and makes sure only the top is raycastable so the button clicks properly
        List<Image> ImageList = GameManager.FindComponent<Image>(transform);

        //change the childs to not be raycastable so it doesnt interfare with this
        for (int i = 0; i < ImageList.Count; i++)
        {
            ImageList[i].raycastTarget = i == 0;
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
        ChangeSizeVoid(1.1f);
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

        if (QuitGameBool)
        {
            Application.Quit();
        }

        if (PlayGameBool)
        {
            PlayerController.global.PauseVoid(false);
        }

        if (SpeedBool)
        {
            Time.timeScale = Time.timeScale == 2 ? 1 : 2;
            GetComponent<Image>().color = Time.timeScale == 2 ? Color.red : Color.white;
            transform.GetComponent<Image>().sprite = Time.timeScale == 2 ? play : fastForward;
        }

        if (OptionsBool)
        {
            Menu.global.OptionsCanvas.SetActive(true);
            Menu.global.MenuCanvas.SetActive(false);
        }
        if (RestartBool)
        {
            Time.timeScale = 1f;
            PlayerController.global.transform.SetParent(LevelManager.global.transform); //so it can properly reset and clear old player
            GameManager.global.NextScene(1);
        }

        if (BackBool)
        {
            Menu.global.OptionsCanvas.SetActive(false);
            Menu.global.MenuCanvas.SetActive(true);
        }
        if (MenuBool)
        {
            Time.timeScale = 1f;
            GameManager.global.NextScene(0);
        }
        if (NextScene)
        {
            Time.timeScale = 1f;
            GameManager.global.NextScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    //changes the size of the button for visual feedback
    void ChangeSizeVoid(float shrinkFloat)
    {
        transform.localScale = new Vector3(shrinkFloat, shrinkFloat, shrinkFloat);
    }

    //change the color when its clicked for more visual feedback
    void ChangeColourVoid(Color _color)
    {
        GetComponent<Image>().color = _color;
    }

    void Update()
    {
        if (SpeedBool)
        {
            if (Input.GetMouseButtonDown(1) && PlayerModeHandler.global.playerModes != PlayerModes.Paused && PlayerModeHandler.global.playerModes != PlayerModes.UpgradeMenu)
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.SpeedButtonClickSound);
                OnPointerClick(null);
            }
        }
    }
}
