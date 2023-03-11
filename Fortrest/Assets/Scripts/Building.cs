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

    public enum BuildingType
    {
        Cannon,
        CannonBP,
        Wood,
        Stone,
        Food,
    }

    public BuildingType resourceObject;

    private float health;
    public float maxHealth = 5;
    public float energyConsumptionPerClick = 2;
    public int resourceAmount = 5;

    public Image healthBarImage;

    private float gatherCooldown = 0.75f;
    private float nextGather;
    AnimationState HealthAnimationState;

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
        GameManager.ChangeAnimationLayers(healthBarImage.transform.parent.parent.GetComponent<Animation>());

        //create a for loop from the list
        for (int i = 0; i < transformList.Count; i++)
        {
            //sets all transforms to the building layer, so the mouse will easily be able to click the building
            if (transformList[i].gameObject.layer != LayerMask.NameToLayer("UI"))
                transformList[i].gameObject.layer = LayerMask.NameToLayer("Building");
        }

        if (resourceObject == BuildingType.Cannon)
        {
            LevelManager.global.BuildingList.Add(transform);
        }
    }


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

    public void OnMouseOver()
    {
        float minDistanceFloat = 4;

        // Debug.Log(PlayerModeHandler.global.playerModes + " " + (PlayerModeHandler.global.playerModes == PlayerModes.ResourceMode));
        if (Vector3.Distance(PlayerController.global.transform.position, transform.position) < minDistanceFloat && Input.GetMouseButton(0) && NaturalBool && PlayerModeHandler.global.playerModes == PlayerModes.ResourceMode && Time.time > nextGather)
        {
            nextGather = Time.time + gatherCooldown;

            if (health > 1)
            {
                if (resourceObject == BuildingType.Stone)
                {
                    GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.Pickaxe2Sound : GameManager.global.Pickaxe3Sound);
                }
                else if (resourceObject == BuildingType.Wood)
                {
                    GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.TreeChop1Sound : GameManager.global.TreeChop2Sound);
                }

                TakeDamage(1);
            }
            else
            {
                GiveResources();
                DestroyBuilding();
            }

            healthBarImage.fillAmount = Mathf.Clamp(health / maxHealth, 0, 1f);

            PlayerController.global.ApplyEnergyDamage(energyConsumptionPerClick);
        }

    }

    private void GiveResources()
    {
        // Debug.Log(resourceObject.ToString() + " Drop" + " ");
        for (int i = 0; i < resourceAmount; i++)
        {
            Instantiate(Resources.Load(resourceObject.ToString() + " Drop"), new Vector3(transform.position.x + Random.Range(-1, 1), transform.position.y + Random.Range(0, 2), transform.position.z + Random.Range(-1, 1)), transform.rotation);
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

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
        if (resourceObject == BuildingType.Cannon)
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
}
