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
using UnityEngine.EventSystems;
public class GameManager : MonoBehaviour
{
    public static GameManager global; //can be accessed anywhere

    public SFXManager SoundManager; //manages all sound effects
    public SFXManager MusicManager; //manages all music

    public AudioClip MenuMusic; //the music that plays in the menu scene
    public AudioClip GameMusic;  //the music that plays in the game scene
    public AudioClip PauseMusic;  //the music that plays when paused
    public AudioClip NightMusic;

    public AudioClip CollectSound; //the sfx that plays when the mouse presses a button
    public AudioClip MenuClick1Sound;
    public AudioClip MenuClick2Sound;
    public AudioClip Pickaxe1Sound;
    public AudioClip Pickaxe2Sound;
    public AudioClip Pickaxe3Sound;
    public AudioClip PlayerAttackSound;
    public AudioClip PlayerHitSound;
    public AudioClip PlayerJumpSound;
    public AudioClip Footstep1Sound;
    public AudioClip Footstep2Sound;
    public AudioClip SwordSwing1Sound;
    public AudioClip SwordSwing2Sound;
    public AudioClip SwordSwing3Sound;
    public AudioClip TreeChop1Sound;
    public AudioClip TreeChop2Sound;
    public AudioClip TreeChop3Sound;
    public AudioClip TurretPlaceSound;
    public AudioClip TurretShootSound;
    public AudioClip PlayerEatSound;
    public AudioClip CameraLockSound;
    public AudioClip MenuSwooshSound;
    public AudioClip SpeedButtonClickSound;
    public AudioClip BushSound;
    public AudioClip ModeChangeClickSound;
    public AudioClip HouseBuiltNoiseSound;
    public AudioClip HouseBuiltSound;
    public AudioClip SnoringSound;
    public AudioClip CantPlaceSound;
    public AudioClip WhistlingSound;

    public AudioClip CannonSound;
    public AudioClip EatingSound;
    public AudioClip HouseDestroySound;
    public AudioClip InventoryClickSound;
    public AudioClip NewDaySound;
    public AudioClip SlowSound;
    public AudioClip WaterSound;

    public bool CheatInfiniteBuilding;

    public bool DebugSaveBool;
    public bool DebugLoadBool;

    public AnimationClip PopupAnimation;

    public GamepadControls gamepadControls;

    private void Update()
    {
        if (DebugSaveBool)
        {
            DebugSaveBool = false;
            DataSetVoid(false);
        }

        if (DebugLoadBool)
        {
            DebugLoadBool = false;
            DataSetVoid(true);
        }
    }
    //runs on the frame it was awake on
    void Awake()
    {
        //checks if itself exists, as they can only be one

        if (PlayerController.global)
        {
            Destroy(PlayerController.global.transform.parent.gameObject); //no players in main menu
        }

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

            GameObject eventSystemGameObject = new GameObject().AddComponent<EventSystem>().gameObject;
            eventSystemGameObject.AddComponent<StandaloneInputModule>();
            DontDestroyOnLoad(eventSystemGameObject);
            eventSystemGameObject.name = "Event System";

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
                PlayAnimation(GetComponent<Animation>(), "Load In", true, true); //first bool skips this
                PlayerPrefs.SetInt("Quick Load", 0); //now set it to zero as no need for the feature to exist until next time a peer runs another scene
            }

            NextScene(quickLoadInt, quickLoadInt == 0); //go to that next scene
        }

    }

    /*
    private void Update()
    {

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            NextScene(SceneManager.GetActiveScene().buildIndex - 1);
        }

        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            NextScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
#endif
    }
    */

    //this function will compare values and check it is in a certain range, and will correct itself it too far over
    public static float ReturnThresholds(float valueInt, float maxValue, float minValue = 0, bool wrap = true)
    {

        if (minValue == maxValue)
            return valueInt;
        //will run this once or if i = -1

        int stackOverflow = 0;

        for (int i = 0; i < 1; i++)
        {
            stackOverflow++;

            if (stackOverflow > 100)
            {
                Debug.LogWarning("stack overflow");
                return valueInt;
            }
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
    public static AnimationState PlayAnimation(Animation anim, string nameClip = "", bool straight = true, bool quick = false)
    {
        if (anim) //checks exists
        {
            GameManager.ChangeAnimationLayers(anim);

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

    public static AnimatorStateInfo PlayAnimator(Animator animator, string clipNameString = "", bool forwardBool = true, bool instantBool = false)
    {
        if (animator)
        {
            foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clipNameString == "" || clip.name == clipNameString)
                {
                    if (forwardBool)
                    {
                        animator.StopPlayback();
                        animator.StartRecording(0);
                    }
                    else
                    {
                        animator.StopRecording();
                        animator.StartPlayback();
                    }

                    animator.speed = forwardBool ? 1 : -1;

                    animator.Play(clip.name, 0, forwardBool ? 0 : 1);

                    if (instantBool)
                    {
                        animator.Update(0);
                    }

                    return animator.GetCurrentAnimatorStateInfo(0);
                }
            }
        }

        Debug.LogWarning("No animator found");
        return new AnimatorStateInfo();
    }


    //changes all layers of an animation so multiple can play at the same time
    public static void ChangeAnimationLayers(Animation animation, bool playBool = false)
    {
        if (animation)
        {
            int indexInt = 0;
            foreach (AnimationState clip in animation)
            {
                animation[clip.name].layer = indexInt;
                animation[clip.name].speed = 1;
                indexInt += 1;

                if (playBool)
                    animation.Play(clip.name);
            }
        }
    }


    //makes a smooth scene transition
    public void NextScene(int index, bool first = false)
    {
        index = (int)ReturnThresholds(index, SceneManager.sceneCountInBuildSettings - 1);
        //run a coroutine by calling it in GameManager ensures that if the object wont be destroyed and break the coroutine
        StartCoroutine(ChangeSceneIEnumerator(index, first));

    }

    //waits for loading animation to complete
    public IEnumerator ChangeSceneIEnumerator(int index, bool first)
    {
        AnimationState state = PlayAnimation(GetComponent<Animation>(), "Load In", true, first);

        yield return 0; //gives a frame for sfx to load

        //switches between music
        GameManager.global.MusicManager.PlayMusic(index == 0 ? GameManager.global.MenuMusic : GameManager.global.GameMusic);

        //wait until the animation is done
        yield return new WaitUntil(() => !state || !state.enabled || first);

        //gets the scene index and loads it async
        if (!first)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(index);

            //wait until the scene has finished loading
            yield return new WaitUntil(() => operation.isDone);
        }


        //play the outro loading animation
        state = PlayAnimation(GetComponent<Animation>(), "Load Out");

        if (index == 1)
        {
            yield return 0; //gives a second for everything on Start to run

            if ((int)Pref("Game Started", 0, true) == 1)
                GameManager.global.DataSetVoid(true);
        }
    }

    public static GameObject ReturnResource(string resourceObject, Vector3 position, Quaternion rotation)
    {
        GameObject item = Instantiate(Resources.Load("Drops/" + resourceObject + " Drop"), position, rotation) as GameObject;
        item.GetComponent<InventoryItem>().resourceObject = resourceObject;

        return item;
    }

    void TierDataVoid(List<LevelManager.TierData> tierList, bool load)
    {
        for (int i = 0; i < tierList.Count; i++)
        {
            tierList[i].ResourceAmount = (int)Pref(tierList[i].ResourceName + i.ToString(), tierList[i].ResourceAmount, load);
        }
    }

    public void DataSetVoid(bool load)
    {
        if (!load)
        {
            Pref("Game Started", 1, false);
        }
        DataPositionVoid("Player", PlayerController.global.transform, load);
        PlayerController.global.playerHealth = (int)Pref("Player Health", PlayerController.global.playerHealth, load);
        PlayerController.global.playerEnergy = (int)Pref("Player Energy", PlayerController.global.playerEnergy, load);

        LevelManager.global.DaylightTimer = Pref("Daylight", LevelManager.global.DaylightTimer, load);
        LevelManager.global.day = (int)Pref("Day", LevelManager.global.day, load);
        LevelManager.global.enemyTimer = (int)Pref("Goblin Timer", LevelManager.global.enemyTimer, load);

        LevelManager.global.enemyThreshold = (int)Pref("Goblin Threshold", LevelManager.global.enemyThreshold, load);
        int itemSize = (int)Pref("Item Size", LevelManager.global.InventoryItemList.Count, load);

        for (int i = 0; i < itemSize; i++)
        {
            string original = load ? "" : LevelManager.global.InventoryItemList[i].GetComponent<InventoryItem>().resourceObject.ToString();
            string resourceObject = PrefString("Item Resource" + i, original, load);

            Transform resource = load ? ReturnResource(resourceObject, Vector3.zero, Quaternion.identity).transform : LevelManager.global.InventoryItemList[i].transform;

            // int collected = (int)Pref("Item Collected" + i, resource.GetComponent<InventoryItem>().CollectedBool ? 1 : 0, load);

            resource.GetComponent<InventoryItem>().resourceAmount = (int)Pref("Item Amount" + i, resource.GetComponent<InventoryItem>().resourceAmount, load);

            // if (load && collected == 1)
            //    resource.GetComponent<InventoryItem>().CollectVoid();

            DataPositionVoid("Item Position" + i, resource, load);
            DataEulerVoid("Item Euler" + i, resource, load);
        }

        TierDataVoid(LevelManager.global.WoodTierList, load);
        TierDataVoid(LevelManager.global.StoneTierList, load);

        for (int i = 0; i < LevelManager.global.BridgeList.Count; i++)
        {
            LevelManager.global.BridgeList[i].isBuilt = Pref("Bridge Built" + i, LevelManager.global.BridgeList[i].isBuilt ? 1 : 0, load) == 1;
        }

        LevelManager.ProcessEnemyList((enemy) =>
        {
            int i = LevelManager.global.EnemyList.IndexOf(enemy);
            enemy.health = Pref("Enemy Health" + i, enemy.health, load);
            DataPositionVoid("Enemy Position" + i, enemy.transform, load);
            DataEulerVoid("Enemy Euler" + i, enemy.transform, load);

            if (enemy.health <= 0 && load)
            {
                enemy.gameObject.SetActive(false);
            }

        });

        int turretSize = (int)Pref("Turret Size", 0, load); //also resets on save

        if (load)
        {
            for (int i = 0; i < turretSize; i++)
            {
                GameObject turret = Instantiate(PlayerModeHandler.global.turretPrefabs[(int)Pref("Turret Type" + i, 0, true)]);

                DataPositionVoid("Turret Position" + i, turret.transform, load);
                DataEulerVoid("Turret Euler" + i, turret.transform, load);
            }

            PlayerController.global.HealthRestore(0); //refresh
        }

        //DEFENSE
        LevelManager.ProcessBuildingList((building) =>
        {
            DataBuildingVoid(building, load);
        });

        //NATURE
        LevelManager.ProcessBuildingList((building) =>
        {
            DataBuildingVoid(building, load);
        }, true);
    }

    public static float Pref(string pref, float value, bool load)
    {
        if (load)
        {
            value = PlayerPrefs.GetFloat(pref);
        }
        else
        {
            PlayerPrefs.SetFloat(pref, value);
        }

        return value;
    }

    string PrefString(string pref, string value, bool load)
    {
        if (load)
        {
            value = PlayerPrefs.GetString(pref);
        }
        else
        {
            PlayerPrefs.SetString(pref, value);
        }

        return value;
    }


    public void DataBuildingVoid(Transform value, bool load)
    {
        Building building = value.GetComponent<Building>();

        if (building.resourceObject == Building.BuildingType.HouseNode)
        {
            building = building.transform.parent.GetComponent<Building>();
        }

        building.health = (int)Pref("Building Health" + LevelManager.global.ReturnIndex(value), building.health, load);

        if (load)
        {
            building.TakeDamage(0); //refreshes the bar

            if (building.health <= 0)
                building.DisableInvoke();
        }

        else if (building.resourceObject == Building.BuildingType.Defense)
        {
            int turretSize = (int)Pref("Turret Size", 0, true);

            for (int i = 0; i < PlayerModeHandler.global.turretPrefabs.Length; i++)
            {
                if (building.name.Contains(PlayerModeHandler.global.turretPrefabs[i].name))
                {
                    //Debug.Log(turretSize + " " + i + " " + PlayerModeHandler.global.turretPrefabs[i].name);
                    Pref("Turret Type" + turretSize, i, false);
                    break;
                }
            }

            DataPositionVoid("Turret Position" + turretSize, building.transform, false);
            DataEulerVoid("Turret Euler" + turretSize, building.transform, false);
            building.GetComponent<TurretShooting>().CurrentLevel = (int)Pref("Turret Level" + turretSize, building.GetComponent<TurretShooting>().CurrentLevel, load);
            Pref("Turret Size", turretSize + 1, false);
        }

    }
    void DataPositionVoid(string pref, Transform value, bool load)
    {
        /*
        if(value.GetComponent<Building>() && value.GetComponent<Building>().resourceObject == Building.BuildingType.House)
        {
            Debug.Log("mOVING");
        }
        */

        float x = Pref(pref + "x", value.position.x, load);
        float y = Pref(pref + "y", value.position.y, load);
        float z = Pref(pref + "z", value.position.z, load);

        value.position = new Vector3(x, y, z);
    }

    void DataEulerVoid(string pref, Transform value, bool load)
    {
        float x = Pref(pref + "x", value.eulerAngles.x, load);
        float y = Pref(pref + "y", value.eulerAngles.y, load);
        float z = Pref(pref + "z", value.eulerAngles.z, load);

        value.eulerAngles = new Vector3(x, y, z);
    }

    //Runs animations like UI popping
    public static void TemporaryAnimation(GameObject requestedGameObject, AnimationClip requestedAnimationClip, float i = 0)
    {
        global.StartCoroutine(TemporaryAnimationIEnumerator(requestedGameObject, requestedAnimationClip, i));
    }
    //checks to see if animation wrap only plays once
    public static bool ReturnWrapOnceBool(WrapMode wrapMode)
    {
        return wrapMode == WrapMode.Default || wrapMode == WrapMode.Once;
    }
    public static IEnumerator TemporaryAnimationIEnumerator(GameObject requestedGameObject, AnimationClip requestedAnimationClip, float i)
    {
        Animation animation = requestedGameObject.GetComponent<Animation>();
        bool WasCreated = false;

        if (!animation)
        {
            animation = requestedGameObject.AddComponent<Animation>();
            animation.playAutomatically = false;
            WasCreated = true;
        }

        if (!animation.enabled)
            animation.enabled = true;

        if (!animation.clip)
            animation.clip = requestedAnimationClip;

        if (!animation.GetClip(requestedAnimationClip.name))
        {
            animation.AddClip(requestedAnimationClip, requestedAnimationClip.name);
        }

        //	Debug.Log(animation);

        /*
        if (SetupManager.singleton.PopupAnimationClip == requestedAnimationClip) //there is a single frame where the button is full size
        {
            //animation.transform.localScale = Vector3.zero;
            // Debug.Log(animation);
        }
        */

        PlayAnimation(animation, requestedAnimationClip.name);
        animation[requestedAnimationClip.name].time -= i / 14;

        if (animation && WasCreated && ReturnWrapOnceBool(requestedAnimationClip.wrapMode))
        {
            yield return new WaitUntil(() => !animation || !animation.isPlaying);


            Destroy(animation);
        }
    }
}
