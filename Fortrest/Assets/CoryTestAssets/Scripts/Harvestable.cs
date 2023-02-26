using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Harvestable : MonoBehaviour
{
    public enum ResourceType
    {
        Tree,
        Rock,
        Grass,
        Food
    }

    public ResourceType resourceObject;

    public bool isPlayerInRange = false;

    private float health;
    public float maxHealth = 5;
    public int resourceAmount = 5;

    public GameObject wood;
    public GameObject stone;
    public GameObject grass;
    public GameObject food;

    public Image healthBarImage;

    private void Start()
    {
        health = maxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private void OnMouseOver()
    {
        if (isPlayerInRange && Input.GetMouseButtonDown(0))
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
        }
    }

    private void GiveResources()
    {
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
    }
}
