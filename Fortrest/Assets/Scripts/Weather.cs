using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather : MonoBehaviour
{
    private GameObject[] weatherType = new GameObject[3];   
    private float timer;
    private float weatherTimer;
    private float weatherDuration;
    private bool weatherTriggered;
    private bool stepComplete;
    private int randomInt;
    private Color colorInitial;
    private Color color1;
    private Color color2;
    private Color color3;
    private float startEmission;

    void Start()
    {
        weatherType[0] = transform.GetChild(0).gameObject; // Light Rain
        weatherType[1] = transform.GetChild(1).gameObject; // Heavy Rain
        weatherType[2] = transform.GetChild(2).gameObject; // Snow

        stepComplete = true;
        weatherTimer = Random.Range(200.0f, 250.0f);
        weatherDuration = Random.Range(100.0f, 125.0f);
        //weatherTimer = Random.Range(2.0f, 2.5f);
        //weatherDuration = Random.Range(15.0f, 20.0f);

        ColorUtility.TryParseHtmlString("#FFF4D6", out colorInitial); // Normal
        ColorUtility.TryParseHtmlString("#B0B0B0", out color1); // Light Rain
        ColorUtility.TryParseHtmlString("#202020", out color2); // Heavy Rain
        ColorUtility.TryParseHtmlString("#CFCFCF", out color3); // Snow

        startEmission = 0.0f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > weatherTimer && !weatherTriggered)
        {
            timer = 0;
            weatherTimer = Random.Range(200.0f, 250.0f);
            //weatherTimer = Random.Range(15.0f, 20.0f);
            weatherTriggered = true;
            stepComplete = false;
        }
        else if (weatherTriggered && !stepComplete)
        {
            randomInt = Random.Range(0, 3);
            weatherType[randomInt].SetActive(true);
            switch (randomInt)
            {
                case 0: // Light Rain
                    StartCoroutine(LerpSky(color1));
                    StartCoroutine(LerpParticles(125.0f));
                    break;
                case 1: // Heavy Rain
                    StartCoroutine(LerpSky(color2));
                    StartCoroutine(LerpParticles(350.0f));
                    break;
                case 2: // Snow
                    StartCoroutine(LerpSky(color2));
                    StartCoroutine(LerpParticles(75.0f));
                    break;
                default:
                    break;
            }
            stepComplete = true;
        }
        else if (timer > weatherDuration && weatherTriggered)
        {
            timer = 0;           
            StartCoroutine(LerpSky(colorInitial)); // Normal
            StartCoroutine(LerpParticles(0.0f));
            weatherTriggered = false;          
        }        
    }

    private IEnumerator LerpSky(Color color)
    {
        float tempTimer = 0;
        Color startColor = LevelManager.global.DirectionalLightTransform.GetComponent<Light>().color;

        while (true)
        {
            tempTimer += Time.deltaTime / 7.5f;
            LevelManager.global.DirectionalLightTransform.GetComponent<Light>().color = Color.Lerp(startColor, color, tempTimer / 1.5f);
            if (tempTimer >= 1.5f)
            {
                yield break;
            }
            else
            {
                yield return 0;
            }
        }
    }

    private IEnumerator LerpParticles(float emissionStrength)
    {
        float tempTimer = 0;
        var emissionModule = weatherType[randomInt].GetComponent<ParticleSystem>().emission;

        while (true)
        {
            tempTimer += Time.deltaTime / 7.5f;
            emissionModule.rateOverTime = Mathf.Lerp(startEmission, emissionStrength, tempTimer / 1.75f);
            if (tempTimer >= 1.75f)
            {
                if (emissionStrength == 0.0f)
                {
                    weatherType[randomInt].SetActive(false);
                }
                startEmission = emissionStrength;
                yield break;
            }
            else
            {
                yield return 0;
            }
        }
    }
}