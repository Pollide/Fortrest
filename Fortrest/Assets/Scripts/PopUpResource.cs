using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUpResource : MonoBehaviour
{
    public static PopUpResource global;
    private SpriteRenderer spriteRenderer;
    public TMP_Text amountText;
    public bool displayNow;
    public int resourceInt;
    private bool once = true;

    private void Awake()
    {
        global = this;
    }

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (displayNow)
        {
            StopCoroutine("Hide");
            switch (resourceInt)
            {
                case 0:
                    spriteRenderer.sprite = LevelManager.global.WoodTierList[0].ResourceIcon;
                    amountText.text = LevelManager.global.WoodTierList[0].ResourceAmount.ToString();
                    break;
                case 1:
                    spriteRenderer.sprite = LevelManager.global.WoodTierList[1].ResourceIcon;
                    amountText.text = LevelManager.global.WoodTierList[1].ResourceAmount.ToString();
                    break;
                case 2:
                    spriteRenderer.sprite = LevelManager.global.WoodTierList[2].ResourceIcon;
                    amountText.text = LevelManager.global.WoodTierList[3].ResourceAmount.ToString();
                    break;
                case 3:
                    spriteRenderer.sprite = LevelManager.global.StoneTierList[0].ResourceIcon;
                    amountText.text = LevelManager.global.StoneTierList[0].ResourceAmount.ToString();
                    break;
                case 4:
                    spriteRenderer.sprite = LevelManager.global.StoneTierList[1].ResourceIcon;
                    amountText.text = LevelManager.global.StoneTierList[1].ResourceAmount.ToString();
                    break;
                case 5:
                    spriteRenderer.sprite = LevelManager.global.StoneTierList[2].ResourceIcon;
                    amountText.text = LevelManager.global.StoneTierList[2].ResourceAmount.ToString();
                    break;
                default:
                    break;
            }
            if (spriteRenderer.color.a != 1.0f)
            {
                GameManager.PlayAnimation(spriteRenderer.gameObject.GetComponent<Animation>(), "PopUpAppear");
                GameManager.PlayAnimation(amountText.gameObject.GetComponent<Animation>(), "TextAppear");               
            }
            once = false;
            displayNow = false;
        }
        else
        {
            if (!once)
            {
                StartCoroutine("Hide");
                once = true;
            }
        }
    }

    private IEnumerator Hide()
    {
        yield return new WaitForSeconds(2.0f);
        GameManager.PlayAnimation(spriteRenderer.gameObject.GetComponent<Animation>(), "PopUpDisappear");
        GameManager.PlayAnimation(amountText.gameObject.GetComponent<Animation>(), "TextDisappear");
    }
}