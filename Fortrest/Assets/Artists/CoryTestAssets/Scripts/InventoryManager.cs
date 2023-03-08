using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager global;

    [Header("Item Stack Sizes")]
    public int wood = 0;
    public int stone = 0;

    public TextMeshProUGUI woodAmount;
    public TextMeshProUGUI stoneAmount;

    private void Awake()
    {
        global = this;
    }

    private void Update()
    {
        woodAmount.text = wood.ToString();
        stoneAmount.text = stone.ToString();
    }

    public void AddWood(int amount)
    {
        wood += amount;
    }

    public void AddStone(int amount)
    {
        stone += amount;
    }

    public void MinusWood(int amount)
    {
        wood -= amount;
    }

    public void MinusStone(int amount)
    {
        stone -= amount;
    }
}
