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

    public GameObject OptionsCanvas;
    public GameObject MenuCanvas;
    public Animation CameraAnimation;

    public Animation WelcomeSignAnimation;
    public Animation ExitSignAnimation;
    public Animation SettingsSignAnimation;
    public Animation LevelsSignAnimation;
    public Color TextColor;
    public TMP_Text PlayText;
    bool GoForwardBool;
    int GoHorizontalInt;

    bool SettingsSelectedBool;

    private void Awake()
    {
        global = this; //set the only menu to this. No need to destroy any old ones as the menu isnt under DoNotDestroy

        GameManager.PlayAnimation(CameraAnimation, "Initial Menu");
        TextColor = SettingsSignAnimation.transform.GetChild(2).GetComponent<TMP_Text>().color;
        StartCoroutine(InitalMenuIEnumerator());

        if ((int)GameManager.Pref("Game Started", 0, true) == 1)
        {
            PlayText.text = "Continue\n" + "Day " + (int)GameManager.Pref("Day", 0, true);
        }
        else
        {
            PlayText.text = "New Game";
        }
    }

    private void Update()
    {
        PlayerModeHandler.SetMouseActive(false);
    }
    IEnumerator InitalMenuIEnumerator()
    {
        yield return new WaitUntil(() => !CameraAnimation.isPlaying);

        SignAnimationVoid(WelcomeSignAnimation);

        yield return new WaitUntil(() => InputCheck() && GoHorizontalInt != 0);

        SignAnimationVoid(WelcomeSignAnimation, false);

        if (GoHorizontalInt > 0)
            StartCoroutine(LevelMenuIEnumerator());
        else
            StartCoroutine(ExitMenuIEnumerator());

    }

    IEnumerator LevelMenuIEnumerator(bool play = true)
    {
        if (play)
        {
            GameManager.PlayAnimation(CameraAnimation, "Play Menu");
        }
        else
        {
            GameManager.PlayAnimation(CameraAnimation, "Level To Exit Menu", false);
        }
        yield return new WaitUntil(() => !CameraAnimation.isPlaying);

        SignAnimationVoid(LevelsSignAnimation);
        SettingsSelectedBool = false;

        do
        {
            yield return new WaitUntil(() => InputCheck());

            if (GoHorizontalInt != 0)
            {
                SettingsSelectedBool = !SettingsSelectedBool;
                SignAnimationVoid(SettingsSelectedBool ? SettingsSignAnimation : LevelsSignAnimation);
                SignAnimationVoid(SettingsSelectedBool ? LevelsSignAnimation : SettingsSignAnimation, false);
            }
            yield return 0;
        }
        while (GoHorizontalInt == 0 || SettingsSelectedBool ? GoHorizontalInt == 1 : GoHorizontalInt == -1);

        if (GoForwardBool)
        {
            if (SettingsSelectedBool)
            {
                SignAnimationVoid(SettingsSignAnimation, true);
                SettingsSignAnimation.transform.GetChild(2).GetComponent<TMP_Text>().text = "AGAIN TO CONFIRM"; //nah screw you

                yield return new WaitUntil(() => InputCheck());

                if (GoHorizontalInt == 0)
                {
                    SignAnimationVoid(SettingsSignAnimation, true, true);

                    PlayerPrefs.DeleteAll();
                    GameManager.global.NextScene(0);
                    yield break;
                }
            }
            else
            {
                SignAnimationVoid(LevelsSignAnimation, true, true);
                GameManager.global.NextScene(1);
                yield break;
            }
        }



        if (SettingsSelectedBool)
            SignAnimationVoid(SettingsSignAnimation, false);
        else
            SignAnimationVoid(LevelsSignAnimation, false);




        if (GoHorizontalInt > 0)
        {
            StartCoroutine(ExitMenuIEnumerator("Level To Exit Menu"));
        }
        else
        {
            GameManager.PlayAnimation(CameraAnimation, "Play Menu", false);
            StartCoroutine(InitalMenuIEnumerator());
        }
    }

    IEnumerator ExitMenuIEnumerator(string animation = "Exit Menu")
    {
        GameManager.PlayAnimation(CameraAnimation, animation);

        yield return new WaitUntil(() => !CameraAnimation.isPlaying);

        SignAnimationVoid(ExitSignAnimation);

        yield return new WaitUntil(() => InputCheck());

        if (GoForwardBool)
        {
            SignAnimationVoid(ExitSignAnimation, true, true);
            Application.Quit();
            GameManager.global.NextScene(0);
            yield break;
        }
        else
        {
            SignAnimationVoid(ExitSignAnimation, false);

            if (GoHorizontalInt > 0)
            {
                StartCoroutine(LevelMenuIEnumerator(false));
            }
            else
            {
                GameManager.PlayAnimation(CameraAnimation, "Exit Menu", false);
                StartCoroutine(InitalMenuIEnumerator());
            }
        }
    }

    bool InputCheck()
    {
        GoForwardBool = false;
        GoHorizontalInt = 0;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.W))
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.MenuClick1Sound);
            GoForwardBool = true;
            return true;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.MenuSwooshSound);
            GoHorizontalInt = -1;
            return true;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.MenuSwooshSound);
            GoHorizontalInt = 1;
            return true;
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            return true;
        }

        return false;
    }

    void SignAnimationVoid(Animation signAnimation, bool forward = true, bool accepted = false)
    {
        if (forward)
            GameManager.PlayAnimation(signAnimation, accepted ? "Sign Accepted" : "Sign Selected", forward);

        if (!accepted)
        {

            if (forward)
                signAnimation.Play("Sign Loop");
            else
            {
                // GameManager.PlayAnimation(signAnimation, "Sign Loop", false, true);
                signAnimation.Stop("Sign Loop");
                SettingsSignAnimation.transform.GetChild(2).GetComponent<TMP_Text>().color = TextColor;
                // GameManager.PlayAnimation(signAnimation, "Sign Loop", false, true);
            }
        }
    }
}
