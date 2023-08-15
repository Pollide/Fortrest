using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{
    public static PlayerController global;

    // Class Access   
    public Bow bowScript;
    public CharacterController playerCC;
    public Animator CharacterAnimator;

    // Movement
    private Vector3 moveDirection;
    private float horizontalMovement;
    private float verticalMovement;
    public GameObject house;
    [HideInInspector] public GameObject houseSpawnPoint;
    public GameObject bodyShape;
    private GameObject interactText;
    [HideInInspector] public bool LanternLighted;
    // Speed
    private float playerCurrentSpeed = 0f;
    private float playerWalkSpeed = 5.0f;
    private float playerSprintSpeed = 8.0f;
    private float playerBowSpeed = 3.0f;
    private bool running = false;
    private float runTimer = 0.0f;
    private bool canRun = true;

    // Gathering
    private bool gathering = false;
    private float gatherTimer = 0.0f;
    private float resetGather = 1.25f;

    // Evade
    private float evadeTimer = 0.0f;
    private float evadeCoolDown = 2.5f;
    [HideInInspector] public bool evading = false;
    [HideInInspector] public bool canEvade = true;
    [HideInInspector] public bool playerCanBeDamaged = true;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool blocked = false;

    // Shooting
    private bool canShoot;
    [HideInInspector] public float bowDamage = 1.5f;
    private float bowTimer = 0.0f;
    private float resetBow = 1.5f;
    private bool shooting = false;
    private bool directionSaved = false;
    private Quaternion tempDirection;

    // Spawn Turret
    private bool turretSpawned;
    private float turretTimer = 0.0f;
    private float resetTurret = 30.0f;
    private float turretDuration = 20.0f;
    GameObject miniTurret;

    // Gravity
    private float playerGravMultiplier = 2.0f;
    private float playerGrav = -9.81f;
    private float playerVelocity;

    // Energy
    [HideInInspector] public float playerEnergy = 0f;
    private float maxPlayerEnergy = 100f;
    public Image playerEnergyBarImage;
    private float energySpeed = 12.5f;

    // Health
    private bool deathEffects = false;
    [HideInInspector] public bool playerDead = false;
    private bool playerRespawned;
    [HideInInspector] public bool houseDisplay;
    [HideInInspector] public float playerHealth = 0.0f;
    [HideInInspector] public float maxHealth = 100.0f;
    public HealthBar healthBar;

    // Eating
    [HideInInspector] public int appleAmount = 0;
    private float appleHealAmount = 10.0f;
    [HideInInspector] public int maxApple = 5;

    // Attacks
    [HideInInspector] public float attackDamage = 1.0f;
    private float attackTimer = 0.0f;
    private float resetAttack = 0.95f;
    private float comboTimer = 0.0f;
    private float resetCombo = 1.20f;
    private int attackCount = 0;
    [HideInInspector] public Building currentResource;
    [HideInInspector] public bool damageEnemy = false;
    [HideInInspector] public bool lunge = false;
    public bool upgradedMelee;

    // States
    [Header("Player States")]
    [HideInInspector] public bool playerCanMove = true;
    private bool playerisMoving = false;
    [HideInInspector] public bool attacking = false;

    // Teleporter
    [HideInInspector] public bool canTeleport = false;

    // VFXs
    private VisualEffect VFXSlash;
    private VisualEffect VFXSleeping;
    private VisualEffect VFXSparks;
    private VisualEffect VFXPebble;
    private VisualEffect VFXWoodChip;

    // Tools
    public GameObject AxeGameObject;
    public GameObject HammerGameObject;
    public GameObject PickaxeGameObject;
    public GameObject SwordGameObject;
    public GameObject BowGameObject;
    private GameObject RadiusCamGameObject;
    public GameObject PauseCanvasGameObject;
    public Transform MapSpotHolder;
    public RectTransform MapPlayerRectTransform;
    public Transform MapResourceHolder;
    public GameObject MapResourcePrefab;
    public bool lastWasAxe;

    private bool mapBool;

    [System.Serializable]
    public class ToolData
    {
        public bool AxeBool;
        public bool HammerBool;
        public bool PickaxeBool;
        public bool SwordBool;
        public bool HandBool;
        public bool BowBool;
    }

    // Texts
    public TMP_Text DayTMP_Text;
    public TMP_Text RemaningTMP_Text;
    public TMP_Text SurvivedTMP_Text;
    public TMP_Text enemyText;
    public TMP_Text enemyAmountText;
    public TMP_Text houseUnderAttackText;
    public TMP_Text enemyDirectionText;
    public TMP_Text appleText;
    public TMP_Text turretText;

    // Inventory
    public GameObject DarkenGameObject;
    public GameObject InventoryHolder;
    // Pause
    [HideInInspector] public bool pausedBool;
    public Animation UIAnimation;

    // Death
    private float respawnTimer = 0.0f;
    private bool textAnimated = false;

    // Enemy UI
    private int lastAmount = 0;

    // Damage Indicators    
    public Image[] redSlashes;
    public Image redBorders;
    private bool displaySlash;
    private bool animationPlayed = false;
    private float timer1, timer2, timer3, timer4 = 0.0f;
    private float[] timers = new float[4];

    // Controller
    private bool sprintingCTRL;
    [HideInInspector] public bool movingCTRL;
    [HideInInspector] public bool selectCTRL;
    private bool gatheringCTRL;
    private bool attackingCTRL;
    private bool aimingCTRL;
    private bool evadeCTRL;
    private bool cancelCTRL;
    private bool turretCTRL;
    private bool healCTRL;
    [HideInInspector] public bool interactCTRL;
    [HideInInspector] public bool needInteraction = false;
    [HideInInspector] public bool lockingCTRL = false;
    [HideInInspector] public bool inventoryCTRL = false;
    [HideInInspector] public bool swapCTRL = false;
    [HideInInspector] public Vector2 moveCTRL;
    [HideInInspector] public bool upCTRL;
    [HideInInspector] public bool downCTRL;
    [HideInInspector] public bool canPressCTRL;
    [HideInInspector] public bool pauseSelectCTRL;
    [HideInInspector] public bool releasedCTRL;

    // Keyboard Controls
    private KeyCode[] keyCodes;

    // Swapping
    [HideInInspector] public bool cancelAnimation;
    [HideInInspector] public bool cancelEffects;

    public SkinnedMeshRenderer LanternSkinnedRenderer;

    public bool canGetInHouse;

    public Image countdownBar;
    private bool gapSet;
    private float gap;
    private float fraction;
    public const int MaxApples = 5;
    private float newGap;
    private bool displayAmount;
    public bool teleporting;
    private bool cancelHit;
    [HideInInspector] public bool staggered;

    // Start is called before the first frame update
    void Awake()
    {
        if (GameManager.global)
        {
            // Get Character controller that is attached to the player
            playerCC = GetComponent<CharacterController>();

            global = this;

            houseSpawnPoint = house.transform.GetChild(1).gameObject;
            // Controller stuff

            // Left stick to move
            GameManager.global.gamepadControls.Controls.Move.performed += context => moveCTRL = context.ReadValue<Vector2>();
            GameManager.global.gamepadControls.Controls.Move.canceled += context => moveCTRL = Vector2.zero;
            GameManager.global.gamepadControls.Controls.Move.performed += context => MoveController(true);
            GameManager.global.gamepadControls.Controls.Move.canceled += context => MoveController(false);
            GameManager.global.gamepadControls.Controls.Move.performed += context => PauseSelection();
            GameManager.global.gamepadControls.Controls.Move.canceled += context => canPressCTRL = false;

            // A to sprint
            GameManager.global.gamepadControls.Controls.Sprint.performed += context => SprintController(true);
            GameManager.global.gamepadControls.Controls.Sprint.canceled += context => SprintController(false);

            // A to select in build mode
            GameManager.global.gamepadControls.Controls.Sprint.performed += context => BuildSelectController();

            // A to select in pause mode
            GameManager.global.gamepadControls.Controls.Sprint.performed += context => PauseEnter();

            // X to interact
            GameManager.global.gamepadControls.Controls.Interact.performed += context => InteractController();

            // Y to swap tool
            GameManager.global.gamepadControls.Controls.Swap.performed += context => SwappingController();

            // B to evade
            GameManager.global.gamepadControls.Controls.Evade.performed += context => EvadeController();

            // Right trigger for gathering
            GameManager.global.gamepadControls.Controls.Gathering.performed += context => GatheringController(true);
            GameManager.global.gamepadControls.Controls.Gathering.canceled += context => GatheringController(false);

            // Right trigger for attacking
            GameManager.global.gamepadControls.Controls.Attacking.performed += context => AttackingController();

            // Left trigger for aiming
            GameManager.global.gamepadControls.Controls.Aiming.performed += context => AimingController(true);
            GameManager.global.gamepadControls.Controls.Aiming.canceled += context => AimingController(false);

            // Right Bumper for Mini Turret
            GameManager.global.gamepadControls.Controls.Turret.performed += context => TurretController();

            // Left Bumper to heal
            GameManager.global.gamepadControls.Controls.Heal.performed += context => HealController();

            // Pause button to pause
            GameManager.global.gamepadControls.Controls.Pause.performed += context => PauseController();

            // Select button to open Map
            GameManager.global.gamepadControls.Controls.Map.performed += context => MapController();

            // Select to lock / unlock camera
            //gamepadControls.Controls.CameraLock.performed += context => lockingCTRL = true;
            // X to open / close inventory
            //gamepadControls.Controls.Inventory.performed += context => inventoryCTRL = true;

        }
    }

    // CONTROLLER FUNCTIONS START
    private void SprintController(bool pressed)
    {
        if (!PlayerModeHandler.global.inTheFortress)
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
    }

    public void OpenResourceHolder(bool open)
    {
        GameManager.PlayAnimator(UIAnimation.GetComponent<Animator>(), "Resource Holder Appear", open, false, 1);
    }

    private void BuildSelectController()
    {
        if (PlayerModeHandler.global.inTheFortress)
        {
            if (!selectCTRL)
            {
                selectCTRL = true;
            }
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

    private void PauseSelection()
    {
        if (pausedBool)
        {
            if (moveCTRL.y > 0f && !canPressCTRL)
            {
                upCTRL = true;
                canPressCTRL = true;
            }
            else if (moveCTRL.y < 0f && !canPressCTRL)
            {
                downCTRL = true;
                canPressCTRL = true;
            }
        }
    }

    private void PauseEnter()
    {
        if (pausedBool)
        {
            if (!pauseSelectCTRL)
            {
                pauseSelectCTRL = true;
            }
        }
    }

    private void PauseController()
    {
        PauseVoid(!pausedBool);
    }

    private void MapController()
    {
        MapVoid(!mapBool);
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

    private void AimingController(bool pressed)
    {
        if (PlayerModeHandler.global.playerModes == PlayerModes.CombatMode)
        {
            if (pressed)
            {
                aimingCTRL = true;
                cancelCTRL = true;
            }
            else
            {
                aimingCTRL = false;
                cancelCTRL = false;
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

    private void EvadeController()
    {
        if (!evadeCTRL && canEvade)
        {
            evadeCTRL = true;
        }
    }

    private void SwappingController()
    {
        if (!swapCTRL)
        {
            swapCTRL = true;
            cancelCTRL = true;
        }
    }

    private void TurretController()
    {
        if (!turretCTRL && !turretSpawned)
        {
            turretCTRL = true;
        }
    }

    private void HealController()
    {
        if (!healCTRL && appleAmount > 0)
        {
            healCTRL = true;
        }
    }

    // CONTROLLER FUNCTIONS END

    private void Start()
    {
        LevelManager manager = LevelManager.global;

        // VFXs
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

        // Game Objects
        if (GameObject.Find("Radius Camera"))
        {
            RadiusCamGameObject = GameObject.Find("Radius Camera");
        }

        interactText = house.transform.GetChild(3).gameObject;

        // Setting default values
        playerCurrentSpeed = playerWalkSpeed;
        playerEnergy = maxPlayerEnergy;
        playerEnergyBarImage.fillAmount = 0.935f;
        playerHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth, false);

        // Adding timers to array
        timers[0] = timer1;
        timers[1] = timer2;
        timers[2] = timer3;
        timers[3] = timer4;

        // Setting UI Text
        UpdateAppleText();

        keyCodes = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));

        ChangeTool(new ToolData() { AxeBool = true });
    }

    void Update()
    {
        if (pausedBool && !Input.GetKeyDown(KeyCode.Escape))
        {
            return;
        }

        if (evading)
        {           
            playerCanBeDamaged = false;
            evadeTimer += Time.deltaTime;
            if (!blocked)
            {
                transform.position = Vector3.Lerp(startPosition, endPosition, evadeTimer);               
            }
            return;
        }
        else
        {
            playerCanBeDamaged = true;
            blocked = false;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            playerHealth = 0;
            healthBar.SetHealth(playerHealth, false);
        }

        if (playerCanMove)
        {
            // Controller
            if (moveCTRL.x != 0 || moveCTRL.y != 0)
            {
                horizontalMovement = moveCTRL.x;
                verticalMovement = moveCTRL.y;
            }
            // Keyboard
            else if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                horizontalMovement = Input.GetAxis("Horizontal");
                verticalMovement = Input.GetAxis("Vertical");
            }

            // Physics
            HandleSpeed();
            ApplyGravity();
            ApplyMovement(horizontalMovement, verticalMovement);

            // Mechanics
            Attack();
            Gathering();
            Shoot();
            ModeChanged();
            if (lunge && PlayerModeHandler.global.playerModes == PlayerModes.CombatMode)
            {
                AttackLunge();
            }
        }
        else
        {
            horizontalMovement = 0;
            verticalMovement = 0;
            running = false;
        }

        HandleEnergy();

        CharacterAnimator.SetBool("Upgraded", upgradedMelee);
        TimersFunction();
        ScreenDamage();
        CheckCurrentTool();
        Resting();
        BarDisappear();

        if (playerDead)
        {
            Death();
        }       

        if (teleporting)
        {
            teleporting = false;
        }

        if (Input.GetKeyDown(KeyCode.U)) // TEMPORARY
        {
            upgradedMelee = !upgradedMelee;
        }

        foreach (KeyCode keyCode in keyCodes)
        {
            KeyRead(keyCode);
        }

        if (playerHealth <= 0)
        {
            playerCanMove = false;
            int random = Random.Range(1, 3);
            if (random == 1)
            {
                CharacterAnimator.SetTrigger("Death1");
            }
            else if (random == 2)
            {
                CharacterAnimator.SetTrigger("Death2");
            }
        }
    }

    private void CheckCurrentTool()
    {
        if (PlayerModeHandler.global.playerModes == PlayerModes.ResourceMode)
        {
            if (AxeGameObject.activeSelf)
            {
                lastWasAxe = true;
            }
            else if (PickaxeGameObject.activeSelf)
            {
                lastWasAxe = false;
            }
        }
    }

    private void ModeChanged()
    {
        if (Input.GetKey(KeyCode.Q) || Input.GetMouseButton(1) || cancelCTRL || cancelHit)
        {
            cancelHit = false;
            cancelAnimation = true;
            cancelEffects = true;
            if (!aimingCTRL)
            {
                cancelCTRL = false;
            }
            StopCoroutine("RevertCancel");
            StartCoroutine("RevertCancel");
        }
        //if (Input.GetMouseButton(0) || attackingCTRL || gatheringCTRL)
        //{
        //    cancelEffects = false;
        //}

        if (cancelAnimation)
        {
            CharacterAnimator.SetBool("Swapping", cancelAnimation);
            cancelAnimation = false;
        }
        else
        {
            CharacterAnimator.SetBool("Swapping", cancelAnimation);
        }
    }

    private IEnumerator RevertCancel()
    {
        yield return new WaitForSeconds(0.1f);
        cancelEffects = false;
        //cancelAnimation = false;
    }

    private void Resting()
    {
        if (PlayerModeHandler.global.playerModes == PlayerModes.BuildMode || PlayerModeHandler.global.playerModes == PlayerModes.RepairMode)
        {
            playerHealth += Time.deltaTime * 2.0f;
            healthBar.SetHealth(playerHealth, false);
        }
    }

    private void TimersFunction()
    {
        if (attacking)
        {
            if (!TickTimers(resetAttack, ref attackTimer))
            {
                attacking = false;
                damageEnemy = false;
            }
        }

        if (attackCount >= 0)
        {
            if (!TickTimers(resetCombo, ref comboTimer))
            {
                comboTimer = 0;
                attackCount = 0;
            }
        }

        if (gathering)
        {
            gathering = TickTimers(resetGather, ref gatherTimer);
        }

        if (shooting)
        {
            shooting = TickTimers(resetBow, ref bowTimer);
        }

        if (turretSpawned)
        {
            if (!turretText.gameObject.activeSelf)
            {
                turretText.gameObject.SetActive(true);
            }

            turretSpawned = TickTimers(resetTurret, ref turretTimer);

            int seconds = 30 - (int)turretTimer % 60;

            if (seconds != 0)
            {
                turretText.text = seconds.ToString();
            }

            if (turretTimer >= turretDuration)
            {
                turretSpawned = false;

                if (miniTurret)
                    Destroy(miniTurret, GameManager.PlayAnimation(miniTurret.GetComponent<Animation>(), "MiniTurretSpawn", false).length);
            }
        }
        else
        {
            if (turretText.gameObject.activeSelf)
            {
                turretText.gameObject.SetActive(false);
            }
        }
    }

    private bool TickTimers(float limit, ref float timer)
    {
        timer += Time.deltaTime;

        if (timer >= limit)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void KeyRead(KeyCode letter)
    {
        if (Input.GetKeyDown(letter))
        {
            switch (letter)
            {
                case KeyCode.R:
                    if (appleAmount > 0 && playerCanMove)
                    {
                        EatApple();
                    }
                    break;

                case KeyCode.T:
                    if (!turretSpawned && playerCanMove)
                    {
                        SpawnTurret();
                    }
                    break;

                case KeyCode.E:
                    if (canTeleport)
                    {
                        TeleportPlayer(houseSpawnPoint.transform.position);
                    }
                    break;

                case KeyCode.Space:
                    if (canEvade && playerCanMove)
                    {
                        StartCoroutine(Evade());
                    }
                    break;

                case KeyCode.Escape:
                    PauseVoid(!pausedBool);
                    break;

                case KeyCode.Tab:
                    MapVoid(!mapBool);
                    break;

                default:
                    break;
            }
        }
        else
        {
            if (playerCanMove)
            {
                if (evadeCTRL)
                {
                    evadeCTRL = false;
                    StartCoroutine(Evade());
                }
                if (turretCTRL)
                {
                    turretCTRL = false;
                    SpawnTurret();
                }
                if (healCTRL)
                {
                    healCTRL = false;
                    EatApple();
                }
            }
            if (canTeleport && interactCTRL)
            {
                TeleportPlayer(houseSpawnPoint.transform.position);
            }
        }
    }

    public void TeleportPlayer(Vector3 pos)
    {
        if (pos == Vector3.zero)
        {
            Debug.LogError("POSITION WAS ZERO. IF YOU SEE THIS MESSAGE SEE WHERE IT WAS CALLED FROM TY");
            return;
        }
        if (Boar.global.mounted)
        {
            //Boar.global.canMove = false;
            Boar.global.cc.enabled = false;
            Boar.global.transform.position = pos;
            Boar.global.cc.enabled = true;
            Boar.global.animator.SetBool("Moving", false);
        }
        else
        {
            //PlayerController.global.playerCanMove = false;
            playerCC.enabled = false;
            transform.position = pos;
            playerCC.enabled = true;
            CharacterAnimator.SetBool("Moving", false);
        }
        interactCTRL = false;
        canTeleport = false;
        needInteraction = false;
        teleporting = true;

        LevelManager.global.SceneCamera.transform.position = pos;
    }

    public void ChangeTool(ToolData toolData)
    {
        AxeGameObject.SetActive(toolData.AxeBool);
        HammerGameObject.SetActive(toolData.HammerBool);
        PickaxeGameObject.SetActive(toolData.PickaxeBool);
        SwordGameObject.SetActive(toolData.SwordBool);
        BowGameObject.SetActive(toolData.BowBool);
        if (RadiusCamGameObject != null)
        {
            RadiusCamGameObject.SetActive(toolData.HammerBool);
        }
    }

    private void HandleSpeed()
    {
        if ((Input.GetKey(KeyCode.LeftShift) || sprintingCTRL) && canRun && !staggered && CharacterAnimator.GetBool("Moving") == true && !canShoot)
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

    private IEnumerator Evade()
    {
        evadeTimer = 0;
        canEvade = false;
        evading = true;
        blocked = false;
        CharacterAnimator.ResetTrigger("Evade");
        CharacterAnimator.SetTrigger("Evade");
        startPosition = transform.position;
        endPosition = transform.position + (transform.forward * 8.5f);
        staggered = false;

        yield return new WaitForSeconds(evadeCoolDown);
        canEvade = true;
    }

    public void PauseVoid(bool pause)
    {
        if (!mapBool)
        {
            PauseCanvasGameObject.SetActive(pause);
            if (pause)
            {
                for (int i = 0; i < Pause.global.ButtonHolder.childCount; i++)
                {
                    Pause.global.ButtonHolder.GetChild(i).gameObject.SetActive(i == 0);
                }

                Pause.global.SelectedList = new List<int>();

                for (int i = 0; i < Pause.global.ButtonHolder.childCount; i++)
                {
                    Pause.global.SelectedList.Add(0);
                }
            }

            GameManager.PlayAnimator(UIAnimation.GetComponent<Animator>(), "Pause Appear", pause);
            GameManager.global.MusicManager.PlayMusic(pause ? GameManager.global.PauseMusic : LevelManager.global.ReturnNight() ? GameManager.global.NightMusic : LevelManager.global.ActiveBiomeMusic);

            Time.timeScale = pause ? 0 : 1;
            playerCanMove = !pause;
            pausedBool = pause;
            PlayerModeHandler.SetMouseActive(pause);
        }
    }

    public void MapVoid(bool map)
    {
        if (!pausedBool && PlayerModeHandler.global.playerModes != PlayerModes.BuildMode)
        {
            GameManager.PlayAnimator(UIAnimation.GetComponent<Animator>(), "Map Appear", map);
            GameManager.global.MusicManager.PlayMusic(map ? GameManager.global.PauseMusic : LevelManager.global.ReturnNight() ? GameManager.global.NightMusic : LevelManager.global.ActiveBiomeMusic);
            Time.timeScale = map ? 0 : 1;
            mapBool = map;
            OpenResourceHolder(map);
            if (mapBool)
            {
                MapPlayerRectTransform.anchoredPosition = ConvertToMapCoordinates(transform.position);
                MapPlayerRectTransform.eulerAngles = new Vector3(0, 0, -transform.eulerAngles.y + 45);
                MapPlayerRectTransform.SetAsLastSibling(); //keeps it ontop

                UpdateResourceHolder();
            }
        }
    }

    public void UpdateResourceHolder(int bridgeTypeInt = 0)
    {
        for (int i = 0; i < MapResourceHolder.childCount; i++)
        {
            Destroy(MapResourceHolder.GetChild(i).gameObject);
        }

        List<LevelManager.TierData> woodCostList = new List<LevelManager.TierData>();
        List<LevelManager.TierData> stoneCostList = new List<LevelManager.TierData>();

        for (int i = 0; i < LevelManager.global.WoodTierList.Count; i++)
        {
            woodCostList.Add(new LevelManager.TierData());
        }

        for (int i = 0; i < LevelManager.global.StoneTierList.Count; i++)
        {
            stoneCostList.Add(new LevelManager.TierData());
        }

        if (bridgeTypeInt == 1)
        {
            woodCostList[0].ResourceCost = -30;
            stoneCostList[0].ResourceCost = -30;
        }

        if (bridgeTypeInt == 2)
        {
            woodCostList[1].ResourceCost = -30;
            stoneCostList[1].ResourceCost = -30;
        }

        if (PlayerModeHandler.global.playerModes == PlayerModes.BuildMode)
        {
            if (PlayerModeHandler.global.buildType == BuildType.Turret)
            {
                woodCostList[0].ResourceCost = -10;
                stoneCostList[0].ResourceCost = -5;
            }

            if (PlayerModeHandler.global.buildType == BuildType.Cannon)
            {
                woodCostList[0].ResourceCost = -3;
                stoneCostList[0].ResourceCost = -3;
                woodCostList[1].ResourceCost = -5;
                stoneCostList[1].ResourceCost = -5;
            }

            if (PlayerModeHandler.global.buildType == BuildType.Slow)
            {
                stoneCostList[0].ResourceCost = -5;
                stoneCostList[1].ResourceCost = -10;
                stoneCostList[2].ResourceCost = -2;
            }

            if (PlayerModeHandler.global.buildType == BuildType.Scatter)
            {
                stoneCostList[1].ResourceCost = -5;
                woodCostList[2].ResourceCost = -10;
                stoneCostList[2].ResourceCost = -2;
            }
        }

        //      Debug.Log("UPDATE");

        ResourceGenerate(LevelManager.global.WoodTierList, woodCostList);
        ResourceGenerate(LevelManager.global.StoneTierList, stoneCostList);
    }

    void ResourceGenerate(List<LevelManager.TierData> tierList, List<LevelManager.TierData> costList)
    {
        bool cost = false;

        for (int i = 0; i < tierList.Count; i++)
        {
            if (costList[i].ResourceCost != 0)
            {
                cost = true;
                break;
            }
        }

        for (int i = 0; i < tierList.Count; i++)
        {
            if (cost)
            {
                tierList[i].ResourceCost = costList[i].ResourceCost;

                if (costList[i].ResourceCost == 0)
                {
                    continue;
                }
            }
            else
            {
                tierList[i].ResourceCost = 0;
            }

            // Debug.Log(tierList[i].ResourceAmount + "> 0 || " + tierList[i].ResourceCost + "> 0");
            if (tierList[i].ResourceAmount != 0 || tierList[i].ResourceCost != 0)
            {
                GameObject mapResource = Instantiate(MapResourcePrefab, MapResourceHolder);
                Text costText = mapResource.transform.GetChild(1).GetComponent<Text>();

                costText.text = tierList[i].ResourceAmount.ToString("N0");
                mapResource.transform.GetChild(0).GetComponent<Image>().sprite = tierList[i].ResourceIcon;

                //GameManager.TemporaryAnimation(mapResource, GameManager.global.PopupAnimation, i);

                if (tierList[i].ResourceCost != 0)
                {
                    costText.text += " " + tierList[i].ResourceCost.ToString("N0");

                    costText.color = tierList[i].SufficientResource() ? Color.green : Color.red;
                }
            }

        }
    }

    public bool CheckSufficientResources(bool purchase = false)
    {
        if (!GameManager.global.CheatInfiniteBuilding)
        {
            if (!Sufficient(LevelManager.global.WoodTierList, purchase))
            {
                return false;
            }

            if (!Sufficient(LevelManager.global.StoneTierList, purchase))
            {
                return false;
            }
        }

        if (purchase)
        {
            UpdateResourceHolder();
        }

        return true;
    }

    bool Sufficient(List<LevelManager.TierData> tierList, bool purchase)
    {
        for (int i = 0; i < tierList.Count; i++)
        {
            if (!tierList[i].SufficientResource())
            {
                return false;
            }
            else if (purchase)
            {
                tierList[i].ResourceAmount += tierList[i].ResourceCost;
            }
        }

        return true;
    }

    public Vector2 ConvertToMapCoordinates(Vector3 position)
    {
        // Step 2: Convert to 2D screen space using the map camera
        position.y = position.z;
        position -= new Vector3(100, 170);
        position.x *= 1.5f;
        position.y *= 1.5f;

        // Step 3: Normalize the screen position based on your map's size or aspect ratio
        // Vector2 normalizedMapPosition = new Vector2(screenPosition.x / Screen.width, screenPosition.y / Screen.height);

        return position;
    }

    private void ApplyMovement(float _horizontalMove, float _verticalMove)
    {
        playerisMoving = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) || (movingCTRL);

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
        healthBar.SetHealth(playerHealth, false);
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
                if (upgradedMelee)
                {
                    resetAttack = 0.75f;
                    resetCombo = 1.0f;
                    attackDamage = 1.25f;
                }
                else
                {
                    resetAttack = 0.95f;
                    resetCombo = 1.2f;
                    attackDamage = 1.0f;
                }
                CharacterAnimator.SetTrigger("Swing");
            }
            else if (attackCount == 1)
            {
                if (upgradedMelee)
                {
                    resetAttack = 0.7f;
                    resetCombo = 0.95f;
                    attackDamage = 1.25f;
                }
                else
                {
                    resetAttack = 0.9f;
                    resetCombo = 1.15f;
                    attackDamage = 1.0f;
                }
                CharacterAnimator.SetTrigger("Swing2");
            }
            else if (attackCount == 2)
            {
                if (upgradedMelee)
                {
                    resetAttack = 0.75f;
                    resetCombo = 1.0f;
                    attackDamage = 2.0f;
                }
                else
                {
                    resetAttack = 0.95f;
                    resetCombo = 1.2f;
                    attackDamage = 1.5f;
                }
                CharacterAnimator.SetTrigger("Swing3");
            }
        }
    }

    private void Gathering()
    {
        LevelManager.ProcessBuildingList((building) =>
        {
            float minDistanceFloat = 4.0f;
            float distanceFloat = Vector3.Distance(transform.position, building.position);
            float smallestDistance = 5.0f;
            if (distanceFloat < minDistanceFloat)
            {
                if (distanceFloat <= smallestDistance)
                {
                    smallestDistance = distanceFloat;
                }
            }

            if (Facing(building.position, 75.0f) && !gathering && building.GetComponent<Building>().health > 0 && distanceFloat < minDistanceFloat && distanceFloat == smallestDistance && (Input.GetMouseButton(0) || gatheringCTRL) && PlayerModeHandler.global.playerModes == PlayerModes.ResourceMode)
            {
                gathering = true;
                gatherTimer = 0;
                currentResource = building.GetComponent<Building>();
                ChangeTool(new ToolData() { AxeBool = currentResource.resourceObject == Building.BuildingType.Wood, PickaxeBool = currentResource.resourceObject == Building.BuildingType.Stone, HandBool = currentResource.resourceObject == Building.BuildingType.Bush });
                CharacterAnimator.ResetTrigger("Swing");
                CharacterAnimator.SetTrigger("Swing");
            }

        }, true); //true means natural
    }

    private void Shoot()
    {
        if (PlayerModeHandler.global.playerModes == PlayerModes.CombatMode)
        {
            if (Input.GetMouseButton(1) || aimingCTRL)
            {
                ChangeTool(new ToolData() { BowBool = true });
                canShoot = true;
                if (Input.GetKey(KeyCode.LeftShift) || sprintingCTRL)
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
                ChangeTool(new ToolData() { SwordBool = true });
                canShoot = false;
            }

            if ((Input.GetMouseButtonDown(0) || attackingCTRL) && canShoot && !shooting)
            {
                attackingCTRL = false;
                shooting = true;
                bowTimer = 0;
                bowScript.Shoot();
            }
        }
    }

    public IEnumerator ToolAppear()
    {
        yield return new WaitForSeconds(1.5f);
        ChangeTool(new ToolData() { AxeBool = lastWasAxe, PickaxeBool = !lastWasAxe });
    }

    private void SpawnTurret()
    {
        turretTimer = 0;
        turretSpawned = true;

        miniTurret = Instantiate(PlayerModeHandler.global.turretPrefabs[0], transform.position + (transform.forward * 2) - (Vector3.up * (transform.position.y - 0.48f)), transform.rotation);
        miniTurret.transform.localScale = new Vector3(0.3f, 1, 0.3f);
        miniTurret.GetComponent<TurretShooting>().MiniTurret = true;
        miniTurret.GetComponent<TurretShooting>().CurrentLevel = miniTurret.GetComponent<TurretShooting>().ModelHolder.childCount - 1;
        miniTurret.GetComponent<TurretShooting>().turn_speed = 10;
        miniTurret.GetComponent<TurretShooting>().damage = 1;
        miniTurret.GetComponent<TurretShooting>().fireRate = 3f;
        miniTurret.GetComponent<TurretShooting>().shootingRange = 10;
        GameManager.PlayAnimation(miniTurret.GetComponent<Animation>(), "MiniTurretSpawn");
    }

    private void EatApple()
    {
        if (playerHealth < maxHealth)
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.EatingSound);
            HealthRestore(appleHealAmount);
            appleAmount -= 1;
            GameManager.PlayAnimation(appleText.GetComponent<Animation>(), "EnemyAmount");
            UpdateAppleText();
        }
    }

    public void UpdateAppleText()
    {
        appleText.text = appleAmount.ToString();

        appleText.color = appleAmount >= maxApple ? Color.green : Color.white;
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

    private void AttackLunge()
    {
        float tempTimer = 0;
        tempTimer += Time.deltaTime;

        gameObject.GetComponent<CharacterController>().enabled = false;
        if (attackCount == 0)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward * 1.25f, tempTimer * 5.0f);
        }
        else if (attackCount == 1 || attackCount == 2)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward * 0.75f, tempTimer * 5.0f);
        }
        VFXSlash.transform.position = transform.position;
        gameObject.GetComponent<CharacterController>().enabled = true;
    }

    public void GatheringEffects()
    {
        if (currentResource)
        {
            if (currentResource.resourceObject == Building.BuildingType.Bush)
            {
                StopCoroutine("ToolAppear");
                StartCoroutine("ToolAppear");
            }
            if (PickaxeGameObject.activeSelf)
            {
                GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.Pickaxe2Sound : GameManager.global.Pickaxe3Sound);

                VFXSparks.transform.position = currentResource.transform.position;
                VFXSparks.Play();
                VFXPebble.transform.position = currentResource.transform.position;
                VFXPebble.Play();
            }
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

    public void DisplayEnemiesComingText()
    {
        GameManager.PlayAnimation(enemyDirectionText.GetComponent<Animation>(), "EnemyDirection");
        displayAmount = true;
    }

    public void EnemiesTextControl()
    {
        // Enemy remaining and enemy amount appearing
        if (displayAmount)
        {
            StopCoroutine("TextDisappearing");
            StartCoroutine(TextAppearing());
            displayAmount = false;
        }

        // Calculate enemy amount and display it
        int goblinsInt = 0;
        LevelManager.ProcessEnemyList((enemy) =>
        {
            if (enemy.currentEnemyType != EnemyController.ENEMYTYPE.wolf)
            {
                goblinsInt++;
            }
        });
        int remaining = LevelManager.global.enemiesCount + goblinsInt;
        if (lastAmount != remaining)
        {
            GameManager.PlayAnimation(enemyAmountText.GetComponent<Animation>(), "EnemyAmount");
            lastAmount = remaining;
        }
        enemyAmountText.text = remaining.ToString();

        // Enemy remaining and enemy amount disappearing
        if (LevelManager.global.waveEnd && remaining <= 0)
        {
            StartCoroutine(TextDisappearing());           
        }
    }

    public void Death()
    {
        if (!deathEffects)
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.SnoringSound, 0.2f, true, 0, true);
            VFXSleeping.Play();
            TeleportPlayer(house.transform.position);
            playerCC.enabled = false;
            bodyShape.SetActive(false);
            playerRespawned = false;
            deathEffects = true;
        }
        if (!playerRespawned)
        {
            respawnTimer += Time.deltaTime;
            playerHealth = Mathf.Lerp(0.0f, maxHealth, respawnTimer / 15.0f);
            healthBar.SetHealth(playerHealth, false);
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
                    TeleportPlayer(houseSpawnPoint.transform.position);
                    playerCanMove = true;
                    bodyShape.SetActive(true);
                    playerDead = false;
                    deathEffects = false;
                    playerRespawned = true;
                    respawnTimer = 0.0f;
                    LevelManager.FloatingTextChange(interactText, false);
                    textAnimated = false;
                    needInteraction = false;
                    interactCTRL = false;
                    houseDisplay = true; // Used to reanimate house and make text appear once player respawns
                }
            }
        }
    }

    public void TakeDamage(float damage, bool stagger)
    {
        cancelHit = true;
        staggered = true;
        if (stagger)
        {
            CharacterAnimator.ResetTrigger("Swing");
            CharacterAnimator.ResetTrigger("Swing2");
            CharacterAnimator.ResetTrigger("Swing3");
            int random = Random.Range(1, 4);
            if (random == 1)
            {
                CharacterAnimator.SetTrigger("Hit1");
            }
            else if (random == 2)
            {
                CharacterAnimator.SetTrigger("Hit2");
            }
            else
            {
                CharacterAnimator.SetTrigger("Hit3");
            }
        }       
        playerHealth -= damage;
        healthBar.SetHealth(playerHealth, false);
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

    private void BarDisappear()
    {
        if (enemyDirectionText.rectTransform.anchoredPosition.y == 450.0f)
        {
            if (!gapSet)
            {
                gap = LevelManager.global.randomAttackTrigger - LevelManager.global.DaylightTimer;
                fraction = 663.0f / gap;
                gapSet = true;
            }
            countdownBar.gameObject.SetActive(true);
            countdownBar.rectTransform.sizeDelta = new Vector2(fraction * (LevelManager.global.randomAttackTrigger - LevelManager.global.DaylightTimer), 10.0f);
        }
        if (countdownBar.rectTransform.sizeDelta.x <= 0f)
        {
            enemyDirectionText.rectTransform.anchoredPosition = new Vector2(0f, 0f);
            enemyDirectionText.rectTransform.localScale = new Vector3(0f, 0f, 0f);
            gapSet = false;
            gap = 0f;
            countdownBar.gameObject.SetActive(false);
            LevelManager.global.messageDisplayed = false;
        }
    }

    private IEnumerator TextAppearing()
    {
        bool gapSet = false;
        float fraction = 0f;
        float gradient = 0f;
        if (!gapSet)
        {
            newGap = LevelManager.global.randomAttackTrigger - LevelManager.global.DaylightTimer;
            fraction = 1.0f / newGap;
            gapSet = true;
        }
        while (gradient < 1.0f)
        {
            gradient = fraction * ((LevelManager.global.DaylightTimer + newGap) - LevelManager.global.randomAttackTrigger);
            enemyText.color = LevelManager.global.textGradient.Evaluate(gradient);
            enemyAmountText.color = LevelManager.global.textGradient.Evaluate(gradient);
            yield return null;
        }
    }

    private IEnumerator TextDisappearing()
    {
        float temp = 1.0f;
        while (temp > 0)
        {
            temp -= Time.deltaTime / 2.0f;
            enemyText.color = LevelManager.global.textGradient.Evaluate(temp);
            enemyAmountText.color = LevelManager.global.textGradient.Evaluate(temp);
            yield return null;
        }
    }
}