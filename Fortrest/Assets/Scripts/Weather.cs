using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather : MonoBehaviour
{
    public static Weather global;

    private GameObject[] weatherType = new GameObject[3];
    private float timer;
    private float weatherTimer;
    private float weatherDuration;
    private bool weatherTriggered;
    private bool stepComplete;
    private float startEmission;
    public float DecreaseDayLightIntensity;
    public int currentWeatherInt;
    private bool wasInTaiga;

    public bool DebugForceRain;
    public bool DebugEndRain;
    private void Awake()
    {
        global = this;
    }

    void Start()
    {
        weatherType[0] = transform.GetChild(0).gameObject; // Light Rain
        weatherType[1] = transform.GetChild(1).gameObject; // Heavy Rain
        weatherType[2] = transform.GetChild(2).gameObject; // Snow

        stepComplete = true;
        weatherTimer = Random.Range(200.0f, 250.0f);
        weatherDuration = Random.Range(100.0f, 150.0f);
        startEmission = 0.0f;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (DebugForceRain)
        {
            DebugForceRain = false;
            timer = weatherTimer;
        }

        if (DebugEndRain)
        {
            DebugEndRain = false;
            timer = weatherDuration;

        }
#endif
        if (LevelManager.global.currentTerrainData == LevelManager.global.terrainDataList[3])
        {
            wasInTaiga = true;
            currentWeatherInt = 2;
            weatherType[currentWeatherInt].SetActive(true);
            StartCoroutine(LerpParticles(90.0f));
            DecreaseDayLightIntensity = 0.25f;
        }
        else
        {
            if (wasInTaiga)
            {
                wasInTaiga = false;
                timer = 0;
                StartCoroutine(LerpParticles(0.0f));
                DecreaseDayLightIntensity = 0;
                weatherTriggered = false;
                stepComplete = true;
            }

            timer += Time.deltaTime;

            if (timer > weatherTimer && !weatherTriggered)
            {
                timer = 0;
                weatherTimer = Random.Range(200.0f, 250.0f);
                weatherTriggered = true;
                stepComplete = false;
            }
            else if (weatherTriggered && !stepComplete)
            {
                weatherDuration = Random.Range(10.0f, 15.0f);
                currentWeatherInt = Random.Range(0, 2);
                weatherType[currentWeatherInt].SetActive(true);
                switch (currentWeatherInt)
                {
                    case 0: // Light Rain
                        StartCoroutine(LerpParticles(125.0f));
                        DecreaseDayLightIntensity = 0.2f;
                        break;
                    case 1: // Heavy Rain
                        StartCoroutine(LerpParticles(350.0f));
                        DecreaseDayLightIntensity = 0.3f;
                        break;
                    default:
                        break;
                }
                stepComplete = true;
                GameManager.global.SoundManager.PlaySound(GameManager.global.RainSound);
            }
            else if (timer > weatherDuration && weatherTriggered)
            {
                GameManager.global.SoundManager.StopSelectedSound(GameManager.global.RainSound);
                timer = 0;
                StartCoroutine(LerpParticles(0.0f));
                DecreaseDayLightIntensity = 0;
                weatherTriggered = false;
            }
        }
    }

    private IEnumerator LerpParticles(float emissionStrength)
    {
        float tempTimer = 0;
        var emissionModule = weatherType[currentWeatherInt].GetComponent<ParticleSystem>().emission;

        while (true)
        {
            tempTimer += Time.deltaTime / 7.5f;
            emissionModule.rateOverTime = Mathf.Lerp(startEmission, emissionStrength, tempTimer / 1.75f);
            if (tempTimer >= 1.75f)
            {
                if (emissionStrength == 0.0f)
                {
                    weatherType[currentWeatherInt].SetActive(false);
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