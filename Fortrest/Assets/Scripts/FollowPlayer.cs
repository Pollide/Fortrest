using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Vector3 currentVelocity = Vector3.zero;
    private float distance;
    [SerializeField] private float smoothTime;
    [SerializeField] private float maxSmooth = 2.0f;
    [SerializeField] private float minSmooth = 1.0f;

    void Start()
    {
        transform.position = PlayerController.global.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Weather.global.currentWeatherInt == 2)
        {
            maxSmooth = 2.0f;
            minSmooth = 1.0f;
        }
        else
        {
            maxSmooth = 3.0f;
            minSmooth = 2.5f;
        }

        distance = Vector3.Distance(PlayerController.global.transform.position, transform.position);
        float i = distance / 25;
        smoothTime = Mathf.Lerp(minSmooth, maxSmooth, i);
        transform.position = Vector3.SmoothDamp(transform.position, PlayerController.global.transform.position, ref currentVelocity, smoothTime);   
    }
}
