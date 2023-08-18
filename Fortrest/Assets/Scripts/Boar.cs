using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boar : MonoBehaviour
{
    public static Boar global;
    [HideInInspector] public GameObject text;
    private bool textactive;
    [HideInInspector] public bool mounted = false;
    [HideInInspector] public bool inRange = false;

    private float maxSpeed = 90f;
    private float acceleration = 0f;
    private float deceleration = 0.0f;
    public float currentSpeed;
    private float currentTurn;
    private float turnAnglePerSec = 0.0f;
    private float verticalVelocity;
    private float gravity = -20.0f;

    [HideInInspector] public bool canMove = true;
    private bool isMoving;
    private bool isReversing;

    public Animator animator;
    [HideInInspector] public CharacterController cc;

    public AudioClip mountSound;
    public AudioClip dismountSound;
    public AudioClip stepSound;
    public AudioClip stepSound2;

    private float bobbing = 0f;
    private bool reverse;
    private bool midAir;

    public GameObject body;
    public GameObject house;
    [HideInInspector] public bool closerToHouse;
    public bool canInteractWithBoar;

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

        closerToHouse = Vector3.Distance(PlayerController.global.transform.position, house.transform.position) < Vector3.Distance(PlayerController.global.transform.position, transform.position) ? true : false;

        DisplayText();

        if (!midAir && inRange && !PlayerController.global.playerDead && !PlayerController.global.canTeleport && (mounted || (!mounted && !closerToHouse)) && !PlayerController.global.teleporting)
        {
            canInteractWithBoar = true;
            PlayerController.global.needInteraction = true;
        }
        else
        {
            canInteractWithBoar = false;
            if (!PlayerModeHandler.global.canInteractWithHouse && !PlayerController.global.canTeleport)
            {
                PlayerController.global.needInteraction = false;
            }           
        }

        if ((Input.GetKeyDown(KeyCode.E) || PlayerController.global.interactCTRL) && canInteractWithBoar)
        {           
            Mount();                      
        }
        if (mounted)
        {
            PlayerStick();
            PlayAnimations();
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
                Lerping(100f, 300f, ref deceleration, 20 / 9f); // Deceleration
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0.0f);
                animator.speed = Mathf.Clamp(1 * (currentSpeed * 2), 0.5f, 1.5f);
                if (currentSpeed < 0.15f)
                {
                    animator.SetBool("Moving", false);
                }
                if (currentSpeed > 0.15f)
                {
                    animator.SetBool("Reversing", false);
                }
            }
        }

        if (canMove)
        {
            if (currentSpeed > 0.0f || currentSpeed < 0.0f)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0.0f, currentTurn, 0.0f));
            }
            cc.Move((transform.forward * (currentSpeed / 8.0f) * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f));
            currentTurn = 0.0f;
        }
    }

    private void DisplayText()
    {
        if (canInteractWithBoar)
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
        if (other.gameObject == PlayerController.global.gameObject)
        {
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerController.global.gameObject)
        {
            inRange = false;
        }
    }

    void Mount()
    {
        mounted = !mounted;
        PlayerController.global.interactCTRL = false;
        PlayerController.global.GetComponent<CharacterController>().enabled = false;

        if (mounted)
        {
            GameManager.global.SoundManager.PlaySound(mountSound, 1.0f);
            PlayerController.global.transform.position = new Vector3(transform.position.x, transform.position.y + 4.25f, transform.position.z);
            PlayerController.global.transform.rotation = transform.rotation;
            PlayerController.global.GetComponent<CharacterController>().enabled = true;
            PlayerController.global.GetComponent<PlayerController>().playerCanMove = false;
            PlayerController.global.CharacterAnimator.SetBool("Moving", false);
            PlayerController.global.ChangeTool(new PlayerController.ToolData() { HandBool = true });
            GetComponent<NavMeshObstacle>().enabled = false;
        }
        else
        {
            GameManager.global.SoundManager.PlaySound(dismountSound, 1.0f);
            PlayerController.global.transform.position += transform.right * 2;
            PlayerController.global.transform.rotation = transform.rotation;
            PlayerController.global.GetComponent<CharacterController>().enabled = true;
            PlayerController.global.GetComponent<PlayerController>().playerCanMove = true;
            PlayerController.global.CharacterAnimator.SetBool("Sitting", false);
            PlayerController.global.ChangeTool(new PlayerController.ToolData() { SwordBool = PlayerModeHandler.global.playerModes == PlayerModes.CombatMode, AxeBool = PlayerController.global.lastWasAxe, PickaxeBool = !PlayerController.global.lastWasAxe });
            GetComponent<NavMeshObstacle>().enabled = true;
            StartCoroutine(MidAir());
        }
    }

    private IEnumerator MidAir()
    {
        PlayerController.global.canEvade = false;
        midAir = true;
        yield return new WaitForSeconds(0.45f);
        PlayerController.global.canEvade = true;
        midAir = false;
    }

    void PlayerStick()
    {
        PlayerController.global.transform.position = new Vector3(transform.position.x, transform.position.y + 4.25f, transform.position.z);
        PlayerController.global.transform.rotation = transform.rotation;
        PlayerController.global.CharacterAnimator.SetBool("Sitting", true);
    }

    void Ride()
    {
        Lerping(40f, 60f, ref acceleration, 2 / 9f); // Acceleration
        Lerping(100f, 300f, ref deceleration, 20 / 9f); // Deceleration
        Lerping(50f, 70f, ref turnAnglePerSec, 2 / 9f); // Turn

        if (Input.GetKey(KeyCode.W) || PlayerController.global.moveCTRL.y > 0)
        {
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        else if (!Input.GetKey(KeyCode.S) || PlayerController.global.moveCTRL.y !< 0)
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

        if (Input.GetKey(KeyCode.S) || PlayerController.global.moveCTRL.y < 0)
        {
            currentSpeed -= acceleration * Time.deltaTime;
            currentSpeed = Mathf.Max(- 45.0f, currentSpeed);
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
            isReversing = (Input.GetKey(KeyCode.S) || PlayerController.global.moveCTRL.y < 0) || (currentSpeed <= -0.5f && (!Input.GetKey(KeyCode.S) || PlayerController.global.moveCTRL.y < 0));
            if (isMoving)
            {
                animator.speed = Mathf.Clamp(1 * ((currentSpeed / 120.0f) * 2.0f), 0.5f, 1.5f);               
            }
            if (isReversing)
            {
                animator.speed = Mathf.Clamp(1 * ((currentSpeed / 60.0f) * 2.0f), 0.5f, 1.5f);             
            }
            animator.SetBool("Moving", isMoving);
            animator.SetBool("Reversing", isReversing);
            if (isMoving)
            {
                if (bobbing > -5f && !reverse)
                {
                    bobbing -= Time.deltaTime * ((currentSpeed / 120.0f) * 75f);
                }
                else if (bobbing == -5f)
                {
                    reverse = true;
                }
                if (bobbing < 5f && reverse)
                {
                    bobbing += Time.deltaTime * ((currentSpeed / 120.0f) * 75f);
                }
                else if (bobbing == 5f)
                {
                    reverse = false;
                }
                bobbing = Mathf.Clamp(bobbing, -5f, 5f);
                PlayerController.global.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + bobbing);
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
        if (isMoving)
        {
            GameManager.global.SoundManager.PlaySound(stepSound, 0.25f);
        }
    }

    private void StepTwo()
    {
        if (isMoving)
        {
            GameManager.global.SoundManager.PlaySound(stepSound2, 0.25f);
        }
    }
}