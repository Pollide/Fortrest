using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using TMPro;


public class PlayerController : MonoBehaviour
{
    public static PlayerController global;

    // Components
    CharacterController playerCC;
    public Animator CharacterAnimator;

    // Speed
    public float playerCurrentSpeed = 0f;
    private float playerWalkSpeed = 5.0f;
    private float playerSprintSpeed = 8.0f;
    public bool running = false;
    private float runTimer = 0.0f;
    private bool canRun = true;

    // Gravity
    public float playerGravMultiplier = 3f;
    private float playerGrav = -9.81f;
    private float playerVelocity;

    // Energy
    private float playerEnergy = 0f;
    private float maxPlayerEnergy = 100f;
    public Image playerEnergyBarImage;
    private float energySpeed = 12.5f;

    // Health
    private bool deathEffects = false;
    private bool playerDead = false;
    public float playerHealth = 0.0f;
    private float maxHealth = 100.0f;

    //public float playerJumpHeight = 10f;

    // Attacks
    public float attackDamage = 0.5f;
    public float attackTimer = 0.0f;
    private float resetAttack = 0.75f;
    public float comboTimer = 0.0f;
    private float resetCombo = 1.0f;
    public int attackCount = 0;
    public List<Transform> enemyList = new List<Transform>();
    public Building currentResource;

    // States
    [Header("Player States")]
    public bool playerCanMove = true;
    public bool playerisMoving = false;
    public bool attacking = false;

    // Movement
    private Vector3 moveDirection;

    private GameObject house;
    private GameObject houseSpawnPoint;
    public GameObject bodyShape;
    private GameObject interactText;

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
    }

    private float respawnTimer = 0.0f;
    private int lastAmount = 0;

    public HealthBar healthBar;
    private bool textAnimated = false;
    public bool gathering = false;
    public float gatherTimer = 0.0f;
    private float resetGather = 1.25f;

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

    private void HandleSpeed()
    {
        if (Input.GetKey(KeyCode.LeftShift) && canRun && CharacterAnimator.GetBool("Moving") == true)
        {
            running = true;
        }
        else
        {
            running = false;
        }

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

        playerEnergyBarImage.fillAmount = Mathf.Lerp(0.320f, 0.935f, playerEnergy / maxPlayerEnergy);

        if (playerEnergy >= maxPlayerEnergy)
        {
            playerEnergy = maxPlayerEnergy;
            playerEnergyBarImage.fillAmount = 0.935f;
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerHealth = 0.0f;
            healthBar.SetHealth(playerHealth);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseVoid(!PausedBool);
        }

        if (playerCanMove)
        {
            float horizontalMovement = Input.GetAxis("Horizontal");
            float verticalMovement = Input.GetAxis("Vertical");

            HandleSpeed();
            HandleEnergy();
            ApplyGravity();
            ApplyMovement(horizontalMovement, verticalMovement);
            Attack();
            Gathering();
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

        if (gathering)
        {
            gatherTimer += Time.deltaTime;

            if (gatherTimer >= resetGather)
            {
                gathering = false;
            }
        }

        if (playerHealth <= 0 || playerDead)
        {
            Death();
        }     
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
        playerisMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

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
                //attackCount++;
            }
            else if (attackCount == 1)
            {
                comboTimer = 0;
                CharacterAnimator.SetTrigger("Swing2"); // Play different animation here
                //attackCount++;
            }
            else if (attackCount == 2)
            {
                comboTimer = 0;
                CharacterAnimator.SetTrigger("Swing3"); // Play different animation here
                //attackCount = 0;
            }
        }
    }

    private void Gathering()
    {
        for (int i = 0; i < resourcesList.Count; i++)
        {
            if (resourcesList[i])
            {
                float minDistanceFloat = 4;
                float distanceFloat = Vector3.Distance(PlayerController.global.transform.position, resourcesList[i].transform.position);               

                if (FacingResource(resourcesList[i].transform.position) && !gathering && resourcesList[i].health > 0 && distanceFloat < minDistanceFloat && Input.GetMouseButton(0) && PlayerModeHandler.global.playerModes == PlayerModes.ResourceMode)
                {                  
                    gathering = true;
                    gatherTimer = 0;
                    currentResource = resourcesList[i];
                    PlayerController.global.ChangeTool(new PlayerController.ToolData() { AxeBool = resourcesList[i].resourceObject == Building.BuildingType.Wood, PicaxeBool = resourcesList[i].resourceObject == Building.BuildingType.Stone, HandBool = resourcesList[i].resourceObject == Building.BuildingType.Bush });
                    PlayerController.global.CharacterAnimator.ResetTrigger("Swing");
                    PlayerController.global.CharacterAnimator.SetTrigger("Swing");                                          
                }
            }
        }
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
        if (SwordGameObject.activeSelf)
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerAttackSound);
            GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.SwordSwing1Sound : GameManager.global.SwordSwing2Sound);
        }
        attackCount++;
    }

    public void GatheringEffects()
    {
        if (PicaxeGameObject.activeSelf)
        {
            int randomInt = Random.Range(0, 3);
            if (randomInt == 0)
                GameManager.global.SoundManager.PlaySound(GameManager.global.Pickaxe1Sound);
            if (randomInt == 1)
                GameManager.global.SoundManager.PlaySound(GameManager.global.Pickaxe2Sound);
            if (randomInt == 2)
                GameManager.global.SoundManager.PlaySound(GameManager.global.Pickaxe3Sound);

            //VFXSparks.transform.position = PlayerController.global.PicaxeGameObject.transform.position;
            VFXSparks.transform.position = currentResource.transform.position;
            VFXSparks.Play();
            //VFXPebble.transform.position = PlayerController.global.PicaxeGameObject.transform.position;
            VFXPebble.transform.position = currentResource.transform.position;
            VFXPebble.Play();
        }       
        if (currentResource != null)
        {
            if (AxeGameObject.activeSelf && currentResource.resourceObject != Building.BuildingType.Bush)
            {
                GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.TreeChop1Sound : GameManager.global.TreeChop2Sound);

                //VFXWoodChip.transform.position = PlayerController.global.AxeGameObject.transform.position;
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

            for (int i = 0; i < enemyList.Count; i++)
            {
                if (enemyList[i] && enemyList[i].GetComponent<EnemyController>() && enemyList[i].GetComponent<EnemyController>().currentEnemyType != EnemyController.ENEMYTYPE.wolf)
                {
                    goblinsInt++;
                }
            }

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
                if (!textAnimated)
                {
                    LevelManager.FloatingTextChange(interactText, true);
                    textAnimated = true;
                }
                if (Input.GetKeyDown(KeyCode.E))
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
    }

    private bool FacingResource(Vector3 enemyPosition) // Making sure the enemy always faces what it is attacking
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