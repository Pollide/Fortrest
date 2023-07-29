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

    private float timer;
    private bool rotationSet;
    private Quaternion randomRotation;

    [HideInInspector]
    public bool CollectedBool;

    private void Start()
    {
        for (int i = 0; i < resourceAmount; i++)
        {
            LevelManager.global.InventoryItemList.Add(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!CollectedBool)
        {
            if (other.CompareTag("Player"))
            {
                if (name == "Apple" && PlayerController.global.appleAmount >= 5)
                {
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
            PlayerController.global.appleText.text = PlayerController.global.appleAmount.ToString();               
        }
        InventoryManager.global.AddItem(this, resourceAmount);
        CollectedBool = true;
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
            transform.position = Vector3.Lerp(transform.position, new Vector3 (PlayerController.global.transform.position.x, PlayerController.global.transform.position.y + 2.0f, PlayerController.global.transform.position.z), timer * 3.0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, randomRotation, timer / 2.0f);
            transform.localScale -= Vector3.one * 20 * Time.deltaTime;
            gameObject.GetComponent<BoxCollider>().enabled = false;

            if (transform.localScale.x <= 0.5f)
            {
                gameObject.SetActive(false);
            }
        }
    }
}


