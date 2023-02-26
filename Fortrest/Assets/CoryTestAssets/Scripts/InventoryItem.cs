using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    private InventoryManager inventoryManager;

    public enum ItemType
    {
        Wood,
        Stone,
        Grass,
        Food
    }

    public ItemType type;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);

            switch (type)
            {
                case ItemType.Food:
                    inventoryManager.AddFood(1);
                    break;
                case ItemType.Wood:
                    inventoryManager.AddWood(1);
                    break;
                case ItemType.Grass:
                    inventoryManager.AddGrass(1);
                    break;
                case ItemType.Stone:
                    inventoryManager.AddStone(1);
                    break;
            }
        }
    }
}


