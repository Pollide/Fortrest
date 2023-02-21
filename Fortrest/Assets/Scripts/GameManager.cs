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

    public SFXManager SoundManager; //manages all sound effects
    public SFXManager MusicManager; //manages all music

    public AudioClip MenuMusic; //the music that plays in the menu scene
    public AudioClip GameMusic;  //the music that plays in the game scene
    public AudioClip PauseMusic;  //the music that plays when paused

    public AudioClip ClickAudioClip; //the sfx that plays when the mouse presses a button
    public AudioClip HoverAudioClip; //the sfx that plays when the mouse hovers over a button

    //runs on the frame it was awake on
    void Awake()
    {
        //checks if itself exists, as they can only be one
        if (global)
        {
            //destroys the duplicate
            Destroy(gameObject);
        }
        else
        {
            //itself doesnt exist so set it
            global = this;

            //keeps it between scenes
            DontDestroyOnLoad(gameObject);

            //plays the menu music
            GameManager.global.MusicManager.PlayMusic(MenuMusic);

            //this checks if it is the first time playing the game. It wont run again
            if (PlayerPrefs.GetInt("First Time") == 0)
            {
                PlayerPrefs.SetInt("First Time", 1);

                PlayerPrefs.SetFloat("Music", 0.65f); //sets the music level
                PlayerPrefs.SetFloat("Sound", 0.9f); //sets the sound volume
            }

            //quick load is an editor feature that I added to help other peers when testing projects.
            //Basically when your in another scene, the pref will save then redirect to the menu scene.
            //Then will check the saved pref and redirect you back to the scene you were in. The reason this is done
            //is to get the GameManager into the game scene, as it exists in the menu scene to begin with.

            int quickLoadInt = PlayerPrefs.GetInt("Quick Load");

            if (PlayerPrefs.GetInt("Quick Load") > 0)
            {
                PlayAnimation(GetComponent<Animation>(), "Load In", true, true); //forces the load animation to start on
                PlayerPrefs.SetInt("Quick Load", 0); //now set it to zero as no need for the feature to exist until next time a peer runs another scene
                NextScene(quickLoadInt); //go to that next scene
            }
        }
    }

    //this function will compare values and check it is in a certain range, and will correct itself it too far over
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
                    i = -1; //setting i as -1 means that the next loop will ++ and make it i = 0, which is default
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
        //if component list is default, then it will create one
        if (componentList == default)
        {
            componentList = new List<T>();
            Component componentParent = searching.GetComponent(typeof(T)); //T is a generic type variable which can be anything. This makes it very robust with handling multiple components
            
            if (componentParent) //checks if the parent has the component, then add it. Adding the parent helps with continuity.
            {
                componentList.Add((T)componentParent);//to add it to the list, must convert Component to T by calling (T)
            }
        }

        //for loop runs through all children of the 'searching' parent
        for (int i = 0; i < searching.childCount; i++)
        {
            //the child that is actively being searched
            Transform searched = searching.GetChild(i); //active transfoorm

            //if the child has more children, then it will 
            if (searched.childCount > 0)
                componentList = FindComponent(searching.GetChild(i), componentList); //next depth  
          
            //found the component
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
        //array
        if (layerNameArray == null)
        {
            layerNameArray = new string[] { "Default" }; 
        }

        int layerMaskInt = 0;

        for (int i = 0; i < layerNameArray.Length; i++)
        {
            //bit shifting layers
            layerMaskInt = layerMaskInt | 1 << LayerMask.NameToLayer(layerNameArray[i]);
        }

        return layerMaskInt;
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

    //changes all layers of an animation so multiple can play at the same time
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


    //makes a smooth scene transition
    public void NextScene(int index)
    {
        //run a coroutine by calling it in GameManager ensures that if the object wont be destroyed and break the coroutine
        StartCoroutine(ChangeSceneIEnumerator(index));

    }

    //waits for loading animation to complete
    public IEnumerator ChangeSceneIEnumerator(int index)
    {
        AnimationState state = PlayAnimation(GetComponent<Animation>(), "Load In");

        //switches between music
        GameManager.global.MusicManager.PlayMusic(index == 2 ? GameManager.global.GameMusic : GameManager.global.MenuMusic);

        //wait until the animation is done
        yield return new WaitUntil(() => !state.enabled);

        //gets the scene index and loads it async
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);

        //wait until the scene has finished loading
        yield return new WaitUntil(() => operation.isDone);

        //play the outro loading animation
        state = PlayAnimation(GetComponent<Animation>(), "Load Out");
    }
}
