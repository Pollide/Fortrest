using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        transform.LookAt(transform.position + (Menu.global ? Menu.global.CameraTransform.forward : LevelManager.global.SceneCamera.transform.forward));
    }
}
