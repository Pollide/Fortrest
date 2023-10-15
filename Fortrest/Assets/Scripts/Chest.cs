using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chest : MonoBehaviour
{
    private bool canBeOpened;
    private string resource;
    public TMP_Text promptText;
    private bool textDisplayed;
    [HideInInspector]
    public bool opened;
    public Animation openAnimation;

    private void Start()
    {
        GameManager.PlayAnimation(openAnimation, "ChestClose", true, true);
        if (LevelManager.global)
            LevelManager.global.chestList.Add(this);
    }

    void Update()
    {
        if (canBeOpened && (Input.GetKeyDown(KeyCode.E) || PlayerController.global.interactCTRL))
        {
            opened = true;
            promptText.gameObject.SetActive(false);
            SpawnResources();
            canBeOpened = false;
            PlayerController.global.interactCTRL = false;
            PlayerController.global.needInteraction = false;
            GameManager.PlayAnimation(openAnimation, "ChestOpen");
            GameManager.global.SoundManager.PlaySound(GameManager.global.ChestOpenSound);
        }
    }

    public void LoadOpen()
    {
        opened = true;
        canBeOpened = false;
        GameManager.PlayAnimation(openAnimation, "ChestOpen", true, true);
    }

    private void SpawnResources()
    {
        int resourceAmount = Random.Range(5, 9);
        float posX = 0.0f;
        float posZ = 0.0f;
        for (int i = 0; i < 8; i++)
        {
            PickResource();
            if (i == 0)
            {
                posX = 3f;
                posZ = 0f;
            }
            else if (i == 1 || i == 6)
            {
                posX *= -1f;
            }
            else if (i == 2)
            {
                posX = 0f;
                posZ = 3f;
            }
            else if (i == 3)
            {
                posZ *= -1f;
            }
            else if (i == 4)
            {
                posX = 2.2f;
                posZ = 2.2f;
            }
            else if (i == 5 || i == 7)
            {
                posX *= -1;
                posZ *= -1;
            }
            GameManager.ReturnResource(resource, new Vector3(transform.position.x + posX, transform.position.y + 2.0f, transform.position.z + posZ), transform.rotation * Quaternion.Euler(resource.Contains("Wood") ? 0 : Random.Range(0, 361), Random.Range(0, 361), Random.Range(0, 361)));
        }
    }

    private void PickResource()
    {
        int randomInt = Random.Range(1, 7);
        switch (randomInt)
        {
            case 1:
                resource = "Wood";
                break;
            case 2:
                resource = "Stone";
                break;
            case 3:
                resource = "HardWood";
                break;
            case 4:
                resource = "SlateStone";
                break;
            case 5:
                resource = "CoarseWood";
                break;
            case 6:
                resource = "MossyStone";
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerController.global.gameObject && !opened)
        {
            canBeOpened = true;
            if (!textDisplayed)
            {
                promptText.gameObject.SetActive(true);
                textDisplayed = true;
            }
            PlayerController.global.needInteraction = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerController.global.gameObject && !opened)
        {
            canBeOpened = false;
            if (textDisplayed)
            {
                promptText.gameObject.SetActive(false);
                textDisplayed = false;
            }
            PlayerController.global.needInteraction = false;
        }
    }
}
