using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // private Vector3 offset;
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
        max = maxSmooth - minSmooth;
    }

    private void Update()
    {
        Vector3 targetPosition = PlayerController.global.transform.position;
        cameraDistance = Vector3.Distance(PlayerController.global.transform.position, transform.position);
        float i = cameraDistance / (max * 50);
        smoothTime = Mathf.Lerp(minSmooth, maxSmooth, i);

        if (Input.GetKeyDown(KeyCode.CapsLock) || PlayerController.global.lockingCTRL)
        {
            PlayerController.global.lockingCTRL = false;
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
