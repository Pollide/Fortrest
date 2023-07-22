using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    bool soundPlayed;

    public int resourceAmount = 1;

    // Whether the item is stackable or not
    public bool stackable;

    // Name of the item
    public new string name;

    public GameObject dragableItem;
    public string resourceObject;

    private void Start()
    {
        LevelManager.global.InventoryItemList.Add(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            soundPlayed = false;

            CollectVoid();

        }

        if (!soundPlayed)
        {
            soundPlayed = true;
            GameManager.global.SoundManager.PlaySound(GameManager.global.CollectSound);
        }
    }

    public void CollectVoid()
    {
        InventoryManager.global.AddItem(this, resourceAmount);

        gameObject.SetActive(false);
    }
}


