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
        House,
    }

    public BuildingType resourceObject;

    [HideInInspector]
    public float health;
    public float maxHealth = 5;
    public float energyConsumptionPerClick = 2;
    public int resourceAmount = 5;
    public int constructionCostWood = 5;
    public int constructionCostStone = 5;

    public Image healthBarImage;

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

        if (NaturalBool)
            LevelManager.global.NaturalBuildingList.Add(this);
        else
            LevelManager.global.BuildingList.Add(transform);
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

        if (enabled)
        {
            enabled = false;

            if (resourceObject == BuildingType.House)
            {
                GameManager.PlayAnimation(GetComponent<Animation>(), "Nature Destroy");
                Invoke("RestartGame", GameManager.PlayAnimation(LevelManager.global.GetComponent<Animation>(), "Gameover").length);
                LevelManager.global.SurvivedTMP_Text.text = "Survived " + (LevelManager.global.day + 1) + " days";
                return;
            }

            Invoke("NowDestroy", GameManager.PlayAnimation(GetComponent<Animation>(), "Nature Destroy").length);   
        }
    }

    void NowDestroy()
    {
        if (resourceObject == BuildingType.Cannon)
        {
            Destroy(gameObject.transform.parent.gameObject);
            LevelManager.global.BuildingList.Remove(transform);
        }
        else
        {
            Destroy(gameObject);
            LevelManager.global.NaturalBuildingList.Remove(this);
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
}
