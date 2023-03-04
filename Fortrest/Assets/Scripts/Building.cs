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

        LevelManager.global.BuildingList.Add(transform);
    }

    public void OnMouseDown()
    {
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(0) && !NaturalBool)
        {
            PreviousPos = transform.position;
            screenPoint = Camera.main.WorldToScreenPoint(PreviousPos);
            offset = PreviousPos - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
            LevelManager.global.ActiveBuildingGameObject = gameObject;
        }
    }

    public void OnMouseUp()
    {
        if (!NaturalBool)
            LevelManager.global.ActiveBuildingGameObject = null;
    }

    public void OnMouseDrag()
    {
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && !NaturalBool)
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

        float minDistanceFloat = 2;
        if (Vector3.Distance(PlayerController.global.transform.position, transform.position) < minDistanceFloat && Input.GetMouseButtonDown(0) && NaturalBool && modeHandler.playerModes == PlayerModes.ResourceMode)
        {
            if (health > 1)
            {
                health--;
                healthBarImage.fillAmount = Mathf.Clamp(health / maxHealth, 0, 1f);
            }
            else if (health == 1)
            {
                health--;
                GiveResources();
                Destroy(gameObject);
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
        /*
        switch (resourceObject)
        {


            case ResourceType.Tree:
                for (int i = 0; i < resourceAmount; i++)
                {
                    Instantiate(wood, new Vector3(gameObject.transform.position.x + Random.Range(-1, 1), gameObject.transform.position.y + Random.Range(0, 2), gameObject.transform.position.z + Random.Range(-1, 1)), gameObject.transform.rotation);
                }
                break;
            case ResourceType.Rock:
                for (int i = 0; i < resourceAmount; i++)
                {
                    Instantiate(stone, new Vector3(gameObject.transform.position.x + Random.Range(-1, 1), gameObject.transform.position.y + Random.Range(0, 2), gameObject.transform.position.z + Random.Range(-1, 1)), gameObject.transform.rotation);
                }
                break;
            case ResourceType.Grass:
                for (int i = 0; i < resourceAmount; i++)
                {
                    Instantiate(grass, new Vector3(gameObject.transform.position.x + Random.Range(-1, 1), gameObject.transform.position.y + Random.Range(0, 2), gameObject.transform.position.z + Random.Range(-1, 1)), gameObject.transform.rotation);
                }
                break;
            case ResourceType.Food:
                for (int i = 0; i < resourceAmount; i++)
                {
                    Instantiate(food, new Vector3(gameObject.transform.position.x + Random.Range(-1, 1), gameObject.transform.position.y + Random.Range(0, 2), gameObject.transform.position.z + Random.Range(-1, 1)), gameObject.transform.rotation);
                }
                break;
            default:
                break;
        }
        */
    }
}
