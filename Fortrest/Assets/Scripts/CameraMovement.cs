using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public static CameraMovement global;

    private float timer;
    private bool up, down, left, right;
    private float upF, downF, leftF, rightF;
    private bool moveUp, moveDown, moveLeft, moveRight;
    private float[] times;
    private float biggest;
    private KeyCode[] keyCodes;
    private float xMove, yMove, xMin, yMin, xMax, yMax;
    private Vector2 cameraCTRL;

    private void Awake()
    {
        global = this;
    }

    private void Start()
    {
        times = new float[4];
        keyCodes = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));

        GameManager.global.gamepadControls.Controls.Rotate.performed += context => cameraCTRL = context.ReadValue<Vector2>();
        GameManager.global.gamepadControls.Controls.Rotate.canceled += context => cameraCTRL = Vector2.zero;
    }

    void Update()
    {
        if (CameraFollow.global.canMoveCamera)
        {
            timer += Time.deltaTime;

            xMin = -4.4f;
            xMax = 4.6f;
            float offsetX = Mathf.Abs(2f - yMove);
            xMin = xMin + (offsetX / 1.6f);
            xMax = xMax - (offsetX / 1.6f);

            yMin = -2.15f;
            yMax = 6.5f;
            float offsetY = Mathf.Abs(0.01f - xMove);
            yMin = yMin + (offsetY / 1.6f);
            yMax = yMax - (offsetY / 1.6f);

            if (GameManager.global.KeyboardBool)
            {
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
                    
                    if (moveUp && yMove < yMax)
                    {
                        Move(0.125f, 0.125f);
                        yMove += Time.deltaTime;
                    }
                    if (moveDown && yMove > yMin)
                    {
                        Move(-0.125f, -0.125f);
                        yMove -= Time.deltaTime;
                    }
                    if (moveLeft && xMove > xMin)
                    {
                        Move(-0.125f, 0.125f);
                        xMove -= Time.deltaTime;
                    }
                    if (moveRight && xMove < xMax)
                    {
                        Move(0.125f, -0.125f);
                        xMove += Time.deltaTime;
                    }
                }
            } 
            else
            {
                if (Mathf.Abs(cameraCTRL.y) > Mathf.Abs(cameraCTRL.x))
                {
                    if (cameraCTRL.y > 0.1f && yMove < yMax)
                    {
                        Move(0.125f, 0.125f);
                        yMove += Time.deltaTime;
                    }
                    else if (cameraCTRL.y < -0.1f && yMove > yMin)
                    {
                        Move(-0.125f, -0.125f);
                        yMove -= Time.deltaTime;
                    }
                }
                else
                {
                    if (cameraCTRL.x < -0.1f && xMove > xMin)
                    {
                        Move(-0.125f, 0.125f);
                        xMove -= Time.deltaTime;
                    }
                    else if (cameraCTRL.x > 0.1f && xMove < xMax)
                    {
                        Move(0.125f, -0.125f);
                        xMove += Time.deltaTime;
                    }
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

    public void ResetAll()
    {
        xMove = 0f;
        yMove = 0f;
    }
}