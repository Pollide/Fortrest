using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public bool test;

    void LateUpdate()
    {
        if (test)
        {
            transform.localPosition = new Vector3(0, 3.5f, 0);
        }
        transform.LookAt(transform.position + (Menu.global ? Menu.global.CameraTransform.forward : LevelManager.global.SceneCamera.transform.forward));
    }
}
