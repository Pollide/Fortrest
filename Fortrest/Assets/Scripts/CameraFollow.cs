using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform playerPosition;
    private Vector3 offset;
    private Vector3 currentVelocity = Vector3.zero;
    [SerializeField] private float cameraDistance;
    [SerializeField] private bool lockCamera;
    [SerializeField] private float smoothTime;
    [SerializeField] private float maxSmooth = 0.25f;
    [SerializeField] private float minSmooth = 0.01f;
    [SerializeField] private float max;
    
    private void Start()
    {
        lockCamera = true;
        playerPosition = GameObject.Find("Player").transform;
        offset = transform.position - playerPosition.position;
        max = maxSmooth - minSmooth;

        transform.position = playerPosition.position + new Vector3(0.0f, offset.y, 0.0f);
    }

    private void Update()
    {
        Vector3 targetPosition = playerPosition.position + new Vector3(0.0f, offset.y, 0.0f);
        cameraDistance = Vector3.Distance(targetPosition, transform.position);
        float i = cameraDistance / (max * 50);
        smoothTime = Mathf.Lerp(minSmooth, maxSmooth, i);

        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.CameraLockSound);
            lockCamera = !lockCamera;
            return;
        }

        if (lockCamera)
        {
            if (cameraDistance > 0.1f)
            {
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);               
            }
            else
            {
                transform.position = targetPosition;
            }
        }
    }
}
