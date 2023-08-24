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
    public float smoothTime;
    public float smoothTime2;
    private float maxSmooth = 0.6f;
    private float minSmooth = 0.4f;
    private float max;
    private float buildOffsetOrthoSize;
    private float buildOffsetPosX;
    private float buildOffsetPosZ;
    private float buildOffsetRot;
    private Vector3 initialRotation;
    private float initialOrthographicSize;
    private Vector3 direction;

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

    public void Update()
    {
        if (lockCamera)
        {
            if (!PlayerModeHandler.global.inTheFortress)
            {

                FocusOnTarget(false, PlayerController.global.transform.position, initialRotation);
            }
            else
            {
                FocusOnTarget(true, ReturnBuildOffset(), new(buildOffsetRot, 0, 0));
            }
        }
    }

    public Vector3 ReturnBuildOffset()
    {
        return new Vector3(PlayerController.global.transform.position.x + buildOffsetPosX, PlayerController.global.transform.position.y, PlayerController.global.transform.position.z + buildOffsetPosZ);
    }

    public void FocusOnTarget(bool build, Vector3 targetPosition, Vector3 offsetRotation)
    {
        GetComponent<Camera>().orthographicSize = build ? buildOffsetOrthoSize : initialOrthographicSize;

        cameraDistance = Vector3.Distance(targetPosition, transform.position);

        float i = cameraDistance / (max / (5f / 36) * (Time.deltaTime * 100));

        smoothTime = Mathf.Lerp(maxSmooth, minSmooth, i);

        if (!Boar.global.mounted)
        {
            direction = PlayerController.global.moveDirection;
        }
        else
        {
            direction = Boar.global.transform.forward;
            smoothTime *= 0.75f;
        }

        direction.Normalize();
        direction.y = 0;

        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) || (PlayerController.global.movingCTRL))
        {
            if (!Boar.global.mounted)
            {
                if (PlayerController.global.moveDirection != Vector3.zero)
                {
                    if (PlayerController.global.running)
                    {
                        transform.position = Vector3.SmoothDamp(transform.position, targetPosition + direction * 5f, ref currentVelocity, smoothTime);
                    }
                    else
                    {
                        transform.position = Vector3.SmoothDamp(transform.position, targetPosition + direction * 3.5f, ref currentVelocity, smoothTime);
                    }
                }                            
            }
            else if (Boar.global.currentSpeed > 0)
            {
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition + direction * 5.5f * Boar.global.currentSpeed / 90.0f, ref currentVelocity, smoothTime);
            }
            
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
        }

        if (transform.eulerAngles != offsetRotation)
        {
            transform.eulerAngles = offsetRotation;
        }
    }
}
