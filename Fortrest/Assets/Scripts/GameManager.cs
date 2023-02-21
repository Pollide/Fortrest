/*********************************************************************
Bachelor of Software Engineering
Media Design School
Auckland
New Zealand
(c) 2023 Media Design School
File Name : GameManager.cs
Description : holds useful functions and global variables that are accessed ingame
Author :  Allister Hamilton
Mail : allister.hamilton @mds.ac.nz
**************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager global; //can be accessed anywhere

    public SFXManager SoundManager;
    public SFXManager MusicManager;

    public AudioClip MenuMusic;
    public AudioClip GameMusic;
    public AudioClip PauseMusic;

    public AudioClip Click;
    public AudioClip HoverClick;

    void Awake()
    {
        if (global)
        {
            Destroy(gameObject);
        }
        else
        {
            global = this;
            DontDestroyOnLoad(gameObject);
            GameManager.global.MusicManager.PlayMusic(MenuMusic);
            if (PlayerPrefs.GetInt("Game Started") == 0)
            {
                PlayerPrefs.SetInt("Game Started", 1);

                PlayerPrefs.SetFloat("Music", 0.65f);
                PlayerPrefs.SetFloat("Sound", 0.9f);
                PlayerPrefs.SetInt("Level Unlocked" + 1, 1);
            }

            int quickLoadInt = PlayerPrefs.GetInt("Quick Load");

            if (PlayerPrefs.GetInt("Quick Load") > 0)
            {
                PlayerPrefs.SetInt("Quick Load", 0);
                NextScene(quickLoadInt);
            }
        }
    }

    public static Vector3 ColorToVector(Color color)
    {
        return new Vector3(color.r, color.g, color.b);
    }

    public static bool VectorCompare(Vector3 leftVector, Vector3 rightVector, bool larger, bool smaller, float closeFloat = 0.01f)
    {
        bool xBool = ValueCompare(leftVector.x, rightVector.x, larger, smaller, closeFloat);
        bool yBool = ValueCompare(leftVector.y, rightVector.y, larger, smaller, closeFloat);
        bool zBool = ValueCompare(leftVector.z, rightVector.z, larger, smaller, closeFloat);

        return xBool && yBool && zBool;
    }


    public static bool ValueCompare(float left, float right, bool larger, bool smaller = false, float closeFloat = 0.01f)
    {
        //	Debug.Log(left + " - " + closeFloat + " <= " + right + " --- " + (left - closeFloat <= right));

        //checks opposite, if smaller is false, then it assumes larger is active.

        //      not smaller  so  larger                                  not larger   so   smaller

        bool overBool = left + closeFloat >= right;
        bool underBool = left - closeFloat <= right;

        bool returnBool = true;

        if (larger)
        {
            returnBool = returnBool && overBool;
        }

        if (smaller || !larger)
        {
            returnBool = returnBool && underBool;
        }

        return returnBool;
    }


    public static float ReturnThresholds(float valueInt, float maxValue, float minValue = 0, bool wrap = true)
    {
        //will run this once or if i = -1
        for (int i = 0; i < 1; i++)
        {
            if (valueInt < minValue)
            {
                if (wrap)
                {
                    //sets the valueInt back to the min as it went above the max
                    valueInt = maxValue + (minValue + valueInt) + 1;
                    i = -1; //loop runs again
                }
                else
                {
                    valueInt = minValue;
                }

                continue; //continues loop
            }

            if (valueInt > maxValue)
            {
                if (wrap)
                {
                    //sets the valueInt to the max as it went below the min
                    valueInt = minValue + (valueInt - maxValue) - 1;
                    i = -1;
                }
                else
                {
                    valueInt = maxValue;
                }

                continue; //continues loop
            }
        }

        return valueInt; //return the valueInt with new changes
    }

    //searches through childs infinetly and finds the component requested (T is not assigned until the function is called)
    public static List<T> FindComponent<T>(Transform searching, List<T> componentList = default) where T : Component //T is generic type
    {
        if (componentList == default)
        {
            componentList = new List<T>();
            Component componentParent = searching.GetComponent(typeof(T));
            if (componentParent)
            {
                componentList.Add((T)componentParent);
            }
        }

        for (int i = 0; i < searching.childCount; i++)
        {
            Transform searched = searching.GetChild(i); //active transfoorm
            if (searched.childCount > 0)
                componentList = FindComponent(searching.GetChild(i), componentList); //next depth  
            Component captured = searched.GetComponent(typeof(T));
            if (captured)
            {
                componentList.Add((T)captured);
            }
        }

        return componentList; //returns the list
    }

    //unity uses bit shifting to properly detect what layers should be masked, if the array is null, it will only see the "Default" layer.
    public static int ReturnBitShift(string[] layerNameArray = null)
    {
        if (layerNameArray == null)
        {
            layerNameArray = new string[] { "Default" };
        }

        int layerMaskInt = 0;

        for (int i = 0; i < layerNameArray.Length; i++)
        {
            layerMaskInt = layerMaskInt | 1 << LayerMask.NameToLayer(layerNameArray[i]);
        }

        return layerMaskInt;
    }

    //makes a smooth scene transition
    public void NextScene(int index)
    {
        StartCoroutine(ChangeSceneIEnumerator(index));

    }

    //this function manages the anim component and plays / reverses anim
    public AnimationState PlayAnimation(Animation anim, string nameClip = "", bool straight = true, bool quick = false)
    {
        if (anim) //checks exists
        {
            if (anim.GetClip(nameClip) || anim.clip) //exists?
            {
                if (!anim.GetClip(nameClip) && anim.clip) //exists?
                    nameClip = anim.clip.name; //default

                if (straight) //go reverse if false
                {
                    if (quick)
                    {
                        anim[nameClip].time = anim[nameClip].length; //jumps to end
                    }

                    anim[nameClip].speed = 1; //speed is forward/straight
                }
                else
                {
                    if (anim[nameClip].time == 0) //animation needs to jump to end
                    {
                        anim[nameClip].time = anim[nameClip].length;
                    }

                    if (quick)
                        anim[nameClip].time = 0;

                    anim[nameClip].speed = -1; //put it in reverse!
                }

                anim.Play(nameClip);
                // Debug.Log(nameClip);
                return anim[nameClip]; //the state can provide the time, length etc
            }
        } //end of checking anim

        Debug.LogWarning("Could not find animation component");
        return new AnimationState(); //something default, will likely return error that needs to be corrected anyways
    }

    public void CameraEnableVoid(Camera cameraEnable, bool enabled)
    {

        cameraEnable.enabled = true;
        cameraEnable.GetComponent<AudioListener>().enabled = enabled;
    }

    public static void ChangeAnimationLayers(Animation animation)
    {
        if (animation)
        {
            int indexInt = 0;
            foreach (AnimationState clip in animation)
            {
                animation[clip.name].layer = indexInt;
                animation[clip.name].speed = 1;
                indexInt += 1;
            }
        }
    }


    //waits for loading animation to complete
    public IEnumerator ChangeSceneIEnumerator(int index)
    {
        AnimationState state = PlayAnimation(GetComponent<Animation>(), "Load In");

        //  SoundManager.PlaySound(GameManager.global.LoadingSound);


        /*
        yield return new WaitUntil(() => menuBool ? MainMenu.global : SceneMechanics.global);

        Camera sceneCamera = menuBool ? MainMenu.global.SceneCamera : SceneMechanics.global.SceneCamera;
        Camera oppositeCamera = !menuBool ? MainMenu.global.SceneCamera : SceneMechanics.global.SceneCamera;

        CameraEnableVoid(sceneCamera, true);
        CameraEnableVoid(oppositeCamera, false);

        PlayAnimation(sceneCamera.GetComponent<Animation>(), "Fade To");
        PlayAnimation(oppositeCamera.GetComponent<Animation>(), "Fade From");
                SceneManager.LoadScene(menuBool ? 0 : 1, LoadSceneMode.Additive);


        */
        GameManager.global.MusicManager.PlayMusic(index == 2 ? GameManager.global.GameMusic : GameManager.global.MenuMusic);

        yield return new WaitUntil(() => !state.enabled);
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        yield return new WaitUntil(() => operation.isDone);
        state = PlayAnimation(GetComponent<Animation>(), "Load Out");
    }
}
