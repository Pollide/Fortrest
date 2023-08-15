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
    [SerializeField] private float buildOffsetPosX;
    [SerializeField] private float buildOffsetPosZ;
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

    public void Update()
    {
        //if (Input.GetKeyDown(KeyCode.CapsLock) || PlayerController.global.lockingCTRL)
        //{
        //    PlayerController.global.lockingCTRL = false;
        //    GameManager.global.SoundManager.PlaySound(GameManager.global.CameraLockSound);
        //    lockCamera = !lockCamera;
        //}

        if (lockCamera)
        {
            if (PlayerModeHandler.global.playerModes != PlayerModes.BuildMode && PlayerModeHandler.global.playerModes != PlayerModes.RepairMode && PlayerModeHandler.global.playerModes != PlayerModes.UpgradeMenu)
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

        float i = cameraDistance / (max * 50);
        smoothTime = Mathf.Lerp(minSmooth, maxSmooth, i);


        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);

        if (transform.eulerAngles != offsetRotation)
        {
            transform.eulerAngles = offsetRotation;
        }
    }
}
