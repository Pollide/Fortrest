using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // private Vector3 offset;
    public static CameraFollow global;
    private Vector3 currentVelocity = Vector3.zero;
    [SerializeField] private float cameraDistance;
    public float smoothTime;
    public float smoothTime2;
    private float maxSmooth = 0.6f;
    private float minSmooth = 0.4f;
    private float max;
    private float buildOffsetPosX;
    private float buildOffsetPosZ;
    private float buildOffsetRot;
    private Vector3 initialRotation;
    private Vector3 direction;
    public float distance;
    public bool canMoveCamera;
    public bool cameraMoving;
    public bool bossCam;

    private void Awake()
    {
        global = this;
        initialRotation = transform.eulerAngles;
    }

    private void Start()
    {
        max = maxSmooth - minSmooth;
    }

    public void Update()
    {
        if (!bossCam)
        {
            FocusOnTarget(PlayerController.global.transform.position + offset(), initialRotation);
        }
    }

    public Vector3 offset()
    {
        return transform.forward * -60;
    }

    public void FocusOnTarget(Vector3 targetPosition, Vector3 offsetRotation)
    {
        if (PlayerModeHandler.global.inTheFortress)
        {
            if (!cameraMoving)
            {
                transform.position = Vector3.SmoothDamp(transform.position, PlayerController.global.house.transform.position + offset(), ref currentVelocity, 0.2f);
                if (Vector3.Distance(transform.position, PlayerController.global.house.transform.position + offset()) < 2.0f)
                {
                    canMoveCamera = true;
                }
            }
        }
        else
        {
            cameraMoving = false;
            canMoveCamera = false;

            if (!Boar.global || !Boar.global.mounted)
            {
                if (GameManager.global.KeyboardBool)
                {
                    distance = Vector3.Distance(targetPosition, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    distance = Mathf.Clamp(distance, 0f, 12f);
                    distance /= 12f;
                }
                else
                {
                    distance = Mathf.Clamp(Mathf.Abs(PlayerController.global.rotateCTRL.y) + Mathf.Abs(PlayerController.global.rotateCTRL.x), 0f, 1f);
                }

                transform.position = Vector3.SmoothDamp(transform.position, targetPosition + PlayerController.global.transform.forward, ref currentVelocity, 0.2f);
            }
            else
            {
                //cameraDistance = Vector3.Distance(targetPosition, transform.position);
                //float i = cameraDistance / (max / (5f / 36) * (Time.deltaTime * 100));
                //smoothTime = Mathf.Lerp(maxSmooth, minSmooth, i);

                direction = Boar.global.transform.forward;
                direction.Normalize();
                direction.y = 0;

                if (((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) || (PlayerController.global.movingCTRL)) && !Boar.global.isReversing)
                {
                    transform.position = Vector3.SmoothDamp(transform.position, targetPosition + direction * 6.0f * Boar.global.currentSpeed / 90.0f, ref currentVelocity, 0.6f);
                }
                else
                {
                    transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, 0.75f);
                }
            }

            if (transform.eulerAngles != offsetRotation)
            {
                transform.eulerAngles = offsetRotation;
            }
        }
    }
}
