/*********************************************************************
Bachelor of Software Engineering
Media Design School
Auckland
New Zealand
(c) 2023 Media Design School
File Name : Building.cs
Description : Sets up the building mechanics such as shooting enemies
Author :  Allister Hamilton
Mail : allister.hamilton @mds.ac.nz
**************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    public bool NaturalBool;
    public bool DestroyedBool;

    public enum BuildingType
    {
        Cannon,
        CannonBP,
        Wood,
        Stone,
        Bush,
        House,
        HardWood,
        CoarseWood,
        Slate,
        MossyStone,
        HouseNode,
    }

    public BuildingType resourceObject;

    [HideInInspector]
    public float health;
    public float maxHealth = 5;
    public int resourceAmount = 5;
    public int constructionCostWood = 5;
    public int constructionCostStone = 5;

    public Image healthBarImage;
    public HealthBar HUDHealthBar;

    AnimationState HealthAnimationState;

    private float lastHealth;

    private bool underAttack;
    private float timerText = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        //Add a rigidbody to the building so the mouse raycasthit will return the top parent.


        Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.isKinematic = true; //prevents any forces acting upon it
        rigidbody.useGravity = false; //prevents the object from being affected by gravity

        //grabs all children transforms including itself by finding the component it matches, as I put transform there, it will just grab all of them
        List<Transform> transformList = GameManager.FindComponent<Transform>(transform);

        //create a for loop from the list
        for (int i = 0; i < transformList.Count; i++)
        {
            //sets all transforms to the building layer, so the mouse will easily be able to click the building
            if (transformList[i].gameObject.layer != LayerMask.NameToLayer("UI"))
                transformList[i].gameObject.layer = LayerMask.NameToLayer("Building");
        }

        if (resourceObject == BuildingType.House)
        {
            Indicator.global.AddIndicator(transform, Color.yellow, "Home");
            lastHealth = health;
            HUDHealthBar.SetMaxHealth(maxHealth);
        }
        else //the house itself is not part of the buildings list
        {
            LevelManager.global.AddBuildingVoid(transform);
        }
    }

    /*
    public void OnMouseDown()
    {
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(0) && !NaturalBool && PlayerModeHandler.global.playerModes == PlayerModes.BuildMode)
        {
            LevelManager.global.ActiveBuildingGameObject = gameObject;
        }
    }

    public void OnMouseUp()
    {
        if (!NaturalBool && PlayerModeHandler.global.playerModes == PlayerModes.BuildMode)
            LevelManager.global.ActiveBuildingGameObject = null;
    }
    */
    public void GiveResources()
    {
        // Debug.Log(resourceObject.ToString() + " Drop" + " ");
        for (int i = 0; i < resourceAmount; i++)
        {
            Instantiate(Resources.Load("Drops/" + resourceObject.ToString() + " Drop"), new Vector3(transform.position.x + Random.Range(-1, 1), transform.position.y + Random.Range(0, 2), transform.position.z + Random.Range(-1, 1)), transform.rotation);
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (resourceObject == BuildingType.House)
        {
            HUDHealthBar.SetHealth(health);
        }

        HealthAnimation();
    }

    public void Repair(float amount)
    {
        health += amount;

        HealthAnimation();
    }

    public float GetHealth()
    {
        return health;
    }

    public void DestroyBuilding()
    {
        if (enabled && GetComponent<Animation>())
        {
            enabled = false;
            DestroyedBool = true;
            if (resourceObject == BuildingType.House)
            {
                GameManager.global.SoundManager.StopSelectedSound(GameManager.global.SnoringSound);
                GameManager.global.SoundManager.PlaySound(GameManager.global.HouseDestroySound);
                GameManager.PlayAnimation(GetComponent<Animation>(), "Nature Destroy");
                Invoke("RestartGame", GameManager.PlayAnimation(PlayerController.global.UIAnimation, "Gameover").length);
                PlayerController.global.SurvivedTMP_Text.text = "Survived " + (LevelManager.global.day + 1) + " days";
                return;
            }
            Invoke("DisableInvoke", GameManager.PlayAnimation(GetComponent<Animation>(), "Nature Destroy").length);
        }
        PlayerController.global.currentResource = null;
    }

    public void DisableInvoke()
    {
        DestroyedBool = true; //also calls in DestroyBuilding
        if (resourceObject == BuildingType.Cannon)
        {
            transform.parent.gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void RestartGame()
    {
        GameManager.global.NextScene(1);
    }

    private void HealthAnimation()
    {
        Animation animation = healthBarImage.transform.parent.parent.GetComponent<Animation>();

        if (HealthAnimationState != null && HealthAnimationState.enabled)
        {
            // Debug.Log("hit");
            HealthAnimationState.time = 1;
            GameManager.PlayAnimation(animation, "Health Hit");
        }
        else
        {
            //Debug.Log(1);
            HealthAnimationState = GameManager.PlayAnimation(animation, "Health Appear");
        }
    }

    private void Update()
    {
        if (resourceObject == BuildingType.House)
        {
            if (health != lastHealth)
            {
                lastHealth = health;
                timerText = 0.0f;
                if (!underAttack)
                {
                    underAttack = true;
                    GameManager.PlayAnimation(PlayerController.global.houseUnderAttackText.GetComponent<Animation>(), "HouseAttacked");
                }
            }
            else
            {
                if (underAttack)
                {
                    timerText += Time.deltaTime;
                    if (timerText > 5.0f)
                    {
                        underAttack = false;
                        timerText = 0.0f;
                        GameManager.PlayAnimation(PlayerController.global.houseUnderAttackText.GetComponent<Animation>(), "HouseAttackedDisappear");
                    }
                }
            }
        }
    }
}
