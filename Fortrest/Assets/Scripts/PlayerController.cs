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
    public Bow bowScript; // Used to call the shoot function of the bow
    public CharacterController playerCC; // Used to move the player
    public Animator characterAnimator; // Used to manage player animations

    // House & Player Model
    public GameObject house; // Used for teleport location and accessing children of the house
    private GameObject houseSpawnPoint; // Spawn position for the player on start and when reviving

    // Movement   
    [HideInInspector] public Vector3 moveDirection; // Direction the player is moving towards
    private bool left, right, forwards, backwards; // Used for movement animations based on direction
    private float horizontalMovement; // Horizontal variable movement for both controller and Keyboard
    private float verticalMovement; // Vertical variable movement for both controller and Keyboard  

    // Speed
    private float playerCurrentSpeed = 0f; // Speed applied to the player
    private float playerWalkSpeed = 5.0f; // Player walking speed
    private float playerSprintSpeed = 8.0f; // Player running speed
    private float playerBowSpeed = 3.0f; // Player speed while aiming the bow
    [HideInInspector] public bool running = false; // Is the player running
    private float runTimer = 0.0f; // Timer before player can run again after running out of stamina
    private bool canRun = true; // Boolean to trigger above timer or not

    // Gathering
    private bool gathering = false; // Gathering is true when the player starts gathering, and false once the cooldown is over
    private float gatherTimer = 0.0f; // Timer to reset gathering
    private float resetGather = 1f; // Gathering cooldown

    // Evade
    private float evadeCooldown = 2f; // Evade cooldown
    [HideInInspector] public bool evading = false; // Used to apply evade effects
    [HideInInspector] public bool canEvade = true; // Allows the player to evade again when cooldown is done
    [HideInInspector] public bool playerCanBeDamaged = true; // Player is invincible while evading
    public GameObject evadeCooldownImage; // Game object used to show the cooldown of the evade
    private Vector3 evadeImageStartPos = new Vector3(0f, -95f, 0f); // Initial position of the image
    private float evadeTimer;

    // Shooting
    [HideInInspector] public bool canShoot; // Used to allow the player to shoot the bow. True when aiming
    [HideInInspector] public float bowDamage = 1.25f; // Bow Damage
    private float bowTimer = 0.0f; // Timer to reset shooting
    private float resetBow = 1f; // Shooting cooldown
    private bool shooting = false; // Shooting is true when the play starts shooting, and false once the cooldown is over
    private bool initialShot; // Used to add cooldown to the first arrow shot, to match the anim
    [HideInInspector] public bool upgradedBow; // Used to enable the upgraded bow perks
    [HideInInspector] public bool firing; // Used to stop the player from moving while the firing animation is being played

    // Spawn Turret
    private bool turretSpawned; // Used to manage the turret cooldown once the turret is spawned
    private float turretTimer = 0.0f; // Timer to reset the turret ability
    private float resetTurret = 60.0f; // Turret cooldown
    private float turretDuration = 20.0f; // The time the turret stays before disappearing
    private GameObject miniTurret; // Object to hold the instantiated turret

    // Gravity
    private float playerGrav = -19.62f; // Gravity Strength
    private float verticalVelocity; // Player vertical velocity to apply gravity to

    // Energy
    [HideInInspector] public float playerEnergy = 0f; // Player's current energy level used for sprinting
    private float maxPlayerEnergy = 100f; // Maximum energy the player can have
    public Image playerEnergyBarImage; // Energy bar UI
    private float energySpeed = 12.5f; // Speed at which the energy regenerates

    // Health
    private bool deathEffects = false; // Used to apply everything that comes with dying
    [HideInInspector] public bool playerDead = false; // Is true for the duration of the player being dead
    [HideInInspector] public bool playerRespawned; // Used to check if the player respawned and to avoid double interaction with single key press
    [HideInInspector] public float playerHealth = 0.0f; // Player's current health
    private float maxHealth = 100.0f; // Maximum health the player can have
    private float newHealth; // Used to pick up on changes in the health amount
    public HealthBar healthBar; // Health bar UI
    public HealthBar houseHealthBar; // Health bar UI
    // Eating
    [HideInInspector] public int appleAmount = 0; // The current amount of apples being held by the player
    private float appleHealAmount = 10.0f; // The amount of health restored when eating an apple
    [HideInInspector] public int maxApple = 5; // The maximum amount of apples the player can hold

    // Attacks
    [HideInInspector] public float attackDamage = 1.0f; // The damage dealt by the melee attack
    [HideInInspector] public float attackTimer = 0.0f; // Timer to reset the attack
    private float resetAttack = 0.75f; // Attack cooldown
    private float comboTimer = 0.0f; // Timer to reset the combo
    private float resetCombo = 1.10f; // Combo cooldown
    private float staggerCD; // Combo cooldown
    public float staggerCDMax = 1.5f; // Combo cooldown
    private int attackCount = 0; // Current attack number in the combo
    [HideInInspector] public Building currentResource; // Current resource type being gathered
    public bool damageEnemy = false; // Used to enable a time frame during the animation where the enemy can be damaged
    [HideInInspector] public bool lunge = false; // Used to move the player forward (lunge) during their attack
    [HideInInspector] public bool upgradedMelee; // Used to enable the upgraded melee perks
    public bool attackAnimEnded = true; // Safety bool to avoid attack issues. Becomes true using anim behaviour, and is needed to attack again

    // States
    [Header("Player States")]
    public bool playerCanMove = true;
    public bool playerisMoving = false;
    public bool attacking = false;
    public bool canGetInHouse;
    public bool bridgeInteract;
    public bool poisoned;
    public Animation poisonAnimation;
    public bool rooted;
    public GameObject spiderWeb;
    private bool freezeRotation;
    private Vector3 pushDirection;
    private bool staggered;

    // Teleporter
    public bool canTeleport = false;
    public bool teleporting;

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
    public Animator ResourceHolderAnimator;
    public bool lastWasAxe;

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

    public Buttons pauseButtons;
    public GameObject DarkenGameObject;
    public GameObject MiniTurretUI;

    // Pause
    [HideInInspector] public bool pausedBool;
    public Animation UIAnimation;
    public GameObject turretTierOne;
    public GameObject turretTierTwo;
    public Image turretBoarderImage;
    public Image biomeNameImage;
    public Image controllerImage;
    public Sprite controllerSprite;
    public Sprite keyboardSprite;
    public GameObject HUDGameObject;
    // Death
    private float respawnTimer = 0.0f;
    private bool textAnimated = false;
    [HideInInspector] public bool respawning;
    private GameObject respawnText;

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
    [HideInInspector] public bool aimingCTRL;
    public bool aiming;
    private bool evadeCTRL;
    private bool cancelCTRL;
    private bool turretCTRL;
    private bool healCTRL;
    public bool interactCTRL;
    public bool needInteraction = false;
    [HideInInspector] public bool lockingCTRL = false;
    [HideInInspector] public bool inventoryCTRL = false;
    [HideInInspector] public bool swapCTRL = false;
    public Vector2 rotateCTRL;
    [HideInInspector] public bool releasedCTRL;
    [HideInInspector] public bool scrollCTRL;

    // Keyboard Controls
    private KeyCode[] keyCodes;

    // Cancel
    [HideInInspector] public bool cancelAnimation;
    [HideInInspector] public bool cancelEffects;
    [HideInInspector] public bool cancelHit;

    // Lantern
    [HideInInspector] public bool LanternLighted;
    public SkinnedMeshRenderer LanternSkinnedRenderer;

    // Map
    [HideInInspector] public bool mapBool;
    public bool ResourceHolderOpened;
    Vector3 MapPanningPosition;
    private Vector3 mapMousePosition;
    public RectTransform turretMenuHolder;
    public TMP_Text turretMenuTitle;
    public Image turretImageIcon;

    // Animation
    private float speedAnim;
    private float transitionSpeed = 20f;
    private float transitionSpeedDirection = 40f;
    public Animator bowAnimator;
    public float horizontalAnim, verticalAnim;

    private bool playSoundOnce, playSoundOnce2;

    private bool wasHit;
    public bool canAttack = true;

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
            GameManager.global.gamepadControls.Controls.Move.performed += context => MoveController(true);
            GameManager.global.gamepadControls.Controls.Move.canceled += context => MoveController(false);

            // Right stick to rotate
            GameManager.global.gamepadControls.Controls.Rotate.performed += context => rotateCTRL = context.ReadValue<Vector2>();

            // A to sprint
            GameManager.global.gamepadControls.Controls.Sprint.performed += context => SprintController(true);
            GameManager.global.gamepadControls.Controls.Sprint.canceled += context => SprintController(false);
            // Right Joystick click to sprint
            GameManager.global.gamepadControls.Controls.Sprint2.performed += context => SprintController(true);
            GameManager.global.gamepadControls.Controls.Sprint2.canceled += context => SprintController(false);
            // Left Joystick click to sprint
            GameManager.global.gamepadControls.Controls.Sprint3.performed += context => SprintController(true);
            GameManager.global.gamepadControls.Controls.Sprint3.canceled += context => SprintController(false);

            // A to select in build mode
            GameManager.global.gamepadControls.Controls.Sprint.performed += context => BuildSelectController();

            // X to interact
            GameManager.global.gamepadControls.Controls.Interact.performed += context => InteractController();

            // Y to swap tool
            GameManager.global.gamepadControls.Controls.Swap.performed += context => SwappingController();

            // B to evade
            GameManager.global.gamepadControls.Controls.Evade.performed += context => EvadeController();
            GameManager.global.gamepadControls.Controls.Evade.performed += context => PauseVoid(false);

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
            GameManager.global.gamepadControls.Controls.Heal.performed += context => SwapTurret();

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
        if (ResourceHolderOpened != open)
        {
            ResourceHolderOpened = open;
            GameManager.PlayAnimator(ResourceHolderAnimator, "Resource Holder Appear", open, false);
        }
    }

    public void ShakeResourceHolder()
    {
        GameManager.PlayAnimator(ResourceHolderAnimator, "Resource Holder Shake");
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

    private void SwapTurret()
    {
        if (PlayerModeHandler.global.inTheFortress)
        {
            if (!scrollCTRL)
            {
                scrollCTRL = true;
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
        if (PlayerModeHandler.global.playerModes == PlayerModes.ResourceMode && !pausedBool && !mapBool)
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
        if (PlayerModeHandler.global.playerModes == PlayerModes.CombatMode && !pausedBool && !mapBool)
        {
            if (!attackingCTRL && !attacking)
            {
                attackingCTRL = true;
            }
        }
    }

    private void AimingController(bool pressed)
    {
        if (PlayerModeHandler.global.playerModes == PlayerModes.CombatMode && !pausedBool && !mapBool)
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
        if (!interactCTRL && needInteraction && !pausedBool && !mapBool)
        {
            interactCTRL = true;
        }
    }

    private void EvadeController()
    {
        if (!evadeCTRL && !pausedBool && !mapBool)
        {
            evadeCTRL = true;
        }
    }

    private void SwappingController()
    {
        if (!swapCTRL && !pausedBool && !mapBool)
        {
            swapCTRL = true;
            cancelCTRL = true;
        }
    }

    private void TurretController()
    {
        if (Unlocks.global.miniTurretUnlocked && !turretCTRL && !turretSpawned && !pausedBool && !mapBool)
        {
            turretCTRL = true;
        }
    }

    private void HealController()
    {
        if (!healCTRL && !pausedBool && !mapBool)
        {
            healCTRL = true;
        }
    }

    // CONTROLLER FUNCTIONS END

    private void Start()
    {
        // Game Objects
        if (GameObject.Find("Radius Camera"))
        {
            RadiusCamGameObject = GameObject.Find("Radius Camera");
        }

        respawnText = house.transform.GetChild(4).gameObject;

        // Setting default values
        playerCurrentSpeed = playerWalkSpeed;
        playerEnergy = maxPlayerEnergy;
        playerEnergyBarImage.fillAmount = 0.935f;
        playerHealth = maxHealth;
        newHealth = playerHealth;
        //healthBar.SetHealth(playerHealth, maxHealth);

        // Adding timers to array
        timers[0] = timer1;
        timers[1] = timer2;
        timers[2] = timer3;
        timers[3] = timer4;

        // Setting UI Text
        UpdateAppleText();

        keyCodes = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));

        ChangeTool(new ToolData() { AxeBool = true });

        playerCC.enabled = false;
        RotatePlayer();
        playerCC.enabled = true;

        speedAnim = 0f;
        staggerCD = staggerCDMax;
    }
    public bool debugfrocemap;
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L))
        {
            playerHealth = 0f;
        }

        if (debugfrocemap)
        {
            UpdateMap();
        }
#endif
        if (staggerCD > 0f)
        {
            staggerCD -= Time.deltaTime;
        }


        if (!canEvade)
        {
            evadeTimer += Time.deltaTime;
            evadeCooldownImage.transform.localPosition = Vector3.Lerp(evadeImageStartPos, new Vector3(evadeImageStartPos.x, evadeImageStartPos.y + 90f, evadeImageStartPos.z), evadeTimer / evadeCooldown);
        }

        if (mapBool)
        {
            UpdateMap();
        }

        if (pausedBool && !Input.GetKeyDown(KeyCode.Escape) || (mapBool && !Input.GetKeyDown(KeyCode.Tab) && !Input.GetKeyDown(KeyCode.Escape)))
        {
            return;
        }

        if (evading)
        {
            playerCanBeDamaged = false;

            playerCC.Move(transform.forward * 6.0f * Time.deltaTime);
            attacking = false;
            return;
        }
        else
        {
            playerCanBeDamaged = true;
        }

        if (Unlocks.global.extraApplesUnlocked)
        {
            maxApple = 10;
            UpdateAppleText();
            Unlocks.global.extraApplesUnlocked = false;
        }

        if (Unlocks.global.upgradedMeleeUnlocked)
        {
            UpgradeMelee();
        }

        if (Unlocks.global.upgradedBowUnlocked)
        {
            UpgradeBow();
        }

        if (playerCanMove)
        {
            // Controller
            if (GameManager.global.moveCTRL.x != 0 || GameManager.global.moveCTRL.y != 0)
            {
                horizontalMovement = GameManager.global.moveCTRL.x;
                verticalMovement = GameManager.global.moveCTRL.y;
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

            BlendTreeAnimation();

            // Mechanics
            Attack();
            Gathering();

            if (Unlocks.global.bowUnlocked)
            {
                Shoot();
            }

            ModeChanged();
            if (lunge && !playerisMoving)
            {
                AttackLunge();
            }
        }
        else
        {
            if (pushDirection != Vector3.zero)
                playerCC.Move(pushDirection * Time.deltaTime);
            horizontalMovement = 0;
            verticalMovement = 0;
            running = false;
        }

        UpdateHealth();
        HandleEnergy();
        TimersFunction();
        ScreenDamage();
        CheckCurrentTool();
        Resting();
        CalculateMovementAngle(moveDirection);

        if (poisoned)
        {
            GameManager.PlayAnimation(poisonAnimation, "Poison");
            StartCoroutine(PoisonDamage());
            poisoned = false;
        }

        if (rooted)
        {
            StartCoroutine(RootPlayer());
            rooted = false;
        }

        if (playerDead)
        {
            Death();
        }

        foreach (KeyCode keyCode in keyCodes)
        {
            KeyRead(keyCode);
        }

        if (playerHealth <= 0)
        {
            if (!playSoundOnce)
            {
                GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.PlayerDeath1Sound : GameManager.global.PlayerDeath2Sound);
                playSoundOnce = true;
            }
            playerCanMove = false;
            if (Boar.global.mounted)
            {
                Boar.global.Mount();
            }
            characterAnimator.SetTrigger("Death");
        }
    }

    private IEnumerator PoisonDamage()
    {
        for (int i = 0; i < 5; i++)
        {
            playerHealth -= 3f;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator RootPlayer()
    {
        playerCanMove = false;
        spiderWeb.SetActive(true);
        freezeRotation = true;
        characterAnimator.SetBool("Moving", false);
        cancelHit = true;

        yield return new WaitForSeconds(3.5f);

        playerCanMove = true;
        spiderWeb.SetActive(false);
        freezeRotation = false;
    }

    private void LateUpdate()
    {
        if (!pausedBool && !mapBool && !evading && !CameraFollow.global.bossCam && !freezeRotation)
        {
            RotatePlayer();
        }
    }

    private void UpdateHealth()
    {
        if (newHealth != playerHealth)
        {
            healthBar.SetHealth(playerHealth, maxHealth);
            float difference = playerHealth - newHealth;

            if (newHealth != 0 && difference != 0 && playerHealth < maxHealth)
                GameManager.PlayAnimation(UIAnimation, difference > 0 ? "Health Flash Green" : "Health Flash");

            newHealth = playerHealth;
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
        if (Input.GetKey(KeyCode.Q) || (PlayerModeHandler.global.playerModes == PlayerModes.CombatMode && Input.GetMouseButton(1)) || cancelCTRL || cancelHit)
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

        if (cancelAnimation)
        {
            characterAnimator.SetBool("Swapping", cancelAnimation);
            cancelAnimation = false;
        }
        else
        {
            characterAnimator.SetBool("Swapping", cancelAnimation);
        }
    }

    private IEnumerator RevertCancel()
    {
        yield return new WaitForSeconds(0.1f);
        cancelEffects = false;
    }

    private void Resting()
    {
        if (PlayerModeHandler.global.inTheFortress)
        {
            if (playerHealth < maxHealth)
            {
                playerHealth += Time.deltaTime * 2.0f;
            }
            else
            {
                playerHealth = maxHealth;
            }
        }
    }

    private void TimersFunction()
    {
        if (attacking)
        {
            if (!TickTimers(resetAttack, ref attackTimer))
            {
                attacking = false;
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

            int seconds = (int)resetTurret - (int)turretTimer % 60;

            if (seconds > 0)
            {
                turretText.text = seconds.ToString();
            }
            else
            {
                turretSpawned = false;
            }

            if (turretTimer >= turretDuration)
            {
                if (miniTurret)
                {
                    if (!playSoundOnce2)
                    {
                        GameManager.global.SoundManager.PlaySound(GameManager.global.MiniTurretDisappearSound, 1.0f, true, 0, false, miniTurret.transform);
                        playSoundOnce2 = true;
                    }
                    miniTurret.GetComponent<Defence>().enabled = false; //stops it shooting
                    Destroy(miniTurret, GameManager.PlayAnimation(miniTurret.GetComponent<Animation>(), "MiniTurretSpawn", false).length);
                }
            }
        }
        else
        {
            if (playSoundOnce2)
            {
                playSoundOnce2 = false;
            }
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
                    else
                    {
                        GameManager.global.SoundManager.PlaySound(GameManager.global.CantEatSound);
                        GameManager.PlayAnimation(UIAnimation, "Apple Shake");
                    }
                    break;

                case KeyCode.T:
                    if (Unlocks.global.miniTurretUnlocked && !turretSpawned && playerCanMove)
                    {
                        SpawnTurret();
                    }
                    else
                    {
                        GameManager.PlayAnimation(UIAnimation, "Turret Shake");
                    }
                    break;

                case KeyCode.E:
                    if (canTeleport)
                    {
                        TeleportPlayer(houseSpawnPoint.transform.position, false);
                    }
                    break;

                case KeyCode.Space:
                    if (playerCanMove && canEvade)
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
            if (evadeCTRL)
            {
                evadeCTRL = false;
                if (playerCanMove && canEvade)
                {
                    StartCoroutine(Evade());
                }
            }
            if (turretCTRL)
            {
                turretCTRL = false;
                if (playerCanMove)
                {
                    SpawnTurret();
                }
            }
            if (healCTRL)
            {
                healCTRL = false;
                if (playerCanMove && appleAmount > 0)
                {
                    EatApple();
                }
                else
                {
                    GameManager.global.SoundManager.PlaySound(GameManager.global.CantEatSound);
                }
            }
            if (canTeleport && interactCTRL)
            {
                TeleportPlayer(houseSpawnPoint.transform.position, false);
            }
        }
    }

    public void TeleportPlayer(Vector3 pos, bool houseInteract)
    {
        if (pos == Vector3.zero)
        {
            Debug.LogError("POSITION WAS ZERO. IF YOU SEE THIS MESSAGE SEE WHERE IT WAS CALLED FROM TY");
            return;
        }
        if (Boar.global.mounted)
        {
            Boar.global.transform.position = pos;
            Boar.global.animator.SetBool("Moving", false);
        }
        else
        {
            playerCC.enabled = false;
            transform.position = pos;
            playerCC.enabled = true;
            characterAnimator.SetBool("Moving", false);
        }
        interactCTRL = false;
        canTeleport = false;
        if (!Boar.global.canInteractWithBoar && !PlayerModeHandler.global.canInteractWithHouse && playerRespawned && !bridgeInteract)
        {
            needInteraction = false;
        }
        if (!houseInteract)
        {
            teleporting = true;
            GameManager.global.SoundManager.PlaySound(GameManager.global.TeleportSound, 0.7f);
        }

        if (Vector3.Distance(pos, CameraFollow.global.transform.position) > 15)
            CameraFollow.global.transform.position = pos + CameraFollow.global.offset();

        StartCoroutine(RevertBool(true));
    }
    [HideInInspector]
    public ToolData activeToolData = new PlayerController.ToolData() { HandBool = true };
    public void ChangeTool(ToolData toolData)
    {
        activeToolData = toolData;
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

    private void BlendTreeAnimation()
    {
        if (running && speedAnim <= 1f)
        {
            speedAnim += 0.1f * Time.deltaTime * transitionSpeed;
        }
        else
        {
            if (speedAnim >= 0f)
            {
                speedAnim -= 0.1f * Time.deltaTime * transitionSpeed;
            }
        }

        Mathf.Clamp(speedAnim, 0f, 1f);
        characterAnimator.SetFloat("Speed", speedAnim);
    }

    private void HandleSpeed()
    {
        if (playerisMoving && (Input.GetKey(KeyCode.LeftShift) || sprintingCTRL) && canRun && !staggered && !canShoot && !attacking)
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
            }
            else
            {
                playerCurrentSpeed = playerWalkSpeed;
            }
        }

        if (!canRun)
        {
            playerEnergyBarImage.color = new Color32(133, 133, 133, 255);
            runTimer += Time.deltaTime;
            if (runTimer > 2.5f)
            {
                playerEnergyBarImage.color = new Color32(255, 255, 255, 255);
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

        playerEnergyBarImage.fillAmount = Mathf.Lerp(0.0f, 1f, playerEnergy / maxPlayerEnergy);

        if (playerEnergy >= maxPlayerEnergy)
        {
            playerEnergy = maxPlayerEnergy;
        }
        else if (playerEnergy <= 0)
        {
            playerEnergy = 0;
            canRun = false;
        }
    }

    private IEnumerator Evade()
    {
        if (playerisMoving)
        {
            transform.LookAt(transform.position + (moveDirection * 10));
        }

        canShoot = false;
        bowAnimator.SetBool("Aiming", false);
        lunge = false;
        canEvade = false;
        evading = true;
        characterAnimator.ResetTrigger("Evade");
        characterAnimator.SetTrigger("Evade");
        staggered = false;
        GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerEvadeSound);
        evadeTimer = 0f;
        evadeCooldownImage.transform.localPosition = evadeImageStartPos;

        yield return new WaitForSeconds(evadeCooldown);
        canEvade = true;
    }

    public void PauseVoid(bool pause)
    {
        if (pause != pausedBool)
        {
            if (pauseButtons.ChangeMenu(0))
            {
                if (!pause)
                    return;
            }

            if (GameManager.global.GetComponent<Animation>().IsPlaying("Load Out"))
                GameManager.PlayAnimation(GameManager.global.GetComponent<Animation>(), "Load Out", true, true);

            PauseCanvasGameObject.SetActive(pause);
            controllerImage.sprite = GameManager.global.KeyboardBool ? keyboardSprite : controllerSprite;
            GameManager.PlayAnimator(UIAnimation.GetComponent<Animator>(), "Pause Appear", pause);

            GameManager.global.SoundManager.PlaySound(GameManager.global.PauseMenuSound);

            if (!mapBool)
            {
                Time.timeScale = pause ? 0 : 1;
            }
            else
            {
                mapBool = false;
            }
            pausedBool = pause;
        }
    }

    public void MapVoid(bool map)
    {
        if (!pausedBool)
        {
            GameManager.PlayAnimator(UIAnimation.GetComponent<Animator>(), "Map Appear", map);


            GameManager.global.SoundManager.PlaySound(map ? GameManager.global.MapOpenSound : GameManager.global.MapCloseSound);
            Time.timeScale = map ? 0 : 1;
            mapBool = map;
            if (mapBool)
            {
                UpdateMap();
                MapPanningPosition = new Vector2(-MapPlayerRectTransform.anchoredPosition.x, -MapPlayerRectTransform.anchoredPosition.y - 200);
                if (!ResourceHolderOpened)
                    UpdateResourceHolder(showCosts: false);
            }

            OpenResourceHolder(map);
        }
    }

    void UpdateMap()
    {
        MapPlayerRectTransform.anchoredPosition = ConvertToMapCoordinates(transform.position);

        float speed = 10f * Time.unscaledDeltaTime;

        if (GameManager.global.KeyboardBool)
        {
            if (Input.GetMouseButton(0))
            {
                //  Vector3 dragDirection = (Input.mousePosition - mapMousePosition).normalized;
                //  mapMousePosition = Input.mousePosition;
                //  MapPanningPosition += dragDirection * speed;

                Vector3 currentMousePosition = Input.mousePosition;
                Vector3 mouseVelocity = (currentMousePosition - mapMousePosition) / Time.unscaledDeltaTime;

                MapPanningPosition += mouseVelocity * speed * Time.unscaledDeltaTime;
                // MapPanningPosition = Vector3.Slerp(MapPanningPosition, MapPanningPosition + dragDirection, speed);
            }

            mapMousePosition = Input.mousePosition;
        }
        else
        {
            MapPanningPosition -= new Vector3(GameManager.global.moveCTRL.x, GameManager.global.moveCTRL.y) * speed;
        }

        MapSpotHolder.GetComponent<RectTransform>().anchoredPosition = MapPanningPosition;

        MapPlayerRectTransform.eulerAngles = new Vector3(0, 0, -transform.eulerAngles.y + 45);
        MapPlayerRectTransform.SetAsLastSibling(); //keeps it ontop

    }

    public void UpdateResourceHolder(int bridgeTypeInt = 0, int upgradeTypeInt = 0, bool showCosts = true)
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

        if (upgradeTypeInt == -1) //repair
        {
            woodCostList[0].ResourceCost = -10;
            stoneCostList[0].ResourceCost = -5;
        }
        else if (upgradeTypeInt == -2) //destroy
        {
            woodCostList[0].ResourceCost = -5;
        }
        else if (upgradeTypeInt > 0) //upgrade
        {
            woodCostList[0].ResourceCost = -5;
            stoneCostList[0].ResourceCost = -5;
        }
        else if (bridgeTypeInt == 1)
        {
            woodCostList[0].ResourceCost = -30;
            stoneCostList[0].ResourceCost = -10;
        }
        else if (bridgeTypeInt == 2)
        {
            woodCostList[1].ResourceCost = -30;
            stoneCostList[1].ResourceCost = -10;
        }
        else if (bridgeTypeInt == 3)
        {
            woodCostList[2].ResourceCost = -30;
            stoneCostList[2].ResourceCost = -10;
        }
        else if (bridgeTypeInt == 4)
        {
            woodCostList[0].ResourceCost = -10;
            stoneCostList[0].ResourceCost = -10;
            woodCostList[1].ResourceCost = -10;
            stoneCostList[1].ResourceCost = -10;
            woodCostList[2].ResourceCost = -10;
            stoneCostList[2].ResourceCost = -10;
        }
        else if (PlayerModeHandler.global.playerModes == PlayerModes.BuildMode)
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
                woodCostList[2].ResourceCost = -10;
                stoneCostList[2].ResourceCost = -10;
            }
        }

        //      Debug.Log("UPDATE");

        ResourceGenerate(LevelManager.global.WoodTierList, woodCostList, showCosts);
        ResourceGenerate(LevelManager.global.StoneTierList, stoneCostList, showCosts);
    }

    void ResourceGenerate(List<LevelManager.TierData> tierList, List<LevelManager.TierData> costList, bool showCosts = true)
    {

        for (int i = 0; i < tierList.Count; i++)
        {
            tierList[i].ResourceCost = costList[i].ResourceCost;

            if (showCosts && costList[i].ResourceCost == 0)
            {
                continue;
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
                //   Debug.Log(tierList[i].ResourceAmount + " += " + tierList[i].ResourceCost);
                tierList[i].ResourceAmount += tierList[i].ResourceCost;
            }
        }

        return true;
    }
    public Vector2 ConvertToMapCoordinates(Vector3 position)
    {
        // Step 2: Convert to 2D screen space using the map camera
        position.y = position.z;
        position -= new Vector3(110, 60);
        position *= 1.9f;
        // Step 3: Normalize the screen position based on your map's size or aspect ratio
        // Vector2 normalizedMapPosition = new Vector2(screenPosition.x / Screen.width, screenPosition.y / Screen.height);

        return position;
    }

    private void ApplyMovement(float _horizontalMove, float _verticalMove)
    {
        playerisMoving = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) || (movingCTRL);

        characterAnimator.SetBool("Moving", playerisMoving);

        if (playerisMoving)
        {
            moveDirection = new Vector3(_horizontalMove, 0.0f, _verticalMove);

            if (moveDirection.magnitude > 1)
            {
                moveDirection.Normalize();
            }

            moveDirection *= playerCurrentSpeed;

            moveDirection = Quaternion.AngleAxis(45, Vector3.up) * moveDirection;

            ApplyGravity();
        }
        else
        {
            moveDirection.x = 0f;
            moveDirection.z = 0f;
        }

        if (!playerDead && !firing)
        {
            playerCC.Move(moveDirection * Time.deltaTime);
        }
    }
    public float division = 1;
    private void RotatePlayer()
    {
        if (!Boar.global.mounted)
        {
            if (GameManager.global.KeyboardBool)
            {
                Ray ray = LevelManager.global.SceneCamera.ScreenPointToRay(Input.mousePosition);

                Vector3 targetPosition = LevelManager.global.SceneCamera.ScreenToWorldPoint(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hitData, Mathf.Infinity, GameManager.ReturnBitShift(new string[] { "RotationRaycast", "Terrain" })))
                    targetPosition = new Vector3(hitData.point.x, 0, hitData.point.z) - LevelManager.global.SceneCamera.transform.up * 4;

                targetPosition.y = transform.position.y;
                transform.LookAt(targetPosition);
            }
            else
            {
                float angle = Mathf.Atan2(rotateCTRL.y, rotateCTRL.x) * Mathf.Rad2Deg - 135;
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, -angle, transform.eulerAngles.z);
            }
        }
    }

    private void CalculateMovementAngle(Vector3 _moveDirection)
    {
        Vector2 move = new Vector2(_moveDirection.x, _moveDirection.z);
        Vector2 look = new Vector2(transform.forward.x, transform.forward.z);
        float angle = Vector2.SignedAngle(move, look);

        if (playerisMoving)
        {
            if ((angle <= 45f && angle >= 0f) || (angle >= -45f && angle <= 0f))
            {
                forwards = true;
                backwards = false;
                left = false;
                right = false;
            }
            else if (angle < -45f && angle >= -135f)
            {
                forwards = false;
                backwards = false;
                left = true;
                right = false;
            }
            else if ((angle < -135f && angle >= -180f) || (angle >= 135f && angle <= 180f))
            {
                forwards = false;
                backwards = true;
                left = false;
                right = false;
            }
            else if (angle < 135f && angle >= 45f)
            {
                forwards = false;
                backwards = false;
                left = false;
                right = true;
            }
        }
        else
        {
            forwards = false;
            backwards = false;
            right = false;
            left = false;

            ResetValues(ref horizontalAnim);
            ResetValues(ref verticalAnim);
        }

        if (forwards || backwards)
        {
            ResetValues(ref horizontalAnim);

            if (forwards)
            {
                TransitionAnim(ref verticalAnim, true);
            }

            if (backwards)
            {
                TransitionAnim(ref verticalAnim, false);
            }
        }

        if (right || left)
        {
            ResetValues(ref verticalAnim);

            if (right)
            {
                TransitionAnim(ref horizontalAnim, true);
            }

            if (left)
            {
                TransitionAnim(ref horizontalAnim, false);
            }
        }

        characterAnimator.SetFloat("Horizontal", horizontalAnim);
        characterAnimator.SetFloat("Vertical", verticalAnim);
    }

    private void ResetValues(ref float directionFloat)
    {
        if (directionFloat != 0f)
        {
            directionFloat += (directionFloat < 0f ? 0.1f : -0.1f) * Time.deltaTime * transitionSpeedDirection;

            if (Mathf.Abs(directionFloat) < 0.05f)
            {
                directionFloat = 0f;
            }
        }
    }

    private void TransitionAnim(ref float directionFloat, bool positive)
    {
        if (Mathf.Abs(directionFloat) <= 1.0f)
        {
            directionFloat += (positive ? 0.1f : -0.1f) * Time.deltaTime * transitionSpeedDirection;

            if (Mathf.Abs(directionFloat) > 0.95f)
            {
                directionFloat = (positive ? 1f : -1f);
            }
        }
    }

    private void ApplyGravity()
    {
        if (playerCC.isGrounded && verticalVelocity < 0.0f)
        {
            verticalVelocity = -1.0f;
        }
        else
        {
            verticalVelocity += playerGrav * Time.deltaTime;
        }
        moveDirection.y = verticalVelocity;
    }

    public void HealthRestore(float amount)
    {
        playerHealth += amount;
        //healthBar.SetHealth(playerHealth, maxHealth);
    }

    private void UpgradeMelee()
    {
        characterAnimator.SetBool("Upgraded", true);
        resetAttack = 0.6f;
        resetCombo = 0.8f;
        upgradedMelee = true;
        Unlocks.global.upgradedMeleeUnlocked = false;
    }

    private void UpgradeBow()
    {
        Bow.global.fireForce = 60.0f;
        bowDamage = 1.5f;
        upgradedBow = true;
        Unlocks.global.upgradedBowUnlocked = false;
    }

    private void Attack()
    {
        if ((Input.GetMouseButtonDown(0) || attackingCTRL) && attackAnimEnded && !canShoot && !attacking && canAttack && PlayerModeHandler.global.playerModes == PlayerModes.CombatMode)
        {
            if (wasHit)
            {
                wasHit = false;
                return;
            }
            LevelManager.ProcessEnemyList((enemy) =>
            {
                enemy.canBeDamaged = true;
            });
            LevelManager.ProcessCampList((camp) =>
            {
                camp.canBeDamaged = true;
            });
            LevelManager.ProcessBossList((boss) =>
            {
                boss.canBeDamaged = true;
            });

            attackingCTRL = false;
            attacking = true;
            attackTimer = 0;
            comboTimer = 0;

            if (upgradedMelee)
            {
                attackDamage = 1.25f;
            }
            else
            {
                attackDamage = 1f;
            }

            switch (attackCount)
            {
                case 0:
                    characterAnimator.ResetTrigger("Swing1");
                    characterAnimator.SetTrigger("Swing1");
                    break;
                case 1:
                    characterAnimator.ResetTrigger("Swing2");
                    characterAnimator.SetTrigger("Swing2");
                    break;
                case 2:
                    if (upgradedMelee)
                    {
                        attackDamage = 1.75f;
                    }
                    else
                    {
                        attackDamage = 1.5f;
                    }
                    characterAnimator.ResetTrigger("Swing3");
                    characterAnimator.SetTrigger("Swing3");
                    break;
                default:
                    break;
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
            if (building.GetComponent<Building>().resourceObject == Building.ResourceType.Stone)
            {
                minDistanceFloat = 5.0f;
            }
            if (distanceFloat < minDistanceFloat)
            {
                if (distanceFloat <= smallestDistance)
                {
                    smallestDistance = distanceFloat;
                }
            }

            if (Facing(building.position, 75.0f) && building.GetComponent<Building>().health > 0 && distanceFloat < minDistanceFloat && distanceFloat == smallestDistance && PlayerModeHandler.global.playerModes == PlayerModes.ResourceMode)
            {
                currentResource = building.GetComponent<Building>(); //keep outside so cursor knows

                if (!gathering && (Input.GetMouseButton(0) || gatheringCTRL))
                {
                    gathering = true;
                    gatherTimer = 0;
                    ChangeTool(new ToolData() { AxeBool = currentResource.ReturnWood(), PickaxeBool = currentResource.ReturnStone(), HandBool = currentResource.resourceObject == Building.ResourceType.Bush });

                    if (currentResource.ReturnWood())
                    {
                        characterAnimator.ResetTrigger("Wood");
                        characterAnimator.SetTrigger("Wood");
                    }
                    else if (currentResource.ReturnStone())
                    {
                        characterAnimator.ResetTrigger("Stone");
                        characterAnimator.SetTrigger("Stone");
                    }
                    if (currentResource.resourceObject == Building.ResourceType.Bush)
                    {
                        characterAnimator.ResetTrigger("Bush");
                        characterAnimator.SetTrigger("Bush");
                    }
                }
            }
            else if (currentResource && building == currentResource.transform)
            {
                currentResource = null;
            }

        }, true); //true means natural
    }

    private void Shoot()
    {
        if (PlayerModeHandler.global.playerModes == PlayerModes.CombatMode)
        {
            if (Input.GetMouseButton(1) || aimingCTRL)
            {
                if (!initialShot)
                {
                    shooting = true;
                    bowTimer = 0.45f;
                    initialShot = true;
                }
                aiming = true;
                characterAnimator.SetBool("Aiming", true);
                ChangeTool(new ToolData() { BowBool = true });
                bowAnimator.SetBool("Aiming", true);
                canShoot = true;
            }
            else if (BowGameObject.activeSelf)
            {
                aiming = false;
                characterAnimator.SetBool("Aiming", false);
                bowAnimator.SetBool("Aiming", false);
                if (!firing)
                {
                    ChangeTool(new ToolData() { SwordBool = true });
                }
                canShoot = false;
            }
            else
            {
                initialShot = false;
            }

            if ((Input.GetMouseButtonDown(0) || attackingCTRL) && canShoot && !shooting)
            {
                characterAnimator.ResetTrigger("Shoot");
                characterAnimator.SetTrigger("Shoot");
                bowAnimator.ResetTrigger("Shoot");
                bowAnimator.SetTrigger("Shoot");
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

        Vector3 spawn = transform.position + (transform.forward * 2) - (Vector3.up * (transform.position.y - 0.48f));
        spawn.y = 0;

        miniTurret = Instantiate(PlayerModeHandler.global.turretPrefabs[0], spawn, transform.rotation);
        miniTurret.transform.localScale = new Vector3(0.3f, 1, 0.3f);
        miniTurret.GetComponent<Defence>().MiniTurret = true;
        miniTurret.GetComponent<Defence>().turn_speed = 10;
        miniTurret.GetComponent<Defence>().damage = 0.3f;
        miniTurret.GetComponent<Defence>().fireRate = 3f;
        miniTurret.GetComponent<Defence>().shootingRange = 10;
        GameManager.PlayAnimation(miniTurret.GetComponent<Animation>(), "MiniTurretSpawn");
        GameManager.global.SoundManager.PlaySound(GameManager.global.MiniTurretAppearSound, 1.0f, true, 0, false, miniTurret.transform);
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
        else
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.CantEatSound);
        }
    }

    public void UpdateAppleText()
    {
        appleText.text = appleAmount.ToString();

        appleText.color = appleAmount >= maxApple ? Color.green : Color.white;
    }

    public void AttackEffects()
    {
        if (wasHit)
        {
            wasHit = false;
            return;
        }

        if (!attackAnimEnded)
        {
            int randomInt = Random.Range(0, 3);

            if (attackCount == 0 || attackCount == 2)
            {
                LevelManager.global.VFXSlash.transform.position = transform.position;
                LevelManager.global.VFXSlash.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - 90.0f, transform.eulerAngles.z);
                LevelManager.global.VFXSlash.Play();
                if (attackCount == 0)
                {
                    if (randomInt == 1 || randomInt == 2)
                    {
                        GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerAttack1Sound, 0.9f);
                    }
                    GameManager.global.SoundManager.PlaySound(GameManager.global.SwordSwing1Sound);
                }
                else
                {
                    if (randomInt == 1 || randomInt == 2)
                    {
                        GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerAttack3Sound, 0.9f);
                    }
                    GameManager.global.SoundManager.PlaySound(GameManager.global.SwordSwing3Sound);
                }
            }
            else if (attackCount == 1)
            {
                LevelManager.global.VFXSlashReversed.transform.position = transform.position;
                LevelManager.global.VFXSlashReversed.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 90.0f, transform.eulerAngles.z);
                LevelManager.global.VFXSlashReversed.Play();
                if (randomInt == 1 || randomInt == 2)
                {
                    GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerAttack2Sound, 0.9f);
                }
                GameManager.global.SoundManager.PlaySound(GameManager.global.SwordSwing2Sound);
            }

            attackCount++;
            if (attackCount > 2)
            {
                attackCount = 0;
            }
        }
    }

    private void AttackLunge()
    {
        playerCC.Move(transform.forward * 7f * Time.deltaTime);
        LevelManager.global.VFXSlash.transform.position = transform.position;
        LevelManager.global.VFXSlashReversed.transform.position = transform.position;
    }

    public void GatheringEffects()
    {
        if (currentResource)
        {
            if (PickaxeGameObject.activeSelf)
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.PickaxeSound);

                LevelManager.global.VFXSparks.transform.position = currentResource.transform.position;
                LevelManager.global.VFXSparks.Play();
                LevelManager.global.VFXPebble.transform.position = currentResource.transform.position;
                LevelManager.global.VFXPebble.Play();
            }
            if (currentResource.resourceObject == Building.ResourceType.Bush)
            {
                StopCoroutine("ToolAppear");
                StartCoroutine("ToolAppear");
                GameManager.global.SoundManager.PlaySound(GameManager.global.BushSound, 0.5f);
            }
            else if (AxeGameObject.activeSelf)
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.AxeSound);

                LevelManager.global.VFXWoodChip.transform.position = currentResource.transform.position;
                LevelManager.global.VFXWoodChip.Play();
            }

            currentResource.TakeDamage(1);
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
        LevelManager.global.newDay = true;
        DayTMP_Text.text = "DAY " + (LevelManager.global.day + 1).ToString();
        RemaningTMP_Text.text = "Highscore: " + (PlayerPrefs.GetInt("Number of Days") + 1);

        if (LevelManager.global.day > PlayerPrefs.GetInt("Number of Days"))
        {
            RemaningTMP_Text.text = "Highscore Beaten!";
            PlayerPrefs.SetInt("Number of Days", LevelManager.global.day);
        }

        /*
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
        */
    }

    public void EnemiesTextControl()
    {
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
            LevelManager.global.waveEnd = false;
            GameManager.PlayAnimation(UIAnimation, "Enemies Appear", false);
        }
    }

    public void Death()
    {
        if (!deathEffects)
        {
            characterAnimator.gameObject.SetActive(false);
            GameManager.global.SoundManager.PlaySound(GameManager.global.SnoringSound, 0.2f, true, 0, true);
            LevelManager.global.VFXSleeping.Play();
            TeleportPlayer(house.transform.position, true);
            redBorders.gameObject.SetActive(false);
            playerCC.enabled = false;
            playerRespawned = false;
            deathEffects = true;
        }
        if (!playerRespawned)
        {
            respawnTimer += Time.deltaTime;
            playerHealth += 12 * Time.deltaTime;

            if (playerHealth >= maxHealth)
            {
                needInteraction = true;
                if (!textAnimated)
                {
                    LevelManager.FloatingTextChange(respawnText, true);
                    textAnimated = true;
                }
                if (Input.GetKeyDown(KeyCode.E) || interactCTRL)
                {
                    playSoundOnce = false;
                    characterAnimator.gameObject.SetActive(true);
                    characterAnimator.ResetTrigger("Death");
                    characterAnimator.SetTrigger("Respawn");
                    GameManager.global.SoundManager.StopSelectedSound(GameManager.global.SnoringSound);
                    LevelManager.global.VFXSleeping.Stop();
                    TeleportPlayer(houseSpawnPoint.transform.position, true);
                    playerCanMove = true;
                    playerDead = false;
                    deathEffects = false;
                    playerRespawned = true;
                    respawnTimer = 0.0f;
                    LevelManager.FloatingTextChange(respawnText, false);
                    textAnimated = false;
                    if (!Boar.global.canInteractWithBoar && !PlayerModeHandler.global.canInteractWithHouse && !canTeleport && !bridgeInteract)
                    {
                        needInteraction = false;
                    }
                    interactCTRL = false;
                    respawning = true;
                    StartCoroutine(RevertBool(false));
                }
            }
        }
    }

    private IEnumerator RevertBool(bool teleporter)
    {
        yield return new WaitForSeconds(0.2f);
        if (teleporter)
        {
            teleporting = false;
        }
        else
        {
            respawning = false;
        }
    }

    public void TakeDamage(float damage)
    {
        if (playerCanBeDamaged)
        {
            if (!Boar.global.mounted)
            {
                wasHit = true;
                if (staggerCD <= 0)
                {
                    StopCoroutine("Staggered");
                    StartCoroutine("Staggered");
                    cancelHit = true;
                    characterAnimator.ResetTrigger("Hit1");
                    characterAnimator.ResetTrigger("Hit2");
                    characterAnimator.ResetTrigger("Hit3");
                    characterAnimator.ResetTrigger("Swing1");
                    characterAnimator.ResetTrigger("Swing2");
                    characterAnimator.ResetTrigger("Swing3");
                    int random = Random.Range(1, 4);
                    if (random == 1)
                    {
                        characterAnimator.SetTrigger("Hit1");
                    }
                    else if (random == 2)
                    {
                        characterAnimator.SetTrigger("Hit2");
                    }
                    else
                    {
                        characterAnimator.SetTrigger("Hit3");
                    }
                    staggerCD = staggerCDMax;
                }

            }
            int randomInt = Random.Range(0, 3);
            AudioClip temp = null;
            switch (randomInt)
            {
                case 0:
                    temp = GameManager.global.PlayerHit1Sound;
                    break;
                case 1:
                    temp = GameManager.global.PlayerHit2Sound;
                    break;
                case 2:
                    temp = GameManager.global.PlayerHit3Sound;
                    break;
                default:
                    break;
            }
            GameManager.global.SoundManager.PlaySound(temp, 0.9f);
            playerHealth -= damage;
            displaySlash = true;
        }
    }

    private IEnumerator Staggered()
    {
        staggered = true;
        yield return new WaitForSeconds(0.33f);
        staggered = false;
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
        if (playerHealth <= 20.0f && !playerDead)
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

    public IEnumerator PushPlayer(float _waitTime)
    {
        playerCanMove = false;
        yield return new WaitForSeconds(_waitTime);
        playerCanMove = true;
        pushDirection = Vector3.zero; // Reset the push direction.
    }

    public void SetPushDirection(Vector3 _direction, float _pushForce)
    {
        pushDirection += _direction * _pushForce;
        pushDirection.y = 0;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            Vector3 direction = Boar.global.transform.position - transform.position;
            playerCC.Move(direction * Time.deltaTime * 2.0f);
        }
    }
}