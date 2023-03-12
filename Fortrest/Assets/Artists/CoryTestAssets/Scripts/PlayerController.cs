using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{
    public static PlayerController global;
    // Player Variables
    [Header("Player Variables")]
    public float playerCurrSpeed = 5f;
    public float playerMaxSpeed = 5f;
    public float playerSlowedSpeed = 5f;
    public float playerGravMultiplier = 3f;
    public float playerJumpHeight = 10f;
    public float playerEnergy = 100f;
    public float maxPlayerEnergy = 100f;

    public Image playerEnergyBarImage;

    private float playerGrav = -9.81f;
    private float playerVelocity;
    private float attackCooldown = 1.0f;
    private float nextAttack;
    private float attackDamage = 0.5f;

    CharacterController playerCC;
    public Animator CharacterAnimator;

    [Header("Player States")]
    public bool playerCanMove = true;
    public bool playerisMoving = false;
    public bool playerisAttacking = false;

    public List<Transform> enemyList = new List<Transform>();

    // Variable for movement direction
    private Vector3 moveDirection;

    private float footstepTimer;
    private bool noEnergy;
    private bool repaired;
    private bool sleeping;
    public GameObject house;
    public GameObject houseSpawnPoint;
    private GameObject destroyedHouse;
    private GameObject repairedHouse;
    public GameObject bodyShape;
    public GameObject interactText1;
    public GameObject interactText2;

    public VisualEffect VFXSlash;
    public VisualEffect VFXSleeping;

    private bool soundPlaying = false;

    public GameObject AxeGameObject;
    public GameObject HammerGameObject;
    public GameObject PicaxeGameObject;
    public GameObject SwordGameObject;
    public GameObject RadiusGameObject;
    public GameObject RadiusCamGameObject;
    [System.Serializable]
    public class ToolData
    {
        public bool AxeBool;
        public bool HammerBool;
        public bool PicaxeBool;
        public bool SwordBool;
    }

    // Start is called before the first frame update
    void Awake()
    {
        // Get Character controller that is attached to the player
        playerCC = GetComponent<CharacterController>();
        global = this;
    }

    private void Start()
    {
        VFXSlash.Stop();
        VFXSleeping.Stop();
        playerEnergy = maxPlayerEnergy;
        playerEnergyBarImage.fillAmount = 0.935f;
        destroyedHouse = house.transform.Find("Destroyed House").gameObject;
        repairedHouse = house.transform.Find("Repaired House").gameObject;
    }

    public void ChangeTool(ToolData toolData)
    {
        AxeGameObject.SetActive(toolData.AxeBool);
        HammerGameObject.SetActive(toolData.HammerBool);
        PicaxeGameObject.SetActive(toolData.PicaxeBool);
        SwordGameObject.SetActive(toolData.SwordBool);
        RadiusGameObject.SetActive(toolData.HammerBool);
        RadiusCamGameObject.SetActive(toolData.HammerBool);


    }

    // Update is called once per frame
    void Update()
    {
        // Only take input if movement isn't inhibited
        if (playerCanMove)
        {
            // Local veriables for input keys
            float horizontalMovement = Input.GetAxis("Horizontal");
            float verticalMovement = Input.GetAxis("Vertical");

            Jump();
            ApplyGravity();
            ApplyMovement(horizontalMovement, verticalMovement);
            Attack();
        }

        Sleep();

        if (sleeping)
        {
            playerEnergy += Time.deltaTime;
        }

        if (playerEnergy >= maxPlayerEnergy)
        {
            playerEnergy = maxPlayerEnergy;
            playerEnergyBarImage.fillAmount = 0.935f;
        }

        playerEnergyBarImage.fillAmount = Mathf.Lerp(0.320f, 0.935f, playerEnergy / maxPlayerEnergy);

        noEnergy = playerEnergy <= 0;
        playerCurrSpeed = noEnergy ? playerSlowedSpeed : playerMaxSpeed;

        if (noEnergy)
            playerEnergy = 0;

        if (playerisMoving)
        {
            footstepTimer += Time.deltaTime * (noEnergy ? 0.5f : 1.0f);
        }
        else
        {
            footstepTimer = 0;
        }
        if (footstepTimer > 0.35f && sleeping == false)
        {
            footstepTimer = 0;
            AudioClip step = Random.Range(0, 2) == 0 ? GameManager.global.Footstep1Sound : GameManager.global.Footstep2Sound;
            GameManager.global.SoundManager.PlaySound(step, 0.1f);
        }
    }

    // Player movement 
    private void ApplyMovement(float _horizontalMove, float _verticalMove)
    {
        playerisMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        CharacterAnimator.SetBool("Moving", playerisMoving);

        if (playerisMoving)
        {
            moveDirection = new Vector3(_horizontalMove, 0.0f, _verticalMove);

            if (moveDirection.magnitude > 1)
            {
                moveDirection.Normalize();
            }

            moveDirection *= playerCurrSpeed;

            moveDirection = Quaternion.AngleAxis(45, Vector3.up) * moveDirection;

            if (moveDirection != Vector3.zero)
            {
                transform.forward = moveDirection;
            }

            ApplyGravity();
        }
        else
        {
            moveDirection.x = 0f;
            moveDirection.z = 0f;
        }

        playerCC.Move(moveDirection * Time.deltaTime);
    }

    private void Jump()
    {
        if (!playerCC.isGrounded) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerVelocity += playerJumpHeight;
            GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerJumpSound);
        }
    }

    private void ApplyGravity()
    {
        // Run gravity
        if (playerCC.isGrounded && playerVelocity < 0.0f)
        {
            playerVelocity = -1.0f;
        }
        else
        {
            playerVelocity += playerGrav * playerGravMultiplier * Time.deltaTime;
        }

        moveDirection.y = playerVelocity;
    }

    public void ApplyEnergyDamage(float amount)
    {
        CharacterAnimator.ResetTrigger("Swing");
        CharacterAnimator.SetTrigger("Swing");

        playerEnergy -= amount;
    }

    public void ApplyEnergyRestore(float amount)
    {
        playerEnergy += amount;
    }

    private void Attack()
    {
        if (Input.GetMouseButton(0) && Time.time > nextAttack && PlayerModeHandler.global.playerModes == PlayerModes.CombatMode && !PlayerModeHandler.global.MouseOverUI())
        {
            ChangeTool(new ToolData() { SwordBool = true });

            VFXSlash.Play();
            ApplyEnergyDamage(5.0f);

            nextAttack = Time.time + attackCooldown;
            GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerAttackSound);
            GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.SwordSwing1Sound : GameManager.global.SwordSwing2Sound);
            for (int i = 0; i < enemyList.Count; i++) // Goes through the list of targets
            {
                if (Vector3.Distance(transform.position, enemyList[i].transform.position) <= 3.0f && FacingEnemy(enemyList[i].transform.position)) // Distance from player to enemy
                {
                    playerisAttacking = true;
                    enemyList[i].GetComponent<EnemyController>().chasing = true;
                    GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.EnemyHit1Sound : GameManager.global.EnemyHit2Sound);
                    enemyList[i].GetComponent<EnemyController>().Damaged(attackDamage);
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == destroyedHouse)
        {
            interactText1.SetActive(true);
        }
        else if (other.gameObject == repairedHouse)
        {
            interactText2.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == destroyedHouse)
        {
            interactText1.SetActive(false);
        }
        else if (other.gameObject == repairedHouse)
        {
            interactText2.SetActive(false);
        }
    }

    private void Sleep()
    {
        if (Input.GetKeyDown(KeyCode.E) && Vector3.Distance(transform.position, house.transform.position) <= 10.0f)
        {
            if (!repaired)
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.HouseBuiltNoiseSound, 0.5f);
                GameManager.global.SoundManager.PlaySound(GameManager.global.HouseBuiltSound, 0.3f);
                destroyedHouse.SetActive(false);
                repairedHouse.SetActive(true);
                interactText1.SetActive(false);
                repaired = true;
                ChangeTool(new ToolData() { HammerBool = true });
            }
            else
            {
                ChangeTool(new ToolData());
                if (!sleeping)
                {
                    VFXSleeping.Play();
                    bodyShape.SetActive(false);
                    Vector3 sleepingVector = house.transform.position;
                    sleepingVector.y = transform.position.y;
                    transform.position = sleepingVector;
                    playerCanMove = false;
                    playerCC.enabled = false;
                    sleeping = true;
                }
                else
                {
                    VFXSleeping.Stop();
                    transform.position = houseSpawnPoint.transform.position;
                    playerCanMove = true;
                    playerCC.enabled = true;
                    bodyShape.SetActive(true);
                    sleeping = false;
                    GameManager.global.SoundManager.StopSelectedSound(GameManager.global.SnoringSound);
                }
            }
        }

        if (sleeping && soundPlaying == false)
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.SnoringSound, 0.2f, true, 0, true);
            soundPlaying = true;
        }
    }

    private bool FacingEnemy(Vector3 enemyPosition) // Making sure the enemy always faces what it is attacking
    {
        Vector3 enemyDirection = (enemyPosition - transform.position).normalized; // Gets a direction using a normalized vector
        float angle = Vector3.Angle(transform.forward, enemyDirection);
        if (angle > -75.0f && angle < 75.0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}