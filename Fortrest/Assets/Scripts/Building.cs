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
    Vector3 PreviousPos;
    Vector3 screenPoint;
    Vector3 offset;

    public bool NaturalBool;

    public enum BuildingType
    {
        Cannon,
        Wood,
        Stone,
        Grass,
        Food,
    }

    public BuildingType resourceObject;

    private float health;
    public float maxHealth = 5;
    public float energyConsumptionPerClick = 2;
    public int resourceAmount = 5;

    public Image healthBarImage;

    private PlayerController playerController;
    private float gatherCooldown = 0.75f;
    private float nextGather;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

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
            transformList[i].gameObject.layer = LayerMask.NameToLayer("Building");
        }

        if (resourceObject == BuildingType.Cannon)
        {
            LevelManager.global.BuildingList.Add(transform);
        }
    }

    public void OnMouseDown()
    {
        PlayerModeHandler modeHandler = GameObject.Find("Level Manager").GetComponent<PlayerModeHandler>();
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(0) && !NaturalBool && modeHandler.playerModes == PlayerModes.BuildMode)
        {
            PreviousPos = transform.position;
            screenPoint = Camera.main.WorldToScreenPoint(PreviousPos);
            offset = PreviousPos - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
            LevelManager.global.ActiveBuildingGameObject = gameObject;
        }
    }

    public void OnMouseUp()
    {
        PlayerModeHandler modeHandler = GameObject.Find("Level Manager").GetComponent<PlayerModeHandler>();
        if (!NaturalBool && modeHandler.playerModes == PlayerModes.BuildMode)
            LevelManager.global.ActiveBuildingGameObject = null;
    }

    public void OnMouseDrag()
    {
        PlayerModeHandler modeHandler = GameObject.Find("Level Manager").GetComponent<PlayerModeHandler>();
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && !NaturalBool && modeHandler.playerModes == PlayerModes.BuildMode)
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
            curPosition = new Vector3(Mathf.Round(curPosition.x), transform.position.y, Mathf.Round(curPosition.z));
            transform.position = curPosition;

            PreviousPos = transform.position;
        }
    }


    private void OnMouseOver()
    {
        PlayerModeHandler modeHandler = GameObject.Find("Level Manager").GetComponent<PlayerModeHandler>();

        float minDistanceFloat = 3;
        if (Vector3.Distance(PlayerController.global.transform.position, transform.position) < minDistanceFloat && Input.GetMouseButton(0) && NaturalBool && modeHandler.playerModes == PlayerModes.ResourceMode && Time.time > nextGather)
        {
            nextGather = Time.time + gatherCooldown;
            if (health > 1)
            {
                if (resourceObject == BuildingType.Stone)
                {
                    GameManager.global.SoundManager.PlaySound(GameManager.global.Pickaxe2Sound);
                }
                else if (resourceObject == BuildingType.Wood)
                {
                    GameManager.global.SoundManager.PlaySound(GameManager.global.TreeChop1Sound);
                }
                TakeDamage(1);
                healthBarImage.fillAmount = Mathf.Clamp(health / maxHealth, 0, 1f);
            }
            else if (health == 1)
            {
                TakeDamage(1);
                GiveResources();
                DestroyBuilding();
            }
            playerController.ApplyEnergyDamage(energyConsumptionPerClick);
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
    }

    public void Repair(float amount)
    {
        health += amount;
    }

    public float GetHealth()
    {
        return health;
    }

    public void DestroyBuilding()
    {
        Destroy(gameObject);
    }
}
