using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassController : MonoBehaviour
{
    public static CompassController global;

    private LevelManager manager;

    [SerializeField] private TMPro.TMP_Text tMPro;
    [SerializeField] private GameObject[] outlines;
    private bool northRan;
    private bool eastRan;
    private bool southRan;
    private bool westRan;
    private bool isFading = false;
    [SerializeField] private float fadeDuration = 2.0f;

    private void Awake()
    {
        global = this;
    }

    private void Start()
    {
        ChangeText("");
        manager = LevelManager.global;
        tMPro.CrossFadeAlpha(0, 0, false); // Set initial alpha to 0
    }

    void Update()
    {
        if (isFading)
        {
            StartCoroutine(FadeText());
        }

        if (manager.goblinSpawnable && !northRan)
        {
            ChangeText("You hear rumbling from the north");
            StartCoroutine(FadeText());
            outlines[0].SetActive(true);
            northRan = true;
        }

        if (manager.goblinSpawnable && !eastRan && manager.day > 3)
        {
            ChangeText("You hear rumbling from the east");
            StartCoroutine(FadeText());
            outlines[1].SetActive(true);
            eastRan = true;
        }

        if (manager.goblinSpawnable && !eastRan && manager.day > 7)
        {
            ChangeText("You hear rumbling from the south");
            StartCoroutine(FadeText());
            outlines[2].SetActive(true);
            southRan = true;
        }

        if (manager.goblinSpawnable && !eastRan && manager.day > 10)
        {
            ChangeText("You hear rumbling from the west");
            StartCoroutine(FadeText());
            outlines[3].SetActive(true);
            westRan = true;
        }
    }

    IEnumerator FadeText()
    {
        isFading = true;
        tMPro.CrossFadeAlpha(1, fadeDuration, false); // Fade in
        yield return new WaitForSeconds(fadeDuration);
        tMPro.CrossFadeAlpha(0, fadeDuration, false); // Fade out
        yield return new WaitForSeconds(fadeDuration);
        isFading = false;
    }

    public void ChangeText(string text)
    {
        tMPro.text = text;
    }
}