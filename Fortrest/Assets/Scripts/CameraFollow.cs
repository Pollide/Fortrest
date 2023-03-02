using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 offset;
    public Transform playerPosition;
    public float smoothTime;
    private Vector3 currentVelocity = Vector3.zero;
    private bool lockCamera = true;
    public float cameraDistance;

    private void Awake()
    {
        offset = transform.position - playerPosition.position;
    }

    private void Update()
    {
        Vector3 targetPosition = playerPosition.position + offset;
        cameraDistance = Vector3.Distance(targetPosition, transform.position);

        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            lockCamera = !lockCamera;
            return;
        }

        if (lockCamera)
        {
            if (Vector3.Distance(targetPosition, transform.position) > 0.1f)
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
