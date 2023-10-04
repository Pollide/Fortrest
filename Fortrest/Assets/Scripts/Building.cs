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

    public enum ResourceType
    {
        Wood,
        Stone,
        Bush,
        HardWood,
        CoarseWood,
        SlateStone,
        MossyStone,
        none
    }

    public ResourceType resourceObject;

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
    public bool playerinRange;
    private float timerText = 0.0f;

    public GameObject normalHouse;
    public GameObject destroyedHouse;

    public TMP_Text interactText;
    [HideInInspector] public bool textDisplayed;

    public bool DebugDestroyInstantly;

    public Vector2 gridLocation;
    Vector3 treeFallingDirection;

    // [HideInInspector]
    public float destroyedTimer;
    Quaternion startingRotation;
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.ReturnInMainMenu())
        {
            enabled = false;
            if (GetComponent<BoxCollider>())
                GetComponent<BoxCollider>().enabled = false;
            return;
        }
        health = maxHealth;
        startingRotation = transform.rotation;
        //Add a rigidbody to the building so the mouse raycasthit will return the top parent.

        Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.isKinematic = true; //prevents any forces acting upon it
        rigidbody.useGravity = false; //prevents the object from being affected by gravity

        //grabs all children transforms including itself by finding the component it matches, as I put transform there, it will just grab all of them
        List<Transform> transformList = GameManager.FindComponent<Transform>(transform);

        //create a for loop from the list
        //for (int i = 0; i < transformList.Count; i++)
        //{
        //    //sets all transforms to the building layer, so the mouse will easily be able to click the building
        //    if (transformList[i].gameObject.layer != LayerMask.NameToLayer("UI"))
        //        transformList[i].gameObject.layer = LayerMask.NameToLayer("Building");
        //}

        if (buildingObject == BuildingType.House)
        {
            Indicator.global.AddIndicator(transform, Color.yellow, "Home", customSprite: Indicator.global.HomeSprite);
            SetLastHealth();
            if (HUDHealthBar != null)
            {
                HUDHealthBar.SetHealth(health, maxHealth);
            }
        }
        else // the house itself is not part of the buildings list
        {
            if (!GetComponent<Defence>() || !GetComponent<Defence>().MiniTurret)
            {
                LevelManager.global.AddBuildingVoid(transform);
            }
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
            GameManager.ReturnResource(resourceObject.ToString(), new Vector3(transform.position.x + posX, transform.position.y + 2.0f, transform.position.z + posZ), transform.rotation * Quaternion.Euler(resourceObject.ToString().Contains("Wood") ? 0 : Random.Range(0, 361), Random.Range(0, 361), Random.Range(0, 361)));
        }
    }

    public void TakeDamage(float amount)
    {
        if (destroyedTimer == 0)
        {
            health -= amount;
            if (HUDHealthBar && buildingObject == BuildingType.House)
            {
                HUDHealthBar.SetHealth(health, maxHealth);
            }
            if (healthBarImage)
            {
                healthBarImage.fillAmount = Mathf.Clamp(health / maxHealth, 0, 1f);
            }

            if (amount != 0)
            {
                HealthAnimation();

                if (health <= 0)
                {
                    if (resourceObject == ResourceType.Wood || resourceObject == ResourceType.CoarseWood || resourceObject == ResourceType.HardWood)
                    {
                        GameManager.global.SoundManager.PlaySound(GameManager.global.TreeBreakingSound);
                    }
                    else if (resourceObject == ResourceType.Stone || resourceObject == ResourceType.MossyStone || resourceObject == ResourceType.SlateStone)
                    {
                        GameManager.global.SoundManager.PlaySound(GameManager.global.BoulderBreakingSound);
                    }
                    else if (resourceObject == ResourceType.Bush)
                    {
                        GameManager.global.SoundManager.PlaySound(GameManager.global.BushBreakingSound);
                    }
                    DestroyBuilding();
                }
            }
        }
    }


    public void DestroyBuilding()
    {
        if (destroyedTimer == 0 && GetComponent<Animation>())
        {
            HealthAppearTimer = 999;//so it fades away
            if (buildingObject == BuildingType.House)
            {
                GameManager.global.SoundManager.StopSelectedSound(GameManager.global.SnoringSound);
                GameManager.global.SoundManager.PlaySound(GameManager.global.HouseDestroyedSound);
                normalHouse.SetActive(false);
                destroyedHouse.SetActive(true);
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


            if (ReturnWood())
            {
                /*
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<Rigidbody>().AddForce(PlayerController.global.transform.forward * 10, ForceMode.Impulse);
                */

                treeFallingDirection = PlayerController.global.transform.up.normalized;

            }
            else
            {
                if (GetComponent<Defence>())
                {
                    PlayerModeHandler.global.occupied[(int)gridLocation.x, (int)gridLocation.y] = false;
                    LevelManager.global.RemoveBuildingVoid(transform);
                    Destroy(gameObject);
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

    private void HealthAnimation()
    {
        Animation animation = healthBarImage.GetComponentInParent<Animation>();

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
        return resourceObject == ResourceType.HardWood || resourceObject == ResourceType.Wood || resourceObject == ResourceType.CoarseWood;
    }

    public bool ReturnStone()
    {
        return resourceObject == ResourceType.Stone || resourceObject == ResourceType.MossyStone || resourceObject == ResourceType.SlateStone;
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

            if (health != lastHealth)
            {
                SetLastHealth();
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

        if (destroyedTimer != 0)
        {
            if (destroyedTimer > 120)
            {
                transform.rotation = startingRotation;
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 0.5f * Time.deltaTime);

                if (transform.localScale.x > 0.99f)
                {
                    transform.localScale = Vector3.one;
                    health = maxHealth;
                    destroyedTimer = 0;
                }
            }
            else
            {
                transform.localScale = Vector3.zero;
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
                GameManager.PlayAnimation(healthBarImage.GetComponentInParent<Animation>(), "Health Appear", false);
            }
        }

        if (treeFallingDirection != Vector3.zero)
        {
            Debug.DrawRay(transform.position, treeFallingDirection * 10, Color.red);

            // Use LookRotation to orient the tree towards the player
            Quaternion targetRotation = Quaternion.LookRotation(treeFallingDirection);


            // Calculate the angle between the tree's current rotation and the target rotation
            float angleDifference = Vector3.Angle(transform.forward, treeFallingDirection);


            if (angleDifference == 0)
            {

                // If the angle exceeds the maximum allowed angle, set the rotation to the maximum angle
                //   targetRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationAngle);
                treeFallingDirection = Vector3.zero; //Stop falling once it reaches the maximum angle

                ResourceRegenerate();

            }

            //  transform.Rotate(treeFallingDirection * 20 * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 80 * Time.deltaTime);
        }
    }
}
