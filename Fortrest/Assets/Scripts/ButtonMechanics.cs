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

public class ButtonMechanics : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject SelectedGameObject;

    [Header("Pause")]

    [Tooltip("quits the exe, does not work in editor")]
    public bool ResumeBool;
    public bool RestartBool;
    public bool BeginingBool;
    public bool MenuBool;
    [Space(10)] //creates a gap in the inspector

    [Header("Cheats")]
    public bool SaveBool;
    public bool MaxResourcesBool;
    public int TerrainTeleportInt = -1;

    [Header("Build")]
    public int UpgradeInt;

    [Header("Menu")]
    public bool ContinueBool;
    public bool PlayBool;
    public bool ExitBool;
    [Space(10)]
    public bool VolumeBool;
    public bool MusicBool;
    public bool FullScreenBool;
    public bool ResetBool;

    [Header("Addictives")]

    public bool SecondMenuBool;
    public bool BackBool;
    public bool AreYouSureBool;
    bool ToggleBool;
    [Space(10)]
    public Text ButtonText;
    string TextString;

    public void Start()
    {
        if (!ButtonText)
        {
            //this finds all images in the button and makes sure only the top is raycastable so the button clicks properly
            List<Image> ImageList = GameManager.FindComponent<Image>(transform);

            //change the childs to not be raycastable so it doesnt interfare with this
            for (int i = 0; i < ImageList.Count; i++)
            {
                ImageList[i].raycastTarget = i == 0;
            }
        }
        else
        {

            if (ContinueBool)
            {
                if (GameManager.Pref("Has Started", 0, true) == 1)
                {
                    gameObject.SetActive(true);
                    ButtonText.text = "Continue Day " + (int)GameManager.Pref("Day", 0, true);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }

            if (VolumeBool || MusicBool)
            {
                //transform.GetChild(0).GetComponent<TMP_Text>().text = "%" + ((int)(PlayerPrefs.GetFloat(ReturnSFXManager().AudioName) * 100)) + " " + ReturnSFXManager().AudioName;


                ButtonText.text = (VolumeBool ? "Sound " : "Music ") + (PlayerPrefs.GetFloat(ReturnSFXManager().AudioName) * 100).ToString("N0") + "%";

                // transform.GetChild(2).GetComponent<Slider>().onValueChanged += OnPointerClick(null);
            }

            TextString = ButtonText.text;
        }
    }

    //checks to see if the pointer has exited the button
    public void OnPointerExit(PointerEventData eventData)
    {
        //Pause.global.SelectedList[Pause.global.ReturnIndex()] = -1;
        //ChangeColourVoid(new Color(164.0f / 255.0f, 164.0f / 255.0f, 164.0f / 255.0f));

    }

    //checks to see if the pointer has entered the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        //GameManager.global.SoundManager.PlaySound(GameManager.global.MenuClick1Sound);
        Buttons buttons = GetComponentInParent<Buttons>();
        buttons.MenuList[buttons.ReturnIndex()] = transform.GetSiblingIndex();

        CheckUpgrade();
        //ChangeColourVoid(Color.white);
    }

    void CheckUpgrade()
    {
        if (UpgradeInt != 0)
        {
            PlayerController.global.UpdateResourceHolder(UpgradeInt);

            PlayerController.global.OpenResourceHolder(true);
        }
    }

    //checks to see if the mouse was clicked ontop of the button
    public void OnPointerDown(PointerEventData eventData)
    {
        GetComponentInParent<Buttons>().pressingDown = true;
        SelectVoid();
    }


    //checks to see if the mouse was let go ontop of the button
    public void OnPointerUp(PointerEventData eventData)
    {
        //  OnPointerEnter(eventData);
        GetComponentInParent<Buttons>().pressingDown = false;
        //  SelectVoid();



    }

    public void SelectVoid()
    {
        if (ResumeBool)
        {
            PlayerController.global.PauseVoid(false);
        }

        if (UpgradeInt != 0)
        {
            CheckUpgrade();

            if (PlayerController.global.CheckSufficientResources())
            {
                if (UpgradeInt > 0)
                {
                    GameManager.PlayAnimation(GetComponent<Animation>(), "TierUpgrade");

                    PlayerModeHandler.global.UpdateTier(GetComponent<TurretStats>());
                    /*
                    Defence defence = PlayerModeHandler.global.SelectedTurret.GetComponent<Defence>();

                    if (defence && defence.ModelHolder.childCount > defence.CurrentLevel + 1)
                    {
                        defence.CurrentLevel++;
                        defence.ReturnAnimator();
                    }
                    */
                }

                if (UpgradeInt == -1)
                {
                    PlayerModeHandler.global.ReturnVFXBuilding(PlayerModeHandler.global.SelectedTurret.transform);
                    PlayerModeHandler.global.SelectedTurret.health = PlayerModeHandler.global.SelectedTurret.maxHealth;
                    PlayerModeHandler.global.SelectedTurret.TakeDamage(0);
                }

                if (UpgradeInt == -2)
                {
                    PlayerModeHandler.global.ReturnVFXBuilding(PlayerModeHandler.global.SelectedTurret.transform);
                    PlayerModeHandler.global.SelectedTurret.DestroyBuilding();
                }
            }
            else
            {
                GameManager.PlayAnimation(GetComponent<Animation>(), "Cannot Click");
            }
        }



        if (BeginingBool || PlayBool)
        {
            GameManager.Pref("Has Started", 0, false); //restart game
            List<ButtonMechanics> buttons = GameManager.FindComponent<ButtonMechanics>(transform.root);
            for (int i = 0; i < buttons.Count; i++) //stops continue disappearing when starting new game
            {
                buttons[i].enabled = false;
            }

            Time.timeScale = 1f;
            GameManager.global.NextScene(1);
        }

        if (MaxResourcesBool)
        {
            for (int i = 0; i < LevelManager.global.StoneTierList.Count; i++)
            {
                LevelManager.global.WoodTierList[i].ResourceAmount = 9999;
                LevelManager.global.StoneTierList[i].ResourceAmount = 9999;
            }

            PlayerController.global.UpdateResourceHolder();
        }

        if (TerrainTeleportInt != -1)
        {
            Vector3 posVector = LevelManager.global.terrainList[TerrainTeleportInt].transform.position + new Vector3(50, 2, 50);
            posVector.y = 2;

            if (TerrainTeleportInt == 4)
            {
                posVector += new Vector3(5, 0, 150);
            }

            PlayerController.global.TeleportPlayer(posVector, false);
        }

        if (SaveBool)
        {
            GameManager.global.DataSetVoid(false);
        }

        if (SecondMenuBool || BackBool)
        {
            GetComponentInParent<Buttons>().ChangeMenu(BackBool ? 0 : 1);
        }

        if (ContinueBool || RestartBool)
        {
            Time.timeScale = 1f;
            GameManager.global.NextScene(1);
        }

        if (MenuBool)
        {
            Time.timeScale = 1f;
            GameManager.global.NextScene(0);
        }

        if (VolumeBool || MusicBool)
        {

            PlayerPrefs.SetFloat(ReturnSFXManager().AudioName, PlayerPrefs.GetFloat(ReturnSFXManager().AudioName) + 0.1f);

            if (PlayerPrefs.GetFloat(ReturnSFXManager().AudioName) > 1.01f)
            {
                PlayerPrefs.SetFloat(ReturnSFXManager().AudioName, 0);
            }

            Start();

            ReturnSFXManager().RefreshAudioVolumes();

        }

        if (FullScreenBool)
        {
            Screen.fullScreen = !Screen.fullScreen;

            if (Screen.fullScreen)
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;

#if UNITY_EDITOR
            ButtonText.text = "Won't work\nin editor";
#endif
        }

        if (AreYouSureBool)
        {
            ToggleBool = !ToggleBool;

            if (ToggleBool)
            {
                ButtonText.text = "Are You Sure?";
                return;
            }
        }

        if (ResetBool)
        {
            PlayerPrefs.DeleteAll();

            GameManager.global.NextScene(0);
            GameManager.global.transform.SetParent(transform);

            //  GameManager.PlayAnimation(GetComponent<Animation>(), "Sign Accepted");
        }

        if (ExitBool)
        {
            Application.Quit();
            GameManager.global.NextScene(0);
            // GameManager.PlayAnimation(GetComponent<Animation>(), "Sign Accepted");
        }
    }


    SFXManager ReturnSFXManager()
    {
        return VolumeBool ? GameManager.global.SoundManager : GameManager.global.MusicManager;
    }
}
