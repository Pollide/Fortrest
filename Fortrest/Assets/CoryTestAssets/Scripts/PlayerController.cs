using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using TMPro;


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


    //private float attackCooldown = 0.0f;
    //private float nextAttack;
    //private float lastClickedTime = 0;
    //private float maxComboDelay = 1;

    // Attacks
    public float attackDamage = 0.5f;
    public float attackTimer = 0.0f;
    private float resetAttack = 0.75f;
    public float comboTimer = 0.0f;
    private float resetCombo = 1.0f;
    public int attackCount = 0;

    CharacterController playerCC;
    public Animator CharacterAnimator;

    [Header("Player States")]
    public bool playerCanMove = true;
    public bool playerisMoving = false;
    public bool attacking = false;

    public List<Transform> enemyList = new List<Transform>();

    // Variable for movement direction
    private Vector3 moveDirection;

    private float footstepTimer;
    private bool noEnergy;
    private bool repaired;
    private bool sleeping;
    private GameObject house;
    private GameObject houseSpawnPoint;
    private GameObject destroyedHouse;
    private GameObject repairedHouse;
    public GameObject bodyShape;
    private GameObject interactText1;
    private GameObject interactText2;
    private GameObject interactText3;

    private VisualEffect VFXSlash;
    private VisualEffect VFXSleeping;

    private bool soundPlaying = false;

    public GameObject AxeGameObject;
    public GameObject HammerGameObject;
    public GameObject PicaxeGameObject;
    public GameObject SwordGameObject;
    public GameObject RadiusGameObject;
    private GameObject RadiusCamGameObject;


    public TMP_Text DayTMP_Text;
    public TMP_Text RemaningTMP_Text;
    public TMP_Text SurvivedTMP_Text;
    public TMP_Text enemyNumberText;
    public TMP_Text enemyNumberText2;

    public Animation UIAnimation;

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
        if (global)
        {
            //destroys the duplicate
            Destroy(transform.parent.gameObject);
        }
        else
        {
            //itself doesnt exist so set it
            global = this;
        }
    }

    private void Start()
    {
        LevelManager manager = LevelManager.global;
        VFXSlash = manager.transform.Find("VFX").Find("VFX_Slash").GetComponent<VisualEffect>();
        VFXSleeping = manager.transform.Find("VFX").Find("VFX_Sleeping").GetComponent<VisualEffect>();

        VFXSlash.Stop();
        VFXSleeping.Stop();

        playerEnergy = maxPlayerEnergy;
        playerEnergyBarImage.fillAmount = 0.935f;

        if (GameObject.Find("Radius Camera"))
        {
            RadiusCamGameObject = GameObject.Find("Radius Camera");
        }

        if (GameObject.Find("House"))
        {
            house = GameObject.Find("House");

            houseSpawnPoint = house.transform.Find("SpawnPoint").gameObject;
            destroyedHouse = house.transform.Find("Destroyed House").gameObject;
            repairedHouse = house.transform.Find("Repaired House").gameObject;

            interactText1 = house.transform.Find("Floating Text 1").gameObject;
            interactText2 = house.transform.Find("Floating Text 2").gameObject;
            interactText3 = house.transform.Find("Floating Text 3").gameObject;
        }

        RadiusGameObject.transform.localScale = new Vector3(PlayerModeHandler.global.distanceAwayFromPlayer * 2, 0.1f, PlayerModeHandler.global.distanceAwayFromPlayer * 2);
    }

    public void ChangeTool(ToolData toolData)
    {
        AxeGameObject.SetActive(toolData.AxeBool);
        HammerGameObject.SetActive(toolData.HammerBool);
        PicaxeGameObject.SetActive(toolData.PicaxeBool);
        SwordGameObject.SetActive(toolData.SwordBool);
        RadiusGameObject.SetActive(toolData.HammerBool);
        if (RadiusCamGameObject != null)
        {
            RadiusCamGameObject.SetActive(toolData.HammerBool);
        }
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

        if (house != null)
        {
            Sleep();
        }

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

        if (attacking)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= resetAttack)
            {
                attacking = false;
            }
        }

        if (attackCount >= 1)
        {
            comboTimer += Time.deltaTime;

            if (comboTimer >= resetCombo)
            {
                comboTimer = 0;
                attackCount = 0;
            }
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

    public void ApplyEnergyDamage(float amount, bool _attacking)
    {
        if (!_attacking)
        {
            CharacterAnimator.ResetTrigger("Swing");
            CharacterAnimator.SetTrigger("Swing");
        }

        playerEnergy -= amount;
    }

    public void ApplyEnergyRestore(float amount)
    {
        playerEnergy += amount;
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0) && !attacking && PlayerModeHandler.global.playerModes == PlayerModes.CombatMode && !PlayerModeHandler.global.MouseOverUI())
        {
            for (int i = 0; i < enemyList.Count; i++) // Goes through the list of targets
            {
                enemyList[i].GetComponent<EnemyController>().canBeDamaged = true;
            }

            attacking = true;
            attackTimer = 0;
            CharacterAnimator.ResetTrigger("Swing");
            CharacterAnimator.ResetTrigger("Swing2");
            CharacterAnimator.ResetTrigger("Swing3");
            if (attackCount == 0)
            {
                CharacterAnimator.SetTrigger("Swing");
                attackCount++;
                ApplyEnergyDamage(2.0f, true);
            }
            else if (attackCount == 1)
            {
                comboTimer = 0;
                CharacterAnimator.SetTrigger("Swing2"); // Play different animation here
                attackCount++;
                ApplyEnergyDamage(3.0f, true);
            }
            else if (attackCount == 2)
            {
                comboTimer = 0;
                CharacterAnimator.SetTrigger("Swing3"); // Play different animation here
                attackCount = 0;
                ApplyEnergyDamage(5.0f, true);
            }

            GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerAttackSound);
            GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.SwordSwing1Sound : GameManager.global.SwordSwing2Sound);
            VFXSlash.transform.position = transform.position;
            VFXSlash.transform.eulerAngles = transform.eulerAngles;
            VFXSlash.Play();

            //for (int i = 0; i < enemyList.Count; i++) // Goes through the list of targets
            //{
            //
            //    if (Vector3.Distance(transform.position, enemyList[i].transform.position) <= 3.0f && FacingEnemy(enemyList[i].transform.position)) // Distance from player to enemy
            //    {
            //        playerisAttacking = true;
            //        enemyList[i].GetComponent<EnemyController>().chasing = true;
            //        GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.EnemyHit1Sound : GameManager.global.EnemyHit2Sound);
            //        enemyList[i].GetComponent<EnemyController>().Damaged(attackDamage);
            //        break;
            //    }
            //}
        }
    }

    public IEnumerator FreezeTime()
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSeconds(0.025f);
        Time.timeScale = 1;
    }

    public void NewDay()
    {
        if (LevelManager.global.day == 1)
        {
            LevelManager.global.newDay = true;
            DayTMP_Text.text = "DAY TWO";
            RemaningTMP_Text.text = "THREE DAYS LEFT";
        }
        if (LevelManager.global.day == 2)
        {
            LevelManager.global.newDay = true;
            DayTMP_Text.text = "DAY THREE";
            RemaningTMP_Text.text = "TWO DAYS LEFT";
        }
        if (LevelManager.global.day == 3)
        {
            LevelManager.global.newDay = true;
            DayTMP_Text.text = "DAY FOUR";
            RemaningTMP_Text.text = "ONE DAY LEFT";
        }
        if (LevelManager.global.day == 4)
        {
            LevelManager.global.newDay = true;
            DayTMP_Text.text = "DAY FIVE";
            RemaningTMP_Text.text = "FINAL DAY";
        }

        if (LevelManager.global.day == 5)
        {
            LevelManager.global.newDay = true;
            DayTMP_Text.text = "YOU SURVIVED";
            RemaningTMP_Text.text = "How much longer can you survive?";

        }

        if (LevelManager.global.day > 5)
        {
            LevelManager.global.newDay = true;
            DayTMP_Text.text = "DAY " + (LevelManager.global.day + 1).ToString();
            RemaningTMP_Text.text = "Highscore: " + (PlayerPrefs.GetInt("Number of Days") + 1);

            if (LevelManager.global.day > PlayerPrefs.GetInt("Number of Days"))
            {
                RemaningTMP_Text.text = "Highscore Beaten!";
                PlayerPrefs.SetInt("Number of Days", LevelManager.global.day);
            }
        }
    }

    public void EnemiesTextControl()
    {
        //if (enemyList.Count > 0)
        if (enemyNumberText)
        {
            enemyNumberText.text = enemyList.Count.ToString();

            if (LevelManager.global.newDay)
            {
                LevelManager.global.newDay = false;
                StartCoroutine(TextAppearing());
            }
        }
    }

    private IEnumerator TextAppearing()
    {
        float fraction = 0.0f;
        while (fraction < 1.0f)
        {
            //   Debug.Log(fraction);
            fraction = LevelManager.global.DaylightTimer / 180.0f;
            enemyNumberText.color = LevelManager.global.textGradient.Evaluate(fraction);
            enemyNumberText2.color = LevelManager.global.textGradient.Evaluate(fraction);
            yield return null;
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
        if (Input.GetKeyDown(KeyCode.E) && (interactText2.activeSelf || interactText1.activeSelf || interactText3.activeSelf))
        {
            if (!repaired)
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.HouseBuiltNoiseSound, 0.5f);
                GameManager.global.SoundManager.PlaySound(GameManager.global.HouseBuiltSound, 0.3f);
                destroyedHouse.SetActive(false);
                repairedHouse.SetActive(true);
                interactText1.SetActive(false);
                repaired = true;
            }
            else
            {
                if (!sleeping)
                {
                    VFXSleeping.Play();
                    bodyShape.SetActive(false);
                    Vector3 sleepingVector = house.transform.position;
                    sleepingVector.y = transform.position.y;
                    transform.position = sleepingVector;
                    playerCanMove = false;
                    playerCC.enabled = false;
                    soundPlaying = false;
                    sleeping = true;
                    interactText2.SetActive(false);
                    interactText3.SetActive(true);
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
                    interactText2.SetActive(true);
                    interactText3.SetActive(false);
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