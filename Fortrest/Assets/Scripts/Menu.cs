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

    bool GoForwardBool;
    bool GoHorizontalBool;

    bool SettingsSelectedBool;

    private void Awake()
    {
        global = this; //set the only menu to this. No need to destroy any old ones as the menu isnt under DoNotDestroy
        GameManager.ChangeAnimationLayers(CameraAnimation);

        GameManager.ChangeAnimationLayers(WelcomeSignAnimation);
        GameManager.ChangeAnimationLayers(ExitSignAnimation);
        GameManager.ChangeAnimationLayers(SettingsSignAnimation);
        GameManager.ChangeAnimationLayers(LevelsSignAnimation);

        GameManager.PlayAnimation(CameraAnimation, "Initial Menu");

        StartCoroutine(InitalMenuIEnumerator());
    }

    IEnumerator InitalMenuIEnumerator()
    {
        yield return new WaitUntil(() => !CameraAnimation.isPlaying);

        SignAnimationVoid(WelcomeSignAnimation);

        yield return new WaitUntil(() => InputCheck());

        SignAnimationVoid(WelcomeSignAnimation, false);

        if (GoForwardBool)
            StartCoroutine(LevelMenuIEnumerator());
        else
            StartCoroutine(ExitMenuIEnumerator());

    }

    IEnumerator LevelMenuIEnumerator()
    {
        GameManager.PlayAnimation(CameraAnimation, "Play Menu");
        yield return new WaitUntil(() => !CameraAnimation.isPlaying);

        SignAnimationVoid(LevelsSignAnimation);
        SettingsSelectedBool = false;

        do
        {
            yield return new WaitUntil(() => InputCheck(true));

            if (GoHorizontalBool)
            {
                SettingsSelectedBool = !SettingsSelectedBool;
                SignAnimationVoid(SettingsSelectedBool ? SettingsSignAnimation : LevelsSignAnimation);
                SignAnimationVoid(SettingsSelectedBool ? LevelsSignAnimation : SettingsSignAnimation, false);
            }
            yield return 0;
        }
        while (GoHorizontalBool);

        if (GoForwardBool)
        {
            if (SettingsSelectedBool)
            {
                SettingsSignAnimation.transform.GetChild(2).GetComponent<TMP_Text>().text = "Nah\nscrew you";

                yield return new WaitUntil(() => InputCheck());
            }
            else
            {
                GameManager.global.NextScene(1);
                yield break;
            }
        }

        if (SettingsSelectedBool)
            SignAnimationVoid(SettingsSignAnimation, false);
        else
            SignAnimationVoid(LevelsSignAnimation, false);

        GameManager.PlayAnimation(CameraAnimation, "Play Menu", false);
        StartCoroutine(InitalMenuIEnumerator());

    }


    IEnumerator ExitMenuIEnumerator()
    {
        GameManager.PlayAnimation(CameraAnimation, "Exit Menu");

        yield return new WaitUntil(() => !CameraAnimation.isPlaying);

        SignAnimationVoid(ExitSignAnimation);

        yield return new WaitUntil(() => InputCheck());

        if (GoForwardBool)
        {
            Application.Quit();
            GameManager.global.NextScene(0);
        }
        else
        {
            SignAnimationVoid(ExitSignAnimation, false);
            GameManager.PlayAnimation(CameraAnimation, "Exit Menu", false);
            StartCoroutine(InitalMenuIEnumerator());
        }
    }

    bool InputCheck(bool horizontalBool = false)
    {
        GoForwardBool = false;
        GoHorizontalBool = false;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.W))
        {
            GoForwardBool = true;
            return true;
        }

        if (horizontalBool && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)))
        {
            GoHorizontalBool = true;
            return true;
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            return true;
        }

        return false;
    }

    void SignAnimationVoid(Animation signAnimation, bool forward = true)
    {
        if (forward)
            GameManager.PlayAnimation(signAnimation, "Sign Selected", forward);

        if (forward)
            signAnimation.Play("Sign Loop");
        else
        {
            GameManager.PlayAnimation(signAnimation, "Sign Loop", false, true);
            signAnimation.Stop("Sign Loop");
            // GameManager.PlayAnimation(signAnimation, "Sign Loop", false, true);
        }

    }
}
