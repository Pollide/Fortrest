using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController global;

    GamepadControls gamepadControls;
    public Bow bowScript;

    // Components
    CharacterController playerCC;
    public Animator CharacterAnimator;

    // Speed
    public float playerCurrentSpeed = 0f;
    private float playerWalkSpeed = 5.0f;
    private float playerSprintSpeed = 8.0f;
    private float playerBowSpeed = 3.0f;
    public bool running = false;
    private float runTimer = 0.0f;
    private bool canRun = true;

    // Evade
    private float evadeTimer = 0.0f;
    private float evadeCoolDown = 2.5f;
    [HideInInspector] public bool evading = false;
    private bool canEvade;
    [HideInInspector] public bool playerCanBeDamaged = true;
    private Vector3 newPosition;
    private bool blocked = false;

    // Shooting
    private bool canShoot;
    [HideInInspector] public float bowDamage = 1.5f;
    private float bowTimer = 0.0f;
    private float resetBow = 1.5f;
    private bool shooting = false;
    private bool directionSaved = false;
    private Quaternion tempDirection;
    [HideInInspector] public float arrowNumber = 10.0f;

    // Spawn Turret
    private bool turretSpawned;
    [HideInInspector] public bool turretEnd;
    private float turretTimer = 0.0f;
    private float resetTurret = 30.0f;
    private float turretDuration = 20.0f;
    public GameObject miniTurretObject;

    // Gravity
    public float playerGravMultiplier = 3f;
    private float playerGrav = -9.81f;
    private float playerVelocity;

    // Energy
    [HideInInspector]
    public float playerEnergy = 0f;
    private float maxPlayerEnergy = 100f;
    public Image playerEnergyBarImage;
    private float energySpeed = 12.5f;

    // Health
    private bool deathEffects = false;
    private bool playerDead = false;
    public float playerHealth = 0.0f;
    [HideInInspector] public float maxHealth = 100.0f;
    [HideInInspector] public int appleAmount = 0;
    private float appleHealAmount = 10.0f;
    [HideInInspector] public int maxApple = 5;

    //public float playerJumpHeight = 10f;

    // Attacks
    [HideInInspector] public float attackDamage = 1.0f;
    private float attackTimer = 0.0f;
    private float resetAttack = 0.95f;
    private float comboTimer = 0.0f;
    private float resetCombo = 1.20f;
    public int attackCount = 0;
    public Building currentResource;
    public bool damageEnemy = false;

    // States
    [Header("Player States")]
    public bool playerCanMove = true;
    public bool playerisMoving = false;
    public bool attacking = false;

    // Movement
    private Vector3 moveDirection;
    private float horizontalMovement;
    private float verticalMovement;
    private GameObject house;
    private GameObject houseSpawnPoint;
    public GameObject bodyShape;
    private GameObject interactText;

    // Teleporter
    [HideInInspector] public bool canTeleport= false;

    // VFXs
    private VisualEffect VFXSlash;
    private VisualEffect VFXSleeping;
    private VisualEffect VFXSparks;
    private VisualEffect VFXPebble;
    private VisualEffect VFXWoodChip;

    public GameObject AxeGameObject;
    public GameObject HammerGameObject;
    public GameObject PicaxeGameObject;
    public GameObject SwordGameObject;
    public GameObject BowGameObject;
    public GameObject RadiusGameObject;
    private GameObject RadiusCamGameObject;
    public GameObject PauseCanvasGameObject;

    bool PausedBool;

    public TMP_Text DayTMP_Text;
    public TMP_Text RemaningTMP_Text;
    public TMP_Text SurvivedTMP_Text;
    public TMP_Text enemyText;
    public TMP_Text enemyAmountText;
    public TMP_Text houseUnderAttackText;
    public TMP_Text enemyDirectionText;
    public TMP_Text arrowText;
    public TMP_Text appleText;
    public TMP_Text turretText;

    public GameObject DarkenGameObject;

    public Animation UIAnimation;

    [System.Serializable]
    public class ToolData
    {
        public bool AxeBool;
        public bool HammerBool;
        public bool PicaxeBool;
        public bool SwordBool;
        public bool HandBool;
        public bool BowBool;
    }

    private float respawnTimer = 0.0f;
    private int lastAmount = 0;

    public HealthBar healthBar;
    private bool textAnimated = false;
    private bool gathering = false;
    private float gatherTimer = 0.0f;
    private float resetGather = 1.25f;

    public Image[] redSlashes;
    public Image redBorders;
    private bool displaySlash;
    private bool animationPlayed = false;
    private float timer1, timer2, timer3, timer4 = 0.0f;
    private float[] timers = new float[4];
   
    // Controller
    public bool usingController = false;    
    private bool sprintingCTRL;
    private bool movingCTRL;
    private bool gatheringCTRL;
    private bool attackingCTRL;   
    [HideInInspector] public bool interactCTRL;
    [HideInInspector] public bool needInteraction = false;
    [HideInInspector] public bool lockingCTRL = false;
    [HideInInspector] public bool inventoryCTRL = false;
    [HideInInspector] public bool swapCTRL = false;
    private Vector2 moveCTRL;
    private Vector2 rotateCTRL;

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

        // Controller stuff
        gamepadControls = new GamepadControls();
        // Left stick click to sprint
        gamepadControls.Controls.Sprint.performed += context => SprintController(true);
        gamepadControls.Controls.Sprint.canceled += context => SprintController(false);
        // Left stick to move
        gamepadControls.Controls.Move.performed += context => moveCTRL = context.ReadValue<Vector2>();
        gamepadControls.Controls.Move.canceled += context => moveCTRL = Vector2.zero;
        gamepadControls.Controls.Move.performed += context => MoveController(true);
        gamepadControls.Controls.Move.canceled += context => MoveController(false);
        // Pause button to pause
        gamepadControls.Controls.Pause.performed += context => PauseController();
        // Right trigger for gathering
        gamepadControls.Controls.Gathering.performed += context => GatheringController(true);
        gamepadControls.Controls.Gathering.canceled += context => GatheringController(false);
        // Right trigger for attacking
        gamepadControls.Controls.Attacking.performed += context => AttackingController();
        // A to interact // So far only used to respawn
        gamepadControls.Controls.Interact.performed += context => InteractController();
        // Select to lock / unlock camera
        gamepadControls.Controls.CameraLock.performed += context => lockingCTRL = true;
        // X to open / close inventory
        gamepadControls.Controls.Inventory.performed += context => inventoryCTRL = true;
        // Y to swap tool
        gamepadControls.Controls.Swap.performed += context => swapCTRL = true;
    }

    private void SprintController(bool pressed)
    {
        if (pressed)
        {
            sprintingCTRL = true;
        }
        else
        {
            sprintingCTRL = false;
        }
    }

    private void MoveController(bool pressed)
    {
        if (pressed)
        {
            movingCTRL = true;
        }
        else
        {
            movingCTRL = false;
        }
    }

    private void PauseController()
    {
        PauseVoid(!PausedBool);
    }

    private void GatheringController(bool pressed)
    {
        if (PlayerModeHandler.global.playerModes == PlayerModes.ResourceMode)
        {
            if (pressed)
            {
                gatheringCTRL = true;
            }
            else
            {
                gatheringCTRL = false;
            }
        }     
    }

    private void AttackingController()
    {
        if (PlayerModeHandler.global.playerModes == PlayerModes.CombatMode)
        {
            if (!attackingCTRL && !attacking)
            {
                attackingCTRL = true;
            }            
        }        
    }

    private void InteractController()
    {
        if (!interactCTRL && needInteraction)
        {
            interactCTRL = true;
        }        
    }

    private void OnEnable()
    {
        gamepadControls.Controls.Enable();
    }

    private void OnDisable()
    {
        gamepadControls.Controls.Disable();
    }

    private void Start()
    {
        LevelManager manager = LevelManager.global;
        VFXSlash = manager.transform.Find("VFX").Find("VFX_Slash").GetComponent<VisualEffect>();
        VFXSleeping = manager.transform.Find("VFX").Find("VFX_Sleeping").GetComponent<VisualEffect>();
        VFXSparks = manager.transform.Find("VFX").Find("VFX_Sparks").GetComponent<VisualEffect>();
        VFXPebble = manager.transform.Find("VFX").Find("VFX_Pebble").GetComponent<VisualEffect>();
        VFXWoodChip = manager.transform.Find("VFX").Find("VFX_Woodchips").GetComponent<VisualEffect>();
        VFXSlash.Stop();
        VFXSleeping.Stop();
        VFXSparks.Stop();
        VFXPebble.Stop();
        VFXWoodChip.Stop();

        if (GameObject.Find("Radius Camera"))
        {
            RadiusCamGameObject = GameObject.Find("Radius Camera");
        }
        if (GameObject.Find("House"))
        {
            house = GameObject.Find("House");
            houseSpawnPoint = house.transform.Find("SpawnPoint").gameObject;
            interactText = house.transform.Find("Floating Text").gameObject;
        }
        RadiusGameObject.transform.localScale = new Vector3(PlayerModeHandler.global.distanceAwayFromPlayer * 2, 0.1f, PlayerModeHandler.global.distanceAwayFromPlayer * 2);

        playerCurrentSpeed = playerWalkSpeed;
        playerEnergy = maxPlayerEnergy;
        playerEnergyBarImage.fillAmount = 0.935f;
        playerHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        timers[0] = timer1;
        timers[1] = timer2;
        timers[2] = timer3;
        timers[3] = timer4;

        canEvade = true;

        arrowText.text = "Arrow: " + arrowNumber.ToString();
        appleText.text = appleAmount.ToString();
    }

    public void ChangeTool(ToolData toolData)
    {
        AxeGameObject.SetActive(toolData.AxeBool);
        HammerGameObject.SetActive(toolData.HammerBool);
        PicaxeGameObject.SetActive(toolData.PicaxeBool);
        SwordGameObject.SetActive(toolData.SwordBool);
        BowGameObject.SetActive(toolData.BowBool);
        RadiusGameObject.SetActive(toolData.HammerBool);
        if (RadiusCamGameObject != null)
        {
            RadiusCamGameObject.SetActive(toolData.HammerBool);
        }
    }

    private void HandleSpeed()
    {
        if ((Input.GetKey(KeyCode.LeftShift) || sprintingCTRL) && canRun && CharacterAnimator.GetBool("Moving") == true && !canShoot)
        {
            running = true;
        }
        else
        {
            running = false;
        }

        if (canShoot)
        {
            playerCurrentSpeed = playerBowSpeed;
        }
        else
        {
            if (running)
            {
                playerCurrentSpeed = playerSprintSpeed;
                CharacterAnimator.speed = 1.6f;
            }
            else
            {
                playerCurrentSpeed = playerWalkSpeed;
                CharacterAnimator.speed = 1.0f;
            }
        }      

        if (!canRun)
        {
            runTimer += Time.deltaTime;
            if (runTimer > 2.5f)
            {
                canRun = true;
                runTimer = 0;
            }
        }
    }

    private void HandleEnergy()
    {
        if (running)
        {
            playerEnergy -= energySpeed * Time.deltaTime;
        }
        else
        {
            playerEnergy += energySpeed * Time.deltaTime;
        }

        playerEnergyBarImage.fillAmount = Mathf.Lerp(0.0f, 0.5f, playerEnergy / maxPlayerEnergy);

        if (playerEnergy >= maxPlayerEnergy)
        {
            playerEnergy = maxPlayerEnergy;
            playerEnergyBarImage.fillAmount = 0.5f;
        }
        else if (playerEnergy <= 0)
        {
            playerEnergy = 0;
            canRun = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // FOR TESTING
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.O))
        {
            playerHealth -= 30.0f;
            healthBar.SetHealth(playerHealth);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            playerHealth += 30.0f;
            healthBar.SetHealth(playerHealth);
        }
#endif
        if (evading)
        {
            evadeTimer += Time.deltaTime;
            playerCanBeDamaged = false;
            if (!blocked)
            {
                transform.position = Vector3.Lerp(transform.position, newPosition, evadeTimer / 30.0f);
            }            
            return;
        }
        else
        {
            playerCanBeDamaged = true;
            blocked = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseVoid(!PausedBool);
        }

        if (playerCanMove)
        {
            if (moveCTRL.x != 0 || moveCTRL.y != 0)
            {
                horizontalMovement = moveCTRL.x;
                verticalMovement = moveCTRL.y;
            }
            else if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                horizontalMovement = Input.GetAxis("Horizontal");
                verticalMovement = Input.GetAxis("Vertical");
            }          
            
            HandleSpeed();
            HandleEnergy();
            ApplyGravity();
            ApplyMovement(horizontalMovement, verticalMovement);
            Attack();
            Gathering();
            Shoot();
            if (Input.GetKeyDown(KeyCode.Space) && canEvade)
            {
                StartCoroutine(Evade());
            }
            if (Input.GetKeyDown(KeyCode.R) && appleAmount > 0)
            {
                EatApple();
            }
            if (Input.GetKeyDown(KeyCode.F) && !turretSpawned)
            {
                SpawnTurret();
            }
            if (Input.GetKeyDown(KeyCode.T) && canTeleport)
            {
                Teleport();
            }
        }

        if (attacking)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= resetAttack)
            {
                attacking = false;
                damageEnemy = false;
            }
        }

        if (attackCount >= 0)
        {
            comboTimer += Time.deltaTime;

            if (comboTimer >= resetCombo)
            {
                comboTimer = 0;
                attackCount = 0;
            }
        }

        if (gathering)
        {
            gatherTimer += Time.deltaTime;

            if (gatherTimer >= resetGather)
            {
                gathering = false;
            }
        }

        if (shooting)
        {
            bowTimer += Time.deltaTime;

            if (bowTimer >= resetBow)
            {
                shooting = false;
            }
        }

        if (turretSpawned)
        {
            if (!turretText.gameObject.activeSelf)
            {
                turretText.gameObject.SetActive(true);
            }
            turretTimer += Time.deltaTime;
            int seconds = 30 - (int)turretTimer % 60;
            if (seconds != 0)
            {
                turretText.text = seconds.ToString();
            }           

            if (turretTimer >= resetTurret)
            {
                turretSpawned = false;
            }

            if (turretTimer >= turretDuration)
            {
                turretEnd = true;
            }
        }
        else
        {
            if (turretText.gameObject.activeSelf)
            {
                turretText.gameObject.SetActive(false);
            }
        }

        ScreenDamage();

        if (playerHealth <= 0 || playerDead)
        {
            Death();
        }
    }

    private IEnumerator Evade()
    {       
        evadeTimer = 0;
        canEvade = false;
        evading = true;
        CharacterAnimator.ResetTrigger("Evade");
        CharacterAnimator.SetTrigger("Evade");
        newPosition = transform.position + (transform.forward * 7.0f);

        yield return new WaitForSeconds(evadeCoolDown);
        canEvade = true;
    }

    public void PauseVoid(bool pauseBool)
    {
        PauseCanvasGameObject.SetActive(pauseBool);
        PausedBool = pauseBool;
        GameManager.PlayAnimator(UIAnimation.GetComponent<Animator>(), "Pause Appear", pauseBool);
        GameManager.global.MusicManager.PlayMusic(pauseBool ? GameManager.global.PauseMusic : LevelManager.global.ReturnNight() ? GameManager.global.NightMusic : LevelManager.global.ActiveBiomeMusic);
        Time.timeScale = pauseBool ? 0 : 1;
    }

    private void ApplyMovement(float _horizontalMove, float _verticalMove)
    {
        playerisMoving = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) || movingCTRL;

        CharacterAnimator.SetBool("Moving", playerisMoving);

        if (playerisMoving)
        {
            moveDirection = new Vector3(_horizontalMove, 0.0f, _verticalMove);

            if (moveDirection.magnitude > 1)
            {
                moveDirection.Normalize();
            }

            moveDirection *= playerCurrentSpeed;

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

    public void HealthRestore(float amount)
    {
        playerHealth += amount;
        healthBar.SetHealth(playerHealth);
    }

    private void Attack()
    {
        if ((Input.GetMouseButtonDown(0) || attackingCTRL) && !canShoot && !attacking && PlayerModeHandler.global.playerModes == PlayerModes.CombatMode && !PlayerModeHandler.global.MouseOverUI())
        {
            attackingCTRL = false;
            attacking = true;
            attackTimer = 0;
            comboTimer = 0;
            LevelManager.ProcessEnemyList((enemy) =>
            {
                enemy.canBeDamaged = true;
            });

            CharacterAnimator.ResetTrigger("Swing");
            CharacterAnimator.ResetTrigger("Swing2");
            CharacterAnimator.ResetTrigger("Swing3");

            if (attackCount == 0)
            {
                resetAttack = 0.95f;
                resetCombo = 1.2f;
                attackDamage = 1.0f;
                CharacterAnimator.SetTrigger("Swing");
            }
            else if (attackCount == 1)
            {
                resetAttack = 0.9f;
                resetCombo = 1.15f;
                attackDamage = 1.0f;
                CharacterAnimator.SetTrigger("Swing2");
            }
            else if (attackCount == 2)
            {
                resetAttack = 0.95f;
                resetCombo = 1.2f;
                attackDamage = 1.5f;
                CharacterAnimator.SetTrigger("Swing3");
            }
        }
    }

    private void Gathering()
    {
        LevelManager.ProcessBuildingList((building) =>
        {
            float minDistanceFloat = 4;
            float distanceFloat = Vector3.Distance(PlayerController.global.transform.position, building.position);

            if (Facing(building.position, 75.0f) && !gathering && building.GetComponent<Building>().health > 0 && distanceFloat < minDistanceFloat && (Input.GetMouseButton(0) || gatheringCTRL) && PlayerModeHandler.global.playerModes == PlayerModes.ResourceMode)
            {
                gathering = true;
                gatherTimer = 0;
                currentResource = building.GetComponent<Building>();
                ChangeTool(new ToolData() { AxeBool = currentResource.resourceObject == Building.BuildingType.Wood, PicaxeBool = currentResource.resourceObject == Building.BuildingType.Stone, HandBool = currentResource.resourceObject == Building.BuildingType.Bush });
                CharacterAnimator.ResetTrigger("Swing");
                CharacterAnimator.SetTrigger("Swing");
            }

        }, true); //true means natural
    }

    private void Shoot()
    {
        if (PlayerModeHandler.global.playerModes == PlayerModes.CombatMode)
        {
            if (Input.GetMouseButton(1))
            {                
                arrowText.gameObject.SetActive(true);
                ChangeTool(new ToolData() { BowBool = true });
                canShoot = true;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (!directionSaved)
                    {
                        tempDirection = transform.rotation;
                        directionSaved = true;
                    }
                    transform.rotation = tempDirection;
                    CharacterAnimator.SetBool("Moving", false);
                }
                else
                {
                    directionSaved = false;
                }
            }
            else
            {
                arrowText.gameObject.SetActive(false);
                ChangeTool(new ToolData() { SwordBool = true });
                canShoot = false;                
            }

            if (Input.GetMouseButtonDown(0) && canShoot && !shooting && arrowNumber > 0)
            {
                shooting = true;
                bowTimer = 0;
                arrowNumber -= 1;
                GameManager.PlayAnimation(arrowText.GetComponent<Animation>(), "EnemyAmount");
                arrowText.text = "Arrow: " + arrowNumber.ToString();
                bowScript.Shoot();
            }
        }  
    }

    private void SpawnTurret()
    {
        turretTimer = 0;
        turretSpawned = true;
        turretEnd = false;
        GameObject miniTurret = Instantiate(miniTurretObject, transform.position + (transform.forward * 2) - (Vector3.up * (transform.position.y - 0.48f)), transform.rotation);     
    }

    private void Teleport()
    {
        gameObject.GetComponent<CharacterController>().enabled = false;
        transform.position = houseSpawnPoint.transform.position;
        canTeleport = false;
        gameObject.GetComponent<CharacterController>().enabled = true;
    }

    private void EatApple()
    {
        HealthRestore(appleHealAmount);
        appleAmount -= 1;
        GameManager.PlayAnimation(appleText.GetComponent<Animation>(), "EnemyAmount");
        appleText.text = appleAmount.ToString();
    }

    public void AttackEffects()
    {
        VFXSlash.transform.position = transform.position;
        VFXSlash.transform.eulerAngles = transform.eulerAngles;
        if (attackCount == 0 || attackCount == 2)
        {
            VFXSlash.transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y - 15.0f, transform.eulerAngles.z + 180.0f);
        }
        else if (attackCount == 1)
        {
            VFXSlash.transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
        }
        VFXSlash.Play();

        if (PlayerModeHandler.global.playerModes == PlayerModes.CombatMode)
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerAttackSound);
            GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.SwordSwing1Sound : GameManager.global.SwordSwing2Sound);

            attackCount++;
            if (attackCount > 2)
            {
                attackCount = 0;
            }
        }
    }

    public void GatheringEffects()
    {
        if (PicaxeGameObject.activeSelf)
        {
            GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.Pickaxe2Sound : GameManager.global.Pickaxe3Sound);

            VFXSparks.transform.position = currentResource.transform.position;
            VFXSparks.Play();
            VFXPebble.transform.position = currentResource.transform.position;
            VFXPebble.Play();
        }
        if (currentResource != null)
        {
            if (AxeGameObject.activeSelf && currentResource.resourceObject != Building.BuildingType.Bush)
            {
                GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.TreeChop1Sound : GameManager.global.TreeChop2Sound);

                VFXWoodChip.transform.position = currentResource.transform.position;
                VFXWoodChip.Play();
            }
            else if (currentResource.resourceObject == Building.BuildingType.Bush)
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.BushSound);
            }
            currentResource.TakeDamage(1);
            currentResource.healthBarImage.fillAmount = Mathf.Clamp(currentResource.health / currentResource.maxHealth, 0, 1f);
            if (currentResource.health == 0)
            {
                currentResource.GiveResources();
                currentResource.DestroyBuilding();
            }
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

    public void DisplayEnemiesDirection(LevelManager.SPAWNDIRECTION direction)
    {
        enemyDirectionText.text = "The enemies are coming from the " + direction.ToString();
        GameManager.PlayAnimation(enemyDirectionText.GetComponent<Animation>(), "EnemyDirection");
    }

    public void EnemiesTextControl()
    {
        if (enemyAmountText)
        {
            int goblinsInt = 0;

            LevelManager.ProcessEnemyList((enemy) =>
            {
                if (enemy.currentEnemyType != EnemyController.ENEMYTYPE.wolf)
                {
                    goblinsInt++;
                }
            });

            if (lastAmount != goblinsInt)
            {
                GameManager.PlayAnimation(enemyAmountText.GetComponent<Animation>(), "EnemyAmount");
                lastAmount = goblinsInt;
            }

            enemyAmountText.text = goblinsInt.ToString();

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
            enemyText.color = LevelManager.global.textGradient.Evaluate(fraction);
            enemyAmountText.color = LevelManager.global.textGradient.Evaluate(fraction);
            yield return null;
        }
    }

    private void Death()
    {
        if (!deathEffects)
        {
            // lose inventory
            GameManager.global.SoundManager.PlaySound(GameManager.global.SnoringSound, 0.2f, true, 0, true);
            VFXSleeping.Play();
            Vector3 sleepingVector = house.transform.position;
            sleepingVector.y = transform.position.y;
            transform.position = sleepingVector;
            playerCanMove = false;
            playerCC.enabled = false;
            bodyShape.SetActive(false);
            playerDead = true;
            deathEffects = true;
        }
        if (playerDead)
        {
            respawnTimer += Time.deltaTime;
            playerHealth = Mathf.Lerp(0.0f, maxHealth, respawnTimer / 15.0f);
            healthBar.SetHealth(playerHealth);
            if (respawnTimer >= 15.0f)
            {
                needInteraction = true;
                if (!textAnimated)
                {
                    LevelManager.FloatingTextChange(interactText, true);
                    textAnimated = true;
                }
                if (Input.GetKeyDown(KeyCode.E) || interactCTRL)
                {
                    GameManager.global.SoundManager.StopSelectedSound(GameManager.global.SnoringSound);
                    VFXSleeping.Stop();
                    transform.position = houseSpawnPoint.transform.position;
                    playerCanMove = true;
                    playerCC.enabled = true;
                    bodyShape.SetActive(true);
                    playerDead = false;
                    deathEffects = false;
                    respawnTimer = 0.0f;
                    LevelManager.FloatingTextChange(interactText, false);
                    textAnimated = false;
                    interactCTRL = false;
                    needInteraction = false;
                }
            }
        }
    }

    public void FirstStep()
    {
        GameManager.global.SoundManager.PlaySound(GameManager.global.Footstep1Sound, 0.05f);
    }

    public void SecondStep()
    {
        GameManager.global.SoundManager.PlaySound(GameManager.global.Footstep2Sound, 0.05f);
    }

    public void TakeDamage(float damage)
    {
        playerHealth -= damage;
        healthBar.SetHealth(playerHealth);
        displaySlash = true;
    }

    private bool Facing(Vector3 otherPosition, float desiredAngle) // Making sure the enemy always faces what it is attacking
    {
        Vector3 enemyDirection = (otherPosition - transform.position).normalized; // Gets a direction using a normalized vector
        float angle = Vector3.Angle(transform.forward, enemyDirection);
        if (angle > -desiredAngle && angle < desiredAngle)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ScreenDamage()
    {
        if (displaySlash)
        {
            int randomSlash = Random.Range(0, 4);
            redSlashes[randomSlash].color = new Color(redSlashes[randomSlash].color.r, redSlashes[randomSlash].color.g, redSlashes[randomSlash].color.b, 1.0f);
            timers[randomSlash] = 0.0f;
            displaySlash = false;
        }
        if (playerHealth <= 20.0f)
        {
            redBorders.gameObject.SetActive(true);
            if (!animationPlayed)
            {
                GameManager.PlayAnimation(redBorders.gameObject.GetComponent<Animation>(), "HealthWarning");
                animationPlayed = true;
            }
        }
        else
        {
            redBorders.gameObject.SetActive(false);
            animationPlayed = false;
            GameManager.PlayAnimation(redBorders.gameObject.GetComponent<Animation>(), "HealthWarning", true, true);
        }

        for (int i = 0; i < redSlashes.Length; i++)
        {
            timers[i] += Time.deltaTime;
            redSlashes[i].color = SlashDisapear(redSlashes[i].color, timers[i]);
        }
    }

    private Color SlashDisapear(Color color, float timer)
    {
        if (color.a > 0.0f)
        {
            color.a = Mathf.Lerp(1.0f, 0.0f, timer / 5.0f);
        }
        return color;
    }

    private void OnTriggerStay(Collider other)
    {
        if (evading)
        {
            if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Resource") || other.gameObject.CompareTag("Building") || other.gameObject.CompareTag("Boar") || other.gameObject.CompareTag("JustForEvade"))
            {
                if (!other.gameObject.CompareTag("Enemy"))
                {
                    blocked = true;
                }
                else
                {
                    if (Facing(other.gameObject.transform.position, 30.0f))
                    {
                        blocked = true;
                    }
                }
            }
        }       
    }
}