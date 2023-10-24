using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class Boar : MonoBehaviour
{
    public static Boar global;
    [HideInInspector] public GameObject text;
    private bool textactive;
    [HideInInspector] public bool mounted = false;
    [HideInInspector] public bool inRange = false;

    public float maxSpeed = 90f;
    public float acceleration = 0f;
    private float deceleration = 0.0f;
    public float currentSpeed;
    private float currentTurn;
    private float turnAnglePerSec = 0.0f;

    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool isMoving;
    public bool isReversing;

    public Animator animator;

    public AudioClip mountSound;
    public AudioClip dismountSound;
    public AudioClip stepSound;
    public AudioClip stepSound2;

    private bool midAir;

    public GameObject body;
    GameObject house;
    [HideInInspector] public bool closerToHouse;
    public bool canInteractWithBoar;
    private bool dismountRight;

    private float timer = 0f;

    private void Awake()
    {
        global = this;
        currentSpeed = 0.0f;
        currentTurn = 0.0f;
    }

    void Start()
    {
        if (GameManager.ReturnInMainMenu())
        {
            enabled = false;
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<SphereCollider>().enabled = false;
            return;
        }

        text = transform.GetChild(0).gameObject;

        Indicator.global.AddIndicator(transform, Color.green, "Mount", false, Indicator.global.MountSprite);
        house = PlayerController.global.house;
    }

    void Update()
    {
        if (Time.timeScale != 0)
        {
            closerToHouse = Vector3.Distance(PlayerController.global.transform.position, house.transform.position) < Vector3.Distance(PlayerController.global.transform.position, transform.position) ? true : false;

            DisplayText();

            if (!midAir && inRange && !PlayerController.global.playerDead && !PlayerController.global.canTeleport && (mounted || (!mounted && !closerToHouse)) && !PlayerController.global.teleporting && !PlayerController.global.bridgeInteract)
            {
                canInteractWithBoar = true;
                PlayerController.global.needInteraction = true;
            }
            else
            {
                canInteractWithBoar = false;
                if (!PlayerModeHandler.global.canInteractWithHouse && !PlayerController.global.canTeleport && PlayerController.global.playerRespawned && !PlayerController.global.bridgeInteract)
                {
                    PlayerController.global.needInteraction = false;
                }
            }

            if ((Input.GetKeyDown(KeyCode.E) || PlayerController.global.interactCTRL) && canInteractWithBoar)
            {
                Mount();
            }
        }

        if (mounted)
        {
            PlayerStick();
            PlayAnimations();
            if (canMove)
            {
                SmokeTrail();
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
                animator.speed = Mathf.Clamp(1 * ((currentSpeed / 120.0f) * 2.0f), 0.5f, 1.5f);
                if (currentSpeed < 0.15f)
                {
                    animator.SetBool("Moving", false);
                }
            }
            if (currentSpeed < 0)
            {
                Lerping(100f, 300f, ref deceleration, 20 / 9f); // Deceleration
                currentSpeed += deceleration * Time.deltaTime;
                currentSpeed = Mathf.Min(currentSpeed, 0.0f);
                animator.speed = Mathf.Clamp(1 * ((currentSpeed / 60.0f) * 2.0f), 0.5f, 1.5f);
                if (currentSpeed > -0.15f)
                {
                    animator.SetBool("Reversing", false);
                }
            }
            if (currentSpeed == 0)
            {
                animator.SetBool("Moving", false);
                animator.SetBool("Reversing", false);
                transform.position = transform.position;
            }
        }

        if (canMove)
        {
            if (currentSpeed > 0.0f || currentSpeed < 0.0f)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0.0f, currentTurn, 0.0f));
            }

            Vector3 direction = transform.forward + Vector3.up * 3 * (currentSpeed > 0 ? 1 : -1); //searches area in front of boar or behind if reversing
            Collider[] colliders = Physics.OverlapSphere(transform.position + direction, 2, GameManager.ReturnBitShift(new string[] { "Default", "Building", "Resource" }), QueryTriggerInteraction.Ignore);

            if (colliders.Length == 0) //stops boar hitting trees and buildings
            {
                MoveBoar(); //move boar when nothing being collided
            }

            for (int i = 0; i < colliders.Length; i++)//shakes the building without any damage just for visual fun
            {
                Building building = colliders[i].GetComponentInParent<Building>();

                if (building && !building.GetComponent<Animation>().IsPlaying("Nature Shake")) //stops building shaking too often
                {
                    GameManager.global.SoundManager.PlaySound(GameManager.global.BushBreakingSound, 1.0f);
                    building.HealthAnimation();
                    break;
                }
            }

            colliders = Physics.OverlapSphere(transform.position - transform.up, 3, GameManager.ReturnBitShift(new string[] { "Default" }), QueryTriggerInteraction.Ignore);

            if (colliders.Length == 0) //fall to the abyss if walk off the cliff of an island
            {
                transform.position = Vector3.Lerp(transform.position, transform.position - transform.up, 10 * Time.deltaTime);
            }

            currentTurn = 0.0f;
        }
    }

    public void MoveBoar()
    {
        transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward, (currentSpeed / 8.0f) * Time.deltaTime);
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
        if (PlayerController.global && other.gameObject == PlayerController.global.gameObject)
        {
            inRange = true;
        }
        if (Vector3.Distance(transform.right, other.transform.position) > Vector3.Distance(-transform.right, other.transform.position))
        {
            dismountRight = true;
        }
        else
        {
            dismountRight = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (PlayerController.global && other.gameObject == PlayerController.global.gameObject)
        {
            inRange = false;
        }
    }

    public void Mount()
    {
        mounted = !mounted;
        PlayerController.global.interactCTRL = false;
        PlayerController.global.GetComponent<CharacterController>().enabled = false;

        if (mounted)
        {
            PlayerController.global.evading = false;
            GameManager.global.SoundManager.PlaySound(mountSound, 1.0f);
            PlayerController.global.transform.position = new Vector3(transform.position.x, transform.position.y + 4.65f, transform.position.z);
            PlayerController.global.transform.rotation = transform.rotation;
            PlayerController.global.GetComponent<CharacterController>().enabled = true;
            PlayerController.global.GetComponent<PlayerController>().playerCanMove = false;
            PlayerController.global.characterAnimator.SetBool("Moving", false);
            PlayerController.global.ChangeTool(new PlayerController.ToolData() { HandBool = true });
            GetComponent<NavMeshObstacle>().enabled = false;
        }
        else
        {
            GameManager.global.SoundManager.PlaySound(dismountSound, 1.0f);
            if (dismountRight)
            {
                PlayerController.global.transform.position += transform.right * 2;
            }
            else
            {
                PlayerController.global.transform.position += -transform.right * 2;
            }
            PlayerController.global.transform.rotation = transform.rotation;
            PlayerController.global.GetComponent<CharacterController>().enabled = true;
            PlayerController.global.GetComponent<PlayerController>().playerCanMove = true;
            PlayerController.global.characterAnimator.SetBool("Sitting", false);
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
        PlayerController.global.transform.position = new Vector3(transform.position.x, transform.position.y + 4.65f, transform.position.z);
        PlayerController.global.transform.rotation = transform.rotation;
        PlayerController.global.characterAnimator.SetBool("Sitting", true);
        float temp = currentSpeed / 90f;
        PlayerController.global.characterAnimator.SetFloat("MountSpeed", temp);
    }

    void Ride()
    {
        Lerping(40f, 60f, ref acceleration, 2 / 9f); // Acceleration
        Lerping(100f, 300f, ref deceleration, 20 / 9f); // Deceleration
        Lerping(50f, 70f, ref turnAnglePerSec, 2 / 9f); // Turn

        if (Input.GetKey(KeyCode.W) || GameManager.global.moveCTRL.y > 0)
        {
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        else if (!isReversing)
        {
            currentSpeed -= deceleration * Time.deltaTime;
            currentSpeed = Mathf.Max(currentSpeed, 0.0f);
        }

        if (Input.GetKey(KeyCode.A) || GameManager.global.moveCTRL.x < -0.35f)
        {
            currentTurn = -turnAnglePerSec * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || GameManager.global.moveCTRL.x > 0.35f)
        {
            currentTurn = turnAnglePerSec * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S) || GameManager.global.moveCTRL.y < 0)
        {
            currentSpeed -= acceleration * Time.deltaTime;
            currentSpeed = Mathf.Max(-45.0f, currentSpeed);
        }
        else if (!isMoving)
        {
            currentSpeed += deceleration * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, 0.0f);
        }
    }

    /*
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
    */

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
            isMoving = (Input.GetKey(KeyCode.W) || GameManager.global.moveCTRL.y > 0) || (currentSpeed >= 0.5f && (!Input.GetKey(KeyCode.W) || GameManager.global.moveCTRL.y > 0));
            isReversing = (Input.GetKey(KeyCode.S) || GameManager.global.moveCTRL.y < 0) || (currentSpeed <= -0.5f && (!Input.GetKey(KeyCode.S) || GameManager.global.moveCTRL.y < 0));
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

    public bool IsMoving()
    {
        if (isMoving)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SmokeTrail()
    {
        timer += Time.deltaTime;
        if (isMoving && timer > 0.08f)
        {
            timer = 0f;
            GameObject SmokeVFXRight = Instantiate(LevelManager.global.VFXSmoke.gameObject, transform.position + new Vector3(0f, 0.5f, 0f) - (Vector3.forward * 1.25f) - Vector3.right, Quaternion.identity);
            float randomeFloat = Random.Range(0.2f, 0.275f);
            SmokeVFXRight.transform.localScale = new Vector3(randomeFloat * 2f, randomeFloat, randomeFloat * 2f);
            SmokeVFXRight.GetComponent<VisualEffect>().Play();
            Destroy(SmokeVFXRight, 1.5f);
        }
    }
}