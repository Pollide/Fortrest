using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GamepadDetector : MonoBehaviour
{
    [Header("Change these two values below to fit")]
    public Sprite GamepadSprite;
    public string KeyboardString = "W";

    [Header("Below does not need to be changed")]
    public Sprite KeyboardSprite;
    public TMP_Text KeyboardText;

    bool KeyboardBool = true;

    private void Start()
    {
        Refresh();
    }
    private void Update()
    {
        if (GameManager.global && KeyboardBool != GameManager.global.KeyboardBool)
        {
            Refresh();
        }
    }

    public void Refresh()
    {
        KeyboardBool = GameManager.global.KeyboardBool;

        KeyboardText.gameObject.SetActive(KeyboardBool);
        KeyboardText.text = KeyboardString.ToUpper();

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer)
            spriteRenderer.sprite = KeyboardBool ? KeyboardSprite : GamepadSprite;
        else
            GetComponent<Image>().sprite = KeyboardBool ? KeyboardSprite : GamepadSprite;
    }
}
