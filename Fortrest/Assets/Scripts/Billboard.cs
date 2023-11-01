using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    void FixedUpdate()
    {

        if (!GameManager.ReturnInMainMenu())
            transform.LookAt(transform.position + LevelManager.global.SceneCamera.transform.forward);
    }
}
