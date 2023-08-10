using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boar : MonoBehaviour
{
    public static Boar global;

    private GameObject player;
    private GameObject text;
    bool textactive;
    public bool mounted = false;
    public bool inRange = false;

    private float maxSpeed = 70f;
    public float acceleration = 10f;
    private float deceleration = 0.0f;
    public float currentSpeed;
    private float currentTurn;
    private float turnAnglePerSec = 90.0f;
    private float verticalVelocity;
    private float gravity = -20.0f;

    public bool canMove = true;
    public bool isMoving;
    public Animator animator;

    public CharacterController cc;

    public AudioClip mountSound;
    public AudioClip dismountSound;
    public AudioClip stepSound;
    public AudioClip stepSound2;

    private float bobbing = 0f;
    private bool reverse;
    public bool axe;

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
        Indicator.global.AddIndicator(transform, Color.cyan, "Mount", false, Indicator.global.MountSprite);
    }

    void Update()
    {
        if (PlayerController.global.pausedBool)
        {
            return;
        }

        if (Time.timeScale == 0)
            return;

        DisplayText();

        if ((Input.GetKeyDown(KeyCode.E) || PlayerController.global.interactCTRL) && inRange && !PlayerController.global.playerDead && !PlayerController.global.canTeleport && !PlayerController.global.canGetInHouse)
        {
            Mount();
        }
        if (mounted)
        {
            PlayerStick();
            PlayAnimations();
        }

        if (inRange)
        {
            PlayerController.global.needInteraction = true;
        }
        else if (!inRange && !PlayerController.global.playerDead && !PlayerController.global.canTeleport && !PlayerController.global.canGetInHouse)
        {
            PlayerController.global.needInteraction = false;
        }

        ApplyGravity();

        if (mounted)
        {
            if (canMove)
            {
                Ride();
            }
        }
        else
        {
            if (currentSpeed > 0)
            {
                Lerping(0.5f, 2.0f, ref deceleration, 2);
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0.0f);
                animator.speed = Mathf.Clamp(1 * (currentSpeed * 2), 0.5f, 1.5f);
                if (currentSpeed < 0.15f)
                {
                    animator.SetBool("Moving", false);
                }
            }
        }

        if (canMove)
        {
            if (currentSpeed > 0.0f)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0.0f, currentTurn, 0.0f));
            }
            cc.Move((transform.forward * (currentSpeed / 8.0f) * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f));
            currentTurn = 0.0f;
        }
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

    private void OnTriggerStay(Collider other)
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
        PlayerController.global.interactCTRL = false;
        player.GetComponent<CharacterController>().enabled = false;

        if (mounted)
        {
            GameManager.global.SoundManager.PlaySound(mountSound, 1.0f);
            player.transform.position = new Vector3(transform.position.x, transform.position.y + 4.25f, transform.position.z);
            player.transform.rotation = transform.rotation;
            player.GetComponent<PlayerController>().playerCanMove = false;
            PlayerController.global.CharacterAnimator.SetBool("Moving", false);
            PlayerController.global.ChangeTool(new PlayerController.ToolData() { HandBool = true });
            GetComponent<NavMeshObstacle>().enabled = false;
        }
        else
        {
            GameManager.global.SoundManager.PlaySound(dismountSound, 1.0f);
            player.transform.position += transform.right * 2;
            player.transform.rotation = transform.rotation;
            player.GetComponent<PlayerController>().playerCanMove = true;
            PlayerController.global.CharacterAnimator.SetBool("Sitting", false);
            PlayerController.global.ChangeTool(new PlayerController.ToolData() { SwordBool = PlayerModeHandler.global.playerModes == PlayerModes.CombatMode, AxeBool = PlayerController.global.lastWasAxe, PickaxeBool = !PlayerController.global.lastWasAxe });
            GetComponent<NavMeshObstacle>().enabled = true;
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
        Lerping(5, maxSpeed / 2, ref acceleration, 5); // Acceleration
        Lerping(0.5f, maxSpeed * 2, ref deceleration, 2f); // Deceleration
        Lerping(30.0f, 45.0f, ref turnAnglePerSec, 20f); // Turn

        if (Input.GetKey(KeyCode.W) || PlayerController.global.moveCTRL.y > 0)
        {
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        else
        {
            currentSpeed -= deceleration * Time.deltaTime;
            currentSpeed = Mathf.Max(currentSpeed, 0.0f);
        }

        if (Input.GetKey(KeyCode.A) || PlayerController.global.moveCTRL.x < -0.35f)
        {
            currentTurn = -turnAnglePerSec * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || PlayerController.global.moveCTRL.x > 0.35f)
        {
            currentTurn = turnAnglePerSec * Time.deltaTime;
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
            verticalVelocity += gravity * Time.deltaTime;
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
        if (canMove)
        {
            isMoving = (Input.GetKey(KeyCode.W) || PlayerController.global.moveCTRL.y > 0) || (currentSpeed >= 0.5f && (!Input.GetKey(KeyCode.W) || PlayerController.global.moveCTRL.y > 0));
            animator.speed = Mathf.Clamp(1 * (currentSpeed * 2), 0.5f, 1.5f);
            animator.SetBool("Moving", isMoving);
            if (isMoving)
            {
                if (bobbing > -5f && !reverse)
                {
                    bobbing -= Time.deltaTime * (currentSpeed);
                }
                else if (bobbing == -5f)
                {
                    reverse = true;
                }
                if (bobbing < 5f && reverse)
                {
                    bobbing += Time.deltaTime * (currentSpeed);
                }
                else if (bobbing == 5f)
                {
                    reverse = false;
                }
                bobbing = Mathf.Clamp(bobbing, -5f, 5f);
                player.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + bobbing);
            }
            else
            {
                bobbing = 0f;
                reverse = false;
            }
        }
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