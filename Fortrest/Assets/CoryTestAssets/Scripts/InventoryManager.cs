using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    // Set as global so that we can access it from anywhere in scene
    public static InventoryManager global;
    // List to store the inventory slots
    private List<InventorySlot> inventory;
    private List<DragableItem> dragableItems;

    // Maximum number of items that can be held in the inventory
    public int inventorySize = 10;

    public InventorySlotHandler[] inventorySlots;

    private GameObject inventoryPanel;

    PlayerModes currentPlayerModes;

    private void Awake()
    {
        global = this;
    }

    private void Start()
    {
        // Initialize the inventory list with empty slots
        inventory = new List<InventorySlot>(inventorySize);

        dragableItems = new List<DragableItem>(inventorySize);

        inventoryPanel = GameObject.Find("InventoryPanel");

        inventoryPanel.SetActive(false);

        currentPlayerModes = PlayerModeHandler.global.playerModes;
    }

    private void Update()
    {
        OpenInventory();
    }

    // Adds items to inventory list 
    public bool AddItem(InventoryItem item, int quantity = 1)
    {
        // Checks if the item is stackable
        if (item.stackable)
        {
            // Tries to find if the item is already in the list
            InventorySlot slot = inventory.Find(s => s.item.name == item.name);

            DragableItem dragableItem = dragableItems.Find(d => d.name == item.name);

            // If an an item is found then add to quantity
            if (slot != null)
            {
                slot.quantity += quantity;
                Debug.Log("Item added to inventory: " + item.name + " (Quantity: " + slot.quantity + ")");
                
                slot.item.dragableItem.GetComponent<DragableItem>().quantityText.text = slot.quantity.ToString();

                dragableItem.quantityText.text = slot.quantity.ToString();

                return true;
            }
        }
        // Checks if inventory is at max capacity
        if (inventory.Count < inventorySize)
        {
            // Adds a new inventory slot with the new item in it
            InventorySlot newSlot = new InventorySlot(item, quantity);
            inventory.Add(newSlot);
            Debug.Log("Item added to inventory: " + item.name + " (Quantity: " + newSlot.quantity + ")");

            for (int i = 0; i < inventorySlots.Length; i++)
            {
                InventorySlotHandler inventorySlot = inventorySlots[i];
                DragableItem inventoryItemInSlot = inventorySlot.GetComponentInChildren<DragableItem>();

                if (inventoryItemInSlot == null)
                {
                    GameObject newItem = Instantiate(newSlot.item.dragableItem, inventorySlot.transform);

                    newItem.GetComponent<DragableItem>().quantityText.text = newSlot.quantity.ToString();

                    dragableItems.Add(newItem.GetComponent<DragableItem>());

                    return true;
                }
            }
            return true;
        }
        else
        {
            Debug.Log("Inventory is full. Cannot add item: " + item.name);
            return false;
        }

    }

    public bool RemoveItem(string item, int quantity = 1)
    {
        InventorySlot slot = inventory.Find(s => s.item.name == item);
        if (slot != null)
        {
            if (slot.quantity >= quantity)
            {
                slot.quantity -= quantity;
                Debug.Log("Item removed from inventory: " + item + " (Quantity: " + slot.quantity + ")");

                if (slot.quantity <= 0)
                {
                    inventory.Remove(slot);
                }

                return true;
            }
            else
            {
                Debug.Log("Not enough quantity of item: " + item + " in inventory.");
                return false;
            }
        }
        else
        {
            Debug.Log("Item not found in inventory: " + item);
            return false;
        }
    }

    public int GetItemQuantity(string name)
    {
        InventorySlot slot = inventory.Find(s => s.item.name == name);
        if (slot != null)
        {
            return slot.quantity;
        }
        else
        {
            return 0;
        }
    }

    public class InventorySlot
    {
        // The item in the slot
        public InventoryItem item;
        // The quantity of the item
        public int quantity;
        
        public InventorySlot(InventoryItem item, int quantity)
        {
            this.item = item;
            this.quantity = quantity;
        }
    }

    // Open and show inventory items
    public void OpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryPanel.activeInHierarchy)
            {
                PlayerModeHandler.global.playerModes = currentPlayerModes;
                Time.timeScale = 1;
                inventoryPanel.SetActive(false);
            }
            else
            {
                currentPlayerModes = PlayerModeHandler.global.playerModes;
                PlayerModeHandler.global.playerModes = PlayerModes.Paused;
                Time.timeScale = 0;
                inventoryPanel.SetActive(true);
            }
        }
    }
}
