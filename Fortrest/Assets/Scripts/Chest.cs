using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chest : MonoBehaviour
{
    private bool canBeOpened;
    private string resource;
    public Animation floatingTextAnimation;
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
            LevelManager.FloatingTextChange(floatingTextAnimation.gameObject, false);
            SpawnResources();
            PlayerController.global.interactCTRL = false;
            PlayerController.global.needInteraction = false;
            GameManager.global.SoundManager.PlaySound(GameManager.global.ChestOpenSound);

            LoadOpen(false);
        }
    }

    public void LoadOpen(bool quick = true)
    {
        opened = true;
        canBeOpened = false;
        GameManager.PlayAnimation(openAnimation, "ChestOpen", true, quick);
    }

    private void SpawnResources()
    {
        int resourceAmount = Random.Range(5, 9);
        float posX = 0.0f;
        float posZ = 0.0f;
        for (int i = 0; i < 8; i++)
        {
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

            int randomTier = Random.Range(0, LevelManager.global.WoodTierList.Count);
            GameObject prefab = Random.Range(0, 2) == 0 ? LevelManager.global.WoodTierList[randomTier].prefab : LevelManager.global.StoneTierList[randomTier].prefab;

            GameManager.ReturnResource(prefab, new Vector3(transform.position.x + posX, transform.position.y + 2.0f, transform.position.z + posZ), transform.rotation * Quaternion.Euler(resource.Contains("Wood") ? 0 : Random.Range(0, 361), Random.Range(0, 361), Random.Range(0, 361)));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerController.global.gameObject && !canBeOpened && !opened)
        {
            canBeOpened = true;
            if (!textDisplayed)
            {
                LevelManager.FloatingTextChange(floatingTextAnimation.gameObject, true);
                textDisplayed = true;
            }
            PlayerController.global.needInteraction = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerController.global.gameObject && canBeOpened && !opened)
        {
            canBeOpened = false;
            if (textDisplayed)
            {
                LevelManager.FloatingTextChange(floatingTextAnimation.gameObject, false);
                textDisplayed = false;
            }
            PlayerController.global.needInteraction = false;
        }
    }
}
