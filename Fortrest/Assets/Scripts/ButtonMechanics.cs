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
    public bool KeyTipsBool;
    [Header("Build")]
    public int UpgradeInt;
    public bool CloseTurretMenuBool;
    public int TurretSelectInt;
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
    Buttons buttons;

    public void Start()
    {
        buttons = GetComponentInParent<Buttons>();

        if (!buttons)
            return;

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
                if (GameManager.Pref("Game Has Begun", 0, true) == 1)
                {
                    gameObject.SetActive(true);
                    ButtonText.text = "Continue Day " + (int)GameManager.Pref("Day", 0, true);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }

            if (KeyTipsBool)
            {
                ButtonText.text = "Key Tips: " + (GameManager.Pref("Key Tips", 0, true) == 0 ? "ON" : "OFF");
            }

            if (VolumeBool || MusicBool)
            {
                //transform.GetChild(0).GetComponent<TMP_Text>().text = "%" + ((int)(PlayerPrefs.GetFloat(ReturnSFXManager().AudioName) * 100)) + " " + ReturnSFXManager().AudioName;


                ButtonText.text = (VolumeBool ? "Sound " : "Music ") + (PlayerPrefs.GetFloat(ReturnSFXManager().AudioName) * 100).ToString("N0") + "%";

                // transform.GetChild(2).GetComponent<Slider>().onValueChanged += OnPointerClick(null);
            }
        }

        if (TerrainTeleportInt != -1)
        {
            GetComponent<Image>().color = LevelManager.global.terrainDataList[TerrainTeleportInt].indicatorColor;
        }
    }

    //checks to see if the pointer has exited the button
    public void OnPointerExit(PointerEventData eventData)
    {
        //Pause.global.SelectedList[Pause.global.ReturnIndex()] = -1;
        //ChangeColourVoid(new Color(164.0f / 255.0f, 164.0f / 255.0f, 164.0f / 255.0f));
        CheckUpgrade(false);
    }

    //checks to see if the pointer has entered the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        //GameManager.global.SoundManager.PlaySound(GameManager.global.MenuClick1Sound);
        if (buttons)
            buttons.MenuList[buttons.ReturnIndex()] = transform.GetSiblingIndex();

        CheckUpgrade();
        //ChangeColourVoid(Color.white);
    }

    void CheckUpgrade(bool open = true)
    {
        if (UpgradeInt != 0 && PlayerModeHandler.global.SelectedTurret)
        {
            BuildType buildType = BuildType.None;

            if (UpgradeInt < 0 && open)
            {
                if (PlayerModeHandler.global.SelectedTurret.buildingObject == Building.BuildingType.Ballista)
                    buildType = BuildType.Turret;

                if (PlayerModeHandler.global.SelectedTurret.buildingObject == Building.BuildingType.Cannon)
                    buildType = BuildType.Cannon;

                if (PlayerModeHandler.global.SelectedTurret.buildingObject == Building.BuildingType.Slow)
                    buildType = BuildType.Slow;

                if (PlayerModeHandler.global.SelectedTurret.buildingObject == Building.BuildingType.Scatter)
                    buildType = BuildType.Scatter;
            }

            PlayerController.global.UpdateResourceHolder(new PlayerController.ResourceData() { buildType = buildType, upgradeTypeInt = (open ? UpgradeInt : 0) }); //hides cost UI if not hover
        }
    }

    //checks to see if the mouse was clicked ontop of the button
    public void OnPointerDown(PointerEventData eventData)
    {
        if (buttons)
            GetComponentInParent<Buttons>().pressingDown = true;

        SelectVoid();
    }


    //checks to see if the mouse was let go ontop of the button
    public void OnPointerUp(PointerEventData eventData)
    {
        //  OnPointerEnter(eventData);
        if (buttons)
            GetComponentInParent<Buttons>().pressingDown = false;
        //  SelectVoid();



    }

    public void SelectVoid()
    {
        // if(SceneManager.GetActiveScene().buildIndex == 0)
        // {
        GameManager.global.SoundManager.PlaySound(GameManager.global.MenuClickSound);
        //  }

        if (ResumeBool)
        {
            PlayerController.global.PauseVoid(false);
        }

        if (UpgradeInt > 0)
        {
            CheckUpgrade();
            PlayerModeHandler.global.UpdateTier(GetComponent<TurretStats>());
        }

        if (TurretSelectInt == 1)
        {
            PlayerModeHandler.global.SwitchBuildTypeTurret();
        }

        if (TurretSelectInt == 2)
        {
            PlayerModeHandler.global.SwitchBuildTypeCannon();
        }

        if (TurretSelectInt == 3)
        {
            PlayerModeHandler.global.SwitchBuildTypeSlow();
        }

        if (TurretSelectInt == 4)
        {
            PlayerModeHandler.global.SwitchBuildTypeScatter();
        }


        if (KeyTipsBool)
        {
            GameManager.Pref("Key Tips", GameManager.Pref("Key Tips", 0, true) == 1 ? 0 : 1, false);
            Start();
        }

        if (CloseTurretMenuBool)
        {
            PlayerModeHandler.global.TurretMenuSet(false);
        }

        if (UpgradeInt < 0)
        {
            CheckUpgrade();

            if (PlayerController.global.CheckSufficientResources())
            {

                if (UpgradeInt == -1)
                {
                    PlayerModeHandler.global.ReturnVFXBuilding(PlayerModeHandler.global.SelectedTurret.transform);
                    PlayerModeHandler.global.SelectedTurret.TakeDamage(-PlayerModeHandler.global.SelectedTurret.GetComponent<Building>().ReturnRepair()); //minus will actually increase its health
                }

                if (UpgradeInt == -2)
                {
                    PlayerModeHandler.global.ReturnVFXBuilding(PlayerModeHandler.global.SelectedTurret.transform);
                    PlayerModeHandler.global.SelectedTurret.DestroyBuilding();
                    PlayerModeHandler.global.TurretMenuSet(false);
                }
            }
            else
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.CantPlaceSound);
                GameManager.PlayAnimation(GetComponent<Animation>(), "Cannot Click");
            }
        }



        if (BeginingBool || PlayBool)
        {
            GameManager.Pref("Prologue", 0, false);
            GameManager.Pref("Game Has Begun", 0, false); //restart game
                                                          // List<ButtonMechanics> buttons = GameManager.FindComponent<ButtonMechanics>(transform.root);
            GetComponentInParent<Buttons>().enabled = false;
            /*
            for (int i = 0; i < buttons.Count; i++) //stops continue disappearing when starting new game
            {
                buttons[i].enabled = false;
            }
            */

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
        }

        if (TerrainTeleportInt != -1)
        {
            Vector3 posVector = LevelManager.global.terrainDataList[TerrainTeleportInt].terrain.transform.position + new Vector3(50, 2, 50);
            posVector.y = 2;

            if (TerrainTeleportInt == 3)
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
            else
                Screen.fullScreenMode = FullScreenMode.Windowed;
#if UNITY_EDITOR
            ButtonText.text = "Won't work in editor";
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
