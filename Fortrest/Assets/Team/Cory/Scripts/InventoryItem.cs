using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    bool soundPlayed;
    public int TierInt;
    public int resourceAmount = 1;

    // Whether the item is stackable or not
    public bool stackable;

    // Name of the item
    public new string name;

    public GameObject dragableItem;
    public string resourceObject;

    private float timer;
    private bool rotationSet;
    private Quaternion randomRotation;

    [HideInInspector]
    public bool CollectedBool;

    private void Start()
    {
        LevelManager.global.inventoryItemList.Add(gameObject);


        /* I have another idea cory if your wondering why this is hidden again
        for (int i = 0; i < resourceAmount; i++)
        {
            LevelManager.global.inventoryItemList.Add(gameObject);
        }
        */
    }

    private void OnTriggerStay(Collider other)
    {
        if (!CollectedBool)
        {
            if (other.CompareTag("Player"))
            {
                if (name == "Apple" && PlayerController.global.appleAmount >= PlayerController.global.maxApple)
                {
                    Vector3 direction = (transform.position - PlayerController.global.transform.position).normalized;
                    direction.y = 0.4f;
                    GetComponent<Rigidbody>().AddForce(direction / 5.5f, ForceMode.Impulse);
                }
                else
                {
                    soundPlayed = false;
                    CollectVoid();
                }
            }

            if (!soundPlayed)
            {
                soundPlayed = true;
                GameManager.global.SoundManager.PlaySound(GameManager.global.CollectSound);
            }
        }
    }

    public void CollectVoid()
    {
        if (name == "Apple")
        {
            PlayerController.global.appleAmount += 1;
            GameManager.PlayAnimation(PlayerController.global.appleText.GetComponent<Animation>(), "EnemyAmount");
            PlayerController.global.UpdateAppleText();
        }

        if (name.Contains("Wood"))
        {
            LevelManager.global.WoodTierList[TierInt].ResourceAmount += resourceAmount;
            PopUpResource.global.TierData = LevelManager.global.WoodTierList[TierInt];
            PopUpResource.global.displayNow = true;
        }

        if (name.Contains("Stone"))
        {
            LevelManager.global.StoneTierList[TierInt].ResourceAmount += resourceAmount;
            PopUpResource.global.TierData = LevelManager.global.StoneTierList[TierInt];
            PopUpResource.global.displayNow = true;
        }



        // InventoryManager.global.AddItem(this, resourceAmount);
        CollectedBool = true;
        LevelManager.global.inventoryItemList.Remove(gameObject);
    }

    private void Update()
    {
        if (CollectedBool)
        {
            timer = 0.0f;
            timer += Time.deltaTime;
            if (!rotationSet)
            {
                randomRotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                rotationSet = true;
            }
            transform.position = Vector3.Lerp(transform.position, new Vector3(PlayerController.global.transform.position.x, PlayerController.global.transform.position.y + 2.0f, PlayerController.global.transform.position.z), timer * 3.0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, randomRotation, timer / 2.0f);
            transform.localScale -= Vector3.one * 20 * Time.deltaTime;
            gameObject.GetComponent<BoxCollider>().enabled = false;

            if (transform.localScale.x <= 0.5f)
            {
                // gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }
    }
}


