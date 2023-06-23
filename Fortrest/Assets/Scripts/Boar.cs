using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar : MonoBehaviour
{
    public static Boar global;

    private GameObject player;
    private GameObject text;
    bool textactive;
    public bool mounted = false;
    private bool inRange = false;

    private float maxSpeed = 0.75f;
    private float acceleration = 0.2f;
    private float deceleration = 0.0f;
    public float currentSpeed;
    private float currentTurn;
    private float turnAnglePerSec = 90.0f;
    private float verticalVelocity;
    private float gravity = -20.0f;

    private bool isMoving;
    public Animator animator;

    CharacterController cc;

    public AudioClip mountSound;
    public AudioClip dismountSound;
    public AudioClip stepSound;
    public AudioClip stepSound2;

    private void Awake()
    {
        global = this;
        currentSpeed = 0.0f;
        currentTurn = 0.0f;
        verticalVelocity = 0.0f;
    }

    void Start()
    {
        text = transform.GetChild(0).gameObject;
        player = PlayerController.global.gameObject;
        cc = GetComponent<CharacterController>();
        Indicator.global.AddIndicator(transform, Color.green, "Mount", false);
    }

    void Update()
    {
        DisplayText();
        if (Input.GetKeyDown(KeyCode.M) && inRange)
        {
            Mount();
        }
        if (mounted)
        {
            PlayerStick();
            PlayAnimations();
        }
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        if (mounted)
        {
            Ride();
        }
        else
        {
            if (currentSpeed >= 0)
            {
                Lerping(0.5f, 2.0f, ref deceleration, 2);
                currentSpeed -= deceleration * Time.fixedDeltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0.0f);
            }
        }
    }

    private void LateUpdate()
    {
        if (currentSpeed > 0.0f)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0.0f, currentTurn, 0.0f));
        }
        cc.Move(transform.forward * (currentSpeed / 10.0f) + new Vector3(0.0f, verticalVelocity, 0.0f));
        currentTurn = 0.0f;
    }

    private void DisplayText()
    {
        if (inRange)
        {
            if (mounted)
            {
                if (textactive)
                {
                    textactive = false;
                    LevelManager.FloatingTextChange(text, false);
                }
            }
            else
            {
                if (!textactive)
                {
                    textactive = true;
                    LevelManager.FloatingTextChange(text, true);
                }
            }
        }
        else
        {
            if (textactive)
            {
                textactive = false;
                LevelManager.FloatingTextChange(text, false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            inRange = false;
        }
    }

    void Mount()
    {
        mounted = !mounted;

        player.GetComponent<CharacterController>().enabled = false;
        if (mounted)
        {
            GameManager.global.SoundManager.PlaySound(mountSound, 1.0f);
            player.transform.position = new Vector3(transform.position.x, transform.position.y + 4.25f, transform.position.z);
            player.transform.rotation = transform.rotation;
            player.GetComponent<PlayerController>().playerCanMove = false;
            player.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("Moving", false);
            PlayerController.global.SwordGameObject.GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            GameManager.global.SoundManager.PlaySound(dismountSound, 1.0f);
            player.transform.position += transform.right * 2;
            player.transform.rotation = transform.rotation;
            player.GetComponent<PlayerController>().playerCanMove = true;
            PlayerController.global.CharacterAnimator.SetBool("Sitting", false);
            PlayerController.global.SwordGameObject.GetComponent<BoxCollider>().enabled = true;
        }
        player.GetComponent<CharacterController>().enabled = true;
    }

    void PlayerStick()
    {
        player.transform.position = new Vector3(transform.position.x, transform.position.y + 4.25f, transform.position.z);
        player.transform.rotation = transform.rotation;
        PlayerController.global.CharacterAnimator.SetBool("Sitting", true);
    }

    void Ride()
    {
        Lerping(0.2f, 0.4f, ref acceleration, 0.2667f); // Acceleration
        Lerping(0.5f, 2.0f, ref deceleration, 2); // Deceleration
        Lerping(75.0f, 90.0f, ref turnAnglePerSec, 20); // Turn

        if (Input.GetKey(KeyCode.W))
        {
            currentSpeed += acceleration * Time.fixedDeltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        else
        {
            currentSpeed -= deceleration * Time.fixedDeltaTime;
            currentSpeed = Mathf.Max(currentSpeed, 0.0f);
        }

        if (Input.GetKey(KeyCode.A))
        {
            currentTurn = -turnAnglePerSec * Time.fixedDeltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            currentTurn = turnAnglePerSec * Time.fixedDeltaTime;
        }
    }

    private void ApplyGravity()
    {
        // if (Physics.Raycast(transform.position, Vector3.down, Mathf.Infinity)) //only apply gravity if there is a ground
        // {
        if (cc.isGrounded && verticalVelocity < 0.0f)
        {
            verticalVelocity = -1.0f;
        }
        else
        {
            verticalVelocity += gravity * Time.fixedDeltaTime;
        }
        //}
    }

    void Lerping(float min, float max, ref float value, float dividerCoefficient)
    {
        float divider = max - min;
        float i = currentSpeed / (divider / dividerCoefficient);
        value = Mathf.Lerp(min, max, i);
    }

    private void PlayAnimations()
    {
        isMoving = (Input.GetKey(KeyCode.W)) || (currentSpeed >= 0.5f && !Input.GetKey(KeyCode.W));
        animator.speed = Mathf.Clamp(1 * (currentSpeed * 2), 0.5f, 1.5f);
        animator.SetBool("Moving", isMoving);
    }

    private void StepOne()
    {
        GameManager.global.SoundManager.PlaySound(stepSound, 0.25f);
    }

    private void StepTwo()
    {
        GameManager.global.SoundManager.PlaySound(stepSound2, 0.25f);
    }
}