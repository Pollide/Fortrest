using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // private Vector3 offset;
    public static CameraFollow global;
    private Vector3 currentVelocity = Vector3.zero;
    [SerializeField] private float cameraDistance;
    [SerializeField] private bool lockCamera;
    [SerializeField] private float smoothTime;
    [SerializeField] private float maxSmooth = 0.25f;
    [SerializeField] private float minSmooth = 0.01f;
    [SerializeField] private float max;
    [SerializeField] private float buildOffsetOrthoSize;
    [SerializeField] private float buildOffsetRot;
    private Vector3 initialRotation;
    private float initialOrthographicSize;

    private void Awake()
    {
        global = this;
        initialRotation = transform.eulerAngles;
    }

    private void Start()
    {
        initialOrthographicSize = GetComponent<Camera>().orthographicSize;
        lockCamera = true;
        max = maxSmooth - minSmooth;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.CapsLock) || PlayerController.global.lockingCTRL)
        {
            PlayerController.global.lockingCTRL = false;
            GameManager.global.SoundManager.PlaySound(GameManager.global.CameraLockSound);
            lockCamera = !lockCamera;
        }

        if (lockCamera)
        {
            if (PlayerModeHandler.global.playerModes != PlayerModes.BuildMode && PlayerModeHandler.global.playerModes != PlayerModes.RepairMode)
            {
                FocusOnTarget(PlayerController.global.transform.position);
            }
            else
            {
                BuildCam(PlayerController.global.transform.position);
            }
        }
    }

    private void FocusOnTarget(Vector3 targetPosition)
    {
        GetComponent<Camera>().orthographicSize = initialOrthographicSize;
        cameraDistance = Vector3.Distance(targetPosition, transform.position);
        float i = cameraDistance / (max * 50);
        smoothTime = Mathf.Lerp(minSmooth, maxSmooth, i);

        if (cameraDistance > 0.1f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
        }
        else
        {
            transform.position = targetPosition;
        }

        if (transform.eulerAngles != initialRotation)
        {
            transform.eulerAngles = initialRotation;
        }
    }

    public void BuildCam(Vector3 targetPosition)
    {
        Vector3 offsetRotation = new(buildOffsetRot, 0, 0);
        GetComponent<Camera>().orthographicSize = buildOffsetOrthoSize;
        cameraDistance = Vector3.Distance(targetPosition, transform.position);
        float i = cameraDistance / (max * 50);
        smoothTime = Mathf.Lerp(minSmooth, maxSmooth, i);

        if (cameraDistance > 0.1f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
            
        }
        else
        {
            transform.position = targetPosition;
        }

        if (transform.eulerAngles != offsetRotation)
        {
            transform.eulerAngles = offsetRotation;
        }
    }
}
