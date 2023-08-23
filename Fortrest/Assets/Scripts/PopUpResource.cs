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
    public LevelManager.TierData TierData;
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

            spriteRenderer.sprite = TierData.ResourceIcon;
            amountText.text = TierData.ResourceAmount.ToString();

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