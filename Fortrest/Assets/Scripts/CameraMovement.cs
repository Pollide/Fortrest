using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{  
    private float timer;
    private bool up, down, left, right;
    private float upF, downF, leftF, rightF;
    private bool moveUp, moveDown, moveLeft, moveRight;
    private float[] times;
    private float biggest;
    private KeyCode[] keyCodes;
    public Renderer gridRenderer;

    private void Start()
    {
        times = new float[4];
        keyCodes = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));
    }

    void Update()
    {
        if (CameraFollow.global.canMoveCamera)
        {
            timer += Time.deltaTime;           

            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                timer = 0f;             
            }
            else
            {
                foreach (KeyCode keyCode in keyCodes)
                {
                    BigBrainMode(keyCode);
                }              

                for (int i = 0; i < times.Length; i++)
                {
                    if (times[i] > biggest)
                    {
                        biggest = times[i];
                        switch (i)
                        {
                            case 0:
                                SetBoolean(ref moveUp);
                                break;
                            case 1:
                                SetBoolean(ref moveDown);
                                break;
                            case 2:
                                SetBoolean(ref moveLeft);
                                break;
                            case 3:
                                SetBoolean(ref moveRight);
                                break;
                        }
                    }
                }
                if (gridRenderer.isVisible)
                {
                    if (moveUp)
                    {
                        Move(0.125f, 0.125f);
                    }
                    if (moveDown)
                    {
                        Move(-0.125f, -0.125f);
                    }
                    if (moveLeft)
                    {
                        Move(-0.125f, 0.125f);
                    }
                    if (moveRight)
                    {
                        Move(0.125f, -0.125f);
                    }
                }
                else
                {
                    Vector3 direction = (PlayerController.global.house.transform.position - transform.position);
                    direction.Normalize();
                    Move(direction.x, direction.z);
                }
            }                                                                        
        }
    }

    private void BigBrainMode(KeyCode letter)
    {
        if (Input.GetKey(letter))
        {
            switch (letter)
            {
                case KeyCode.W:
                    Process(ref up, ref upF, 0);
                    break;
                case KeyCode.S:
                    Process(ref down, ref downF, 1);
                    break;
                case KeyCode.A:
                    Process(ref left, ref leftF, 2);
                    break;
                case KeyCode.D:
                    Process(ref right, ref rightF, 3);
                    break;
            }
        }       
        else
        {
            switch (letter)
            {
                case KeyCode.W:
                    Reset(ref up, 0);
                    break;
                case KeyCode.S:
                    Reset(ref down, 1);
                    break;
                case KeyCode.A:
                    Reset(ref left, 2);
                    break;
                case KeyCode.D:
                    Reset(ref right, 3);
                    break;
            }
        }      
    }

    private void Process(ref bool direction, ref float time, int i)
    {
        if (!direction)
        {
            time = timer;
            times[i] = time;
            direction = true;
        }
    }

    private void Reset(ref bool direction, int i)
    {
        if (biggest == times[i])
        {
            biggest = 0f;
        }
        times[i] = 0f;
        direction = false;
    }

    private void SetBoolean(ref bool boolean)
    {
        moveUp = false;
        moveDown = false;
        moveLeft = false;
        moveRight = false;
        boolean = true;
    }

    private void Move(float x, float z)
    {
        CameraFollow.global.cameraMoving = true;
        transform.position += new Vector3(x, 0.0f, z) * Time.deltaTime * 100.0f;
    }
}