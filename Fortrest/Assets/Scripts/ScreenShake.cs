using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake global;

    public float duration = 0.1f;
    public bool shake = false;
    public AnimationCurve curve;

    private void Awake()
    {
        global = this;
    }

    void Update()
    {
        if (shake)
        {
            
            StartCoroutine(Shaking());
        }
    }

    IEnumerator Shaking()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0.0f;       
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }
        shake = false;
        transform.position = startPosition;
    }
}
