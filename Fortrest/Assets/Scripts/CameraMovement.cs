using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (CameraFollow.global.canMoveCamera)
        {
            if (Input.GetKey(KeyCode.W))
            {
                CameraFollow.global.cameraMoving = true;
                transform.position += new Vector3(0.125f, 0.0f, 0.125f);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                CameraFollow.global.cameraMoving = true;
                transform.position += new Vector3(-0.125f, 0.0f, -0.125f);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                CameraFollow.global.cameraMoving = true;
                transform.position += new Vector3(-0.125f, 0.0f, 0.125f);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                CameraFollow.global.cameraMoving = true;
                transform.position += new Vector3(0.125f, 0.0f, -0.125f);
            }
        }
    }
}
