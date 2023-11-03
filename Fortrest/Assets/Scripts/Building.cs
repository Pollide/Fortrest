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

    public enum BuildingType
    {
        Ballista,
        Cannon,
        Slow,
        Scatter,
        House,
        HouseNode,
        none
    }

    public BuildingType buildingObject;

    //  [HideInInspector]
    public float health;
    public float maxHealth = 4;
    public int resourceAmount = 5;
    public int constructionCostWood = 5;
    public int constructionCostStone = 5;

    public float ReturnRepair()
    {
        return maxHealth / 5;
    }

    HealthBar HUDHealthBar;

    float HealthAppearTimer = -1;

    private float lastHealth;

    private bool underAttack;
    public bool playerinRange;
    private float timerText = 0.0f;

    public GameObject normalHouse;
    public GameObject destroyedHouse;

    public TMP_Text interactText;
    [HideInInspector] public bool textDisplayed;

    public bool DebugDestroyInstantly;

    public Vector2 gridLocation;
    Vector3 treeFallingDirection;
    float treefallingTimer;
    // [HideInInspector]
    public float destroyedTimer;
    Quaternion startingRotation;

    public GameObject DropPrefab;
    GameObject turretOnFire;
    // Start is called before the first frame update
    void Start()
    {
        List<HealthBar> healthBarList = GameManager.FindComponent<HealthBar>(transform);

        if (healthBarList.Count > 0)
            HUDHealthBar = healthBarList[0];

        if (GameManager.ReturnInMainMenu())
        {
            enabled = false;
            if (GetComponent<BoxCollider>())
                GetComponent<BoxCollider>().enabled = false;
            return;
        }
        health = ReturnMaxHealth();
        startingRotation = transform.rotation;
        //Add a rigidbody to the building so the mouse raycasthit will return the top parent.

        Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.isKinematic = true; //prevents any forces acting upon it
        rigidbody.useGravity = false; //prevents the object from being affected by gravity

        if (buildingObject == BuildingType.House)
        {
            Indicator.global.AddIndicator(transform, Color.yellow, "Home", false, customSprite: Indicator.global.HomeSprite);
            SetLastHealth();
            if (HUDHealthBar != null)
            {
                HUDHealthBar.SetHealth(health, ReturnMaxHealth());


            }
        }
        else if (DropPrefab || buildingObject == BuildingType.HouseNode)
        {
            LevelManager.global.AddBuildingVoid(transform);
        }
    }

    public void SetLastHealth()
    {
        lastHealth = health;
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
            GameManager.ReturnResource(DropPrefab, new Vector3(transform.position.x + posX, transform.position.y + 2.0f, transform.position.z + posZ), transform.rotation * Quaternion.Euler(DropPrefab.GetComponent<ItemDrop>().WoodBool ? 0 : Random.Range(0, 361), Random.Range(0, 361), Random.Range(0, 361)));
        }
    }

    public void TakeDamage(float amount)
    {
        if (destroyedTimer == 0)
        {
            health -= amount;

            if (HUDHealthBar) //house doesnt have one
                HUDHealthBar.SetHealth(health, ReturnMaxHealth());

            if (!DropPrefab)
            {
                if (health < maxHealth * 0.4f)
                {
                    if (!turretOnFire)
                        turretOnFire = Instantiate(PlayerModeHandler.global.turretOnFirePrefab, transform);

                    if (buildingObject == BuildingType.House)
                    {
                        turretOnFire.transform.localScale = new Vector3(6, 6, 6);
                    }
                }
                else if (turretOnFire)
                {
                    Destroy(turretOnFire);
                }
            }

            if (buildingObject == BuildingType.House)
            {
                PlayerController.global.houseHealthBar.SetHealth(health, ReturnMaxHealth());

                if (health < ReturnMaxHealth() && amount != 0)
                    GameManager.PlayAnimation(PlayerController.global.UIAnimation, "House Flash");
            }

            if (amount != 0)
            {
                if (buildingObject != BuildingType.House)
                    HealthAnimation();

                if (health <= 0)
                {

                    if (ReturnWood())
                    {
                        GameManager.global.SoundManager.PlaySound(GameManager.global.TreeBreakingSound);
                    }
                    else if (ReturnStone())
                    {
                        GameManager.global.SoundManager.PlaySound(GameManager.global.BoulderBreakingSound);
                    }
                    else if (ReturnBush())
                    {
                        GameManager.global.SoundManager.PlaySound(GameManager.global.BushBreakingSound);
                    }


                    DestroyBuilding();
                }
            }
        }
    }

    public float ReturnMaxHealth()
    {
        return GetComponent<Defence>() ? GetComponent<Defence>().ReturnHealth(true) : maxHealth;
    }

    public void DestroyBuilding()
    {
        if (destroyedTimer == 0 && GetComponent<Animation>())
        {
            HealthAppearTimer = 999;//so it fades away
            if (buildingObject == BuildingType.House)
            {
                GameManager.global.SoundManager.StopSelectedSound(GameManager.global.SnoringSound);
                GameManager.global.SoundManager.PlaySound(GameManager.global.HouseDestroyedSound, 0.8f);
                normalHouse.SetActive(false);
                destroyedHouse.SetActive(true);
                PlayerController.global.UpdateResourceHolder(new PlayerController.ResourceData(), false);
                PlayerController.global.playerCanMove = false;
                LevelManager.global.enabled = false;//stop the day progressing


                LevelManager.global.enabled = false; //stops functions happening
                PlayerController.global.enabled = false;
                PlayerModeHandler.global.enabled = false;
                Invoke(nameof(RestartGame), GameManager.PlayAnimation(PlayerController.global.UIAnimation, "Gameover").length);

                PlayerController.global.SurvivedTMP_Text.text = "Survived " + (LevelManager.global.day + 1) + " day" + (LevelManager.global.day > 1 ? "s" : "");
                return;
            }

            //  PlayerEulerY = PlayerController.global.transform.eulerAngles.y;


            if (GetComponent<Defence>())
            {
                //PlayerModeHandler.global.occupied[(int)gridLocation.x, (int)gridLocation.y] = false;
                if (PlayerModeHandler.global.SelectedTurret == this)
                {
                    PlayerModeHandler.global.TurretMenuSet(false);
                }
                LevelManager.global.RemoveBuildingVoid(transform);
                Destroy(gameObject);
            }
            else
            {
                if (ReturnWood())
                {
                    treeFallingDirection = (PlayerController.global.transform.up * 2) - PlayerController.global.transform.forward;
                    //   treeFallingDirection = PlayerController.global.transform.up.normalized + (PlayerController.global.transform.position - transform.position).normalized;
                    //treeFallingDirection = (PlayerController.global.transform.position - transform.position).normalized;
                }
                else
                {
                    ResourceRegenerate();
                }
            }

        }
        PlayerController.global.currentResource = null;
    }

    void ResourceRegenerate()
    {
        GameManager.PlayAnimation(GetComponent<Animation>(), "Nature Destroy");

        GiveResources();
        destroyedTimer = 1;
    }

    public void RestartGame()
    {
        GameManager.global.NextScene(1);
    }

    public void HealthAnimation()
    {
        Animation animation = HUDHealthBar.GetComponentInParent<Animation>();

        if (HealthAppearTimer == -1)
        {
            GameManager.PlayAnimation(animation, "Health Appear");
        }

        HealthAppearTimer = 0;

        GameManager.PlayAnimation(animation, "Health Hit");


        if (health > 0)
            GameManager.PlayAnimation(GetComponent<Animation>(), "Nature Shake");
    }

    public bool ReturnWood()
    {
        return DropPrefab && DropPrefab.GetComponent<ItemDrop>().WoodBool;
    }

    public bool ReturnStone()
    {
        return DropPrefab && DropPrefab.GetComponent<ItemDrop>().stoneBool;
    }
    public bool ReturnBush()
    {
        return DropPrefab && DropPrefab.GetComponent<ItemDrop>().foodBool;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (DebugDestroyInstantly)
        {
            DebugDestroyInstantly = false;
            DestroyBuilding();
        }
#endif

        if (buildingObject == BuildingType.House)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.global.transform.position);
            if (distanceToPlayer < 12.5f)
            {
                playerinRange = true;
            }
            else
            {
                playerinRange = false;
            }

            if (PlayerModeHandler.global.canInteractWithHouse && !PlayerModeHandler.global.inTheFortress)
            {
                if (!textDisplayed)
                {
                    LevelManager.FloatingTextChange(interactText.gameObject, true);
                    textDisplayed = true;
                }
            }
            else
            {
                if (textDisplayed)
                {
                    LevelManager.FloatingTextChange(interactText.gameObject, false);
                    textDisplayed = false;
                }
            }

            if (!LevelManager.global.ReturnNight() && health > 0 && health < maxHealth) //house regens slowly
            {
                health += 0.05f * Time.deltaTime;
                GameManager.PlayAnimation(PlayerController.global.UIAnimation, "House Flash Green");
            }

            if (health != lastHealth)
            {
                if (!underAttack && health < lastHealth)
                {
                    underAttack = true;
                    GameManager.PlayAnimation(PlayerController.global.houseUnderAttackText.GetComponent<Animation>(), "HouseAttacked");
                }
                SetLastHealth();
                timerText = 0.0f;
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

        if (destroyedTimer != 0)
        {
            bool appear = destroyedTimer > 120;

            if (appear)
            {
                transform.rotation = startingRotation;
                GameManager.PlayAnimation(GetComponent<Animation>(), "Nature Destroy", false);
                transform.localScale = Vector3.one;
                health = ReturnMaxHealth();
                destroyedTimer = 0;
            }
            else
            {
                destroyedTimer += Time.deltaTime;
            }
            /*
            transform.localScale = Vector3.Slerp(transform.localScale, Vector3.one, Time.deltaTime);

            if (transform.localScale.x > 0.9f)
            {
                transform.localScale = Vector3.one;
                regenerateBool = false;

            }
            */
        }

        if (HealthAppearTimer != -1)
        {
            HealthAppearTimer += Time.deltaTime;

            if (HealthAppearTimer > 5)
            {
                HealthAppearTimer = -1;
                GameManager.PlayAnimation(HUDHealthBar.GetComponentInParent<Animation>(), "Health Appear", false);
            }
        }

        if (treeFallingDirection != Vector3.zero)
        {
            Debug.DrawRay(transform.position, treeFallingDirection * 10, Color.red);

            treefallingTimer += Time.deltaTime;

            if (treefallingTimer > 1f)
            {

                // If the angle exceeds the maximum allowed angle, set the rotation to the maximum angle
                //   targetRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationAngle);
                treeFallingDirection = Vector3.zero; //Stop falling once it reaches the maximum angle
                treefallingTimer = 0;
                ResourceRegenerate();

            }
            else
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(treeFallingDirection), Time.deltaTime * 2f);

        }
    }
}