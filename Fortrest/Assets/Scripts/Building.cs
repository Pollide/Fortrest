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
using TMPro;

public class Building : MonoBehaviour
{
    public bool NaturalBool;
    public bool DestroyedBool;

    public enum BuildingType
    {
        Defense,
        DefenseBP,
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
    public float maxHealth = 4;
    public int resourceAmount = 5;
    public int constructionCostWood = 5;
    public int constructionCostStone = 5;

    public Image healthBarImage;
    public HealthBar HUDHealthBar;

    float HealthAppearTimer = -1;

    private float lastHealth;

    private bool underAttack;
    [HideInInspector] public bool playerinRange;
    private float timerText = 0.0f;

    private GameObject normalHouse;
    private GameObject destroyedHouse;

    public TMP_Text interactText;
    [HideInInspector] public bool textDisplayed;


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
            Indicator.global.AddIndicator(transform, Color.yellow, "Home", customSprite: Indicator.global.HomeSprite);
            lastHealth = health;
            if (HUDHealthBar != null)
            {
                HUDHealthBar.SetMaxHealth(maxHealth, true);
            }
            normalHouse = gameObject.transform.GetChild(0).gameObject;
            destroyedHouse = gameObject.transform.GetChild(1).gameObject;
        }
        else if (resourceObject != BuildingType.DefenseBP) //the house itself is not part of the buildings list
        {
            if (!GetComponent<TurretShooting>() || !GetComponent<TurretShooting>().MiniTurret)
                LevelManager.global.AddBuildingVoid(transform);
        }
    }

    public void GiveResources()
    {
        float posX = 0.0f;
        float posZ = 0.0f;
        for (int i = 0; i < resourceAmount; i++)
        {
            if (i < 2)
            {
                posX += i;
                posZ += i;
            }
            else if (i == 2)
            {
                posX -= i;
                posZ -= i;
            }
            else if (i == 3)
            {
                posX *= -1;
            }
            else if (i == 4)
            {
                posZ = posX;
                posX *= -1;
            }
            GameManager.ReturnResource(resourceObject.ToString(), new Vector3(transform.position.x + posX, transform.position.y + 2.0f, transform.position.z + posZ), transform.rotation * Quaternion.Euler(resourceObject.ToString() == "Wood" ? 0 : Random.Range(0, 361), Random.Range(0, 361), Random.Range(0, 361)));
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (resourceObject == BuildingType.House)
        {
            HUDHealthBar.SetHealth(health, true);
        }

        if (amount != 0)
        {
            HealthAnimation();
        }
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
        if (!DestroyedBool && GetComponent<Animation>())
        {
            DestroyedBool = true;
            if (resourceObject == BuildingType.House)
            {
                GameManager.global.SoundManager.StopSelectedSound(GameManager.global.SnoringSound);
                GameManager.global.SoundManager.PlaySound(GameManager.global.HouseDestroySound);
                normalHouse.SetActive(false);
                destroyedHouse.SetActive(true);
                PlayerController.global.playerCanMove = false;
                LevelManager.global.enabled = false;//stop the day progressing
                Invoke("RestartGame", GameManager.PlayAnimation(PlayerController.global.UIAnimation, "Gameover").length);
                PlayerController.global.SurvivedTMP_Text.text = "Survived " + (LevelManager.global.day + 1) + " days";
                return;
            }

            //  PlayerEulerY = PlayerController.global.transform.eulerAngles.y;

            if (resourceObject == BuildingType.HardWood || resourceObject == BuildingType.Wood || resourceObject == BuildingType.CoarseWood)
            {
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<Rigidbody>().AddForce(PlayerController.global.transform.forward * 10, ForceMode.Impulse);
            }

            Invoke("DisableInvoke", GameManager.PlayAnimation(GetComponent<Animation>()).length);
        }
        PlayerController.global.currentResource = null;
    }

    public void DisableInvoke()
    {
        DestroyedBool = true; //also calls in DestroyBuilding
        gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        GameManager.global.NextScene(1);
    }

    private void HealthAnimation()
    {
        Animation animation = healthBarImage.GetComponentInParent<Animation>();

        if (HealthAppearTimer == -1)
        {
            GameManager.PlayAnimation(animation, "Health Appear");
        }

        HealthAppearTimer = 0;

        GameManager.PlayAnimation(animation, "Health Hit");

        GameManager.PlayAnimation(GetComponent<Animation>(), "Nature Shake");
    }

    private void Update()
    {
        if (resourceObject == BuildingType.House)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.global.transform.position);
            if (distanceToPlayer < 15.0f)
            {
                if (PlayerModeHandler.global.playerModes != PlayerModes.BuildMode && PlayerModeHandler.global.playerModes != PlayerModes.RepairMode)
                {
                    if (!textDisplayed)
                    {
                        LevelManager.FloatingTextChange(interactText.gameObject, true);
                        textDisplayed = true;
                    }
                }

                PlayerController.global.needInteraction = true;
                playerinRange = true;
                PlayerController.global.canGetInHouse = true;
            }
            else
            {
                if (textDisplayed)
                {
                    LevelManager.FloatingTextChange(interactText.gameObject, false);
                    textDisplayed = false;
                }
                playerinRange = false;
                PlayerController.global.canGetInHouse = false;
                if (!Boar.global.inRange && !PlayerController.global.playerDead && !PlayerController.global.canTeleport)
                {
                    PlayerController.global.needInteraction = false;
                }
            }

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

        if (HealthAppearTimer != -1)
        {
            HealthAppearTimer += Time.deltaTime;

            if (HealthAppearTimer > 5)
            {
                HealthAppearTimer = -1;
                GameManager.PlayAnimation(healthBarImage.GetComponentInParent<Animation>(), "Health Appear", false);
            }
        }
    }

}
