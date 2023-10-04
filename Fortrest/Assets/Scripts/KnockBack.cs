using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBack : MonoBehaviour
{
    [HideInInspector] public float strength;
    private float mountStrength;
    [HideInInspector] public bool knock = false;
    [HideInInspector] public bool mountKnock;

    private void Start()
    {
        mountStrength = strength * 5;
    }

    private void Update()
    {
        if (knock)
        {
            if (mountKnock)
            {
                strength = mountStrength;
            }       
            else
            {
                strength = mountStrength / 5;
            }
            StartCoroutine(Push());
            mountKnock = false;
            knock = false;
        }
    }

    // Stop the pushback
    private IEnumerator Push()
    {
        Vector3 direction = (transform.position - PlayerController.global.transform.position).normalized;
        direction.y = 0.0f;       
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + direction;
        float fraction = 0.0f;

        while (fraction < 1.0f)
        {
            fraction += Time.deltaTime * strength;
            transform.position = Vector3.Lerp(startPos, targetPos, fraction);
            yield return null;
        }
    }
}