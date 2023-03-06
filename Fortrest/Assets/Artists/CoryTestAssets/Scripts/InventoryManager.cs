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
    public int grass = 0;
    public int food = 0;

    public TextMeshProUGUI woodAmount;
    public TextMeshProUGUI stoneAmount;
    public TextMeshProUGUI grassAmount;
    public TextMeshProUGUI foodAmount;

    private void Awake()
    {
        global = this;
    }

    private void Update()
    {
        woodAmount.text = wood.ToString();
        stoneAmount.text = stone.ToString();
        grassAmount.text = grass.ToString();
        foodAmount.text = food.ToString();
    }

    public void AddFood(int amount)
    {
        food += amount;
    }

    public void AddWood(int amount)
    {
        wood += amount;
    }

    public void AddStone(int amount)
    {
        stone += amount;
    }

    public void AddGrass(int amount)
    {
        grass += amount;
    }

    public void MinusFood(int amount)
    {
        food -= amount;
    }

    public void MinusWood(int amount)
    {
        wood -= amount;
    }

    public void MinusStone(int amount)
    {
        stone -= amount;
    }

    public void MinusGrass(int amount)
    {
        grass -= amount;
    }
}
