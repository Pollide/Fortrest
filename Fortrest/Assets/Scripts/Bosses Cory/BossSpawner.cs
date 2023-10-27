using UnityEngine;
using System.Collections.Generic;

public class BossSpawner : MonoBehaviour
{
    [HideInInspector]
    public bool bossAwakened;
    bool initialIntro = true;
    public bool escapeByDistance = true;
    [SerializeField] public float Arenasize = 40f;
    // Holds the current boss type
    public TYPE bossType;
    public GameObject BossCanvas;
    public bool DebugDamageBoss;
    // Enum for boss type
    public enum TYPE
    {
        Chieftain,
        Basilisk,
        SpiderQueen,
        Hrafn,
        Lycan,
        IsleMaker,
    }

    [HideInInspector]
    public float health = 100;
    // Holds the bosses max health
    public float maxHealth = 100f;
    //[HideInInspector]
    public bool bossEncountered;
    [HideInInspector]
    public bool canBeDamaged = true;
    [HideInInspector]
    public Vector3 StartPosition;
    [Header("Intro Variables")]
    public float cameraDistance = 5.0f; // Distance between the camera and the enemy
    public Vector3 introPositionOffset = new(0, 2, -2); // Offset from the enemy's position during intro
    public float introTimer = 0.0f;
    public float introLength = 6.0f;
    public float introSlowTimeStart = 3f;
    public float introSlowTimeEnd = 4f;
    public float slowRate = 0.3f;
    //[HideInInspector]
    public bool introCompleted = false;
    [SerializeField] private GameObject introCard;
    public Animator bossAnimator;

    public void Awake()
    {
        health = maxHealth;//on awake before the game loads
        StartPosition = transform.position;
    }

    public void UpdateHealth(float change = 0)
    {
        health += change;

        if (change < 0 && health > 0)
        {
            GameManager.PlayAnimation(BossCanvas.GetComponent<Animation>(), "Boss Health Damage");
        }

        health = Mathf.Clamp(health, 0, maxHealth);

        if (bossEncountered)
            BossCanvas.GetComponentInChildren<HealthBar>(true).SetHealth(health, maxHealth);
    }


    private void Start()
    {
        LevelManager.global.bossList.Add(this);

        if (health > 0)
            Indicator.global.AddIndicator(transform, Color.red, bossType.ToString(), false);

        if (GetComponent<BossStateMachine>())
        {
            GetComponent<BossStateMachine>().bossSpawner = this;
            GetComponent<BossStateMachine>().enabled = false;

            //if (bossType == TYPE.Chieftain)
            bossAnimator.gameObject.SetActive(false);
        }

        if (GetComponent<SpiderBoss>())
        {
            GetComponent<SpiderBoss>().bossSpawner = this;
        }

        if (GetComponent<SquidBoss>())
        {
            GetComponent<SquidBoss>().bossSpawner = this;
        }

        if (GetComponent<BirdBoss>())
        {
            GetComponent<BirdBoss>().bossSpawner = this;
        }
    }

    public void BossEncountered(bool open)
    {

        if (bossAwakened && enabled)
        {
            if (bossEncountered != open)
            {
                if (health <= 0)
                {
                    GameManager.PlayAnimation(BossCanvas.GetComponent<Animation>(), "Boss Health Death");


                    for (int i = 0; i < LevelManager.global.bridgeList.Count; i++)
                    {
                        LevelManager.global.bridgeList[i].CheckIndicators();
                    }
                    enabled = false;
                }
                else
                {
                    GameManager.PlayAnimation(BossCanvas.GetComponent<Animation>(), "Boss Health Appear", open);
                }

                LevelManager.global.activeBossSpawner = open ? this : null;
                UpdateHealth();
                Indicator.global.GetComponent<Canvas>().enabled = !open;
                LevelManager.global.dayPaused = open;

                if (open && LevelManager.global.messageDisplayed)
                {
                    LevelManager.global.messageDisplayed = false;
                    GameManager.PlayAnimation(PlayerController.global.UIAnimation, "Enemies Appear", false, true);
                }
            }

            bossEncountered = open;
        }
    }


    private void Update()
    {
        if (DebugDamageBoss)
        {
            DebugDamageBoss = false;
            UpdateHealth(-10);
        }
        if (CheckPlayerDistance())
        {
            if (!bossAwakened)
            {
                bossAwakened = true;
                BossIntro();

                if (GetComponent<BossStateMachine>())
                {
                    GetComponent<BossStateMachine>().enabled = true;
                    bossAnimator.gameObject.SetActive(true);

                    //   if(bossType == TYPE.Basilisk)
                    //   GameManager.global.SoundManager.PlaySound(enrageAudio);
                }
                else
                {
                    if (GetComponent<BirdBoss>())
                    {
                        GameManager.global.SoundManager.PlaySound(GameManager.global.BirdBossEncounterSound);
                        bossAnimator.SetTrigger("TakeOff");
                    }
                    else
                    {
                        bossAnimator.ResetTrigger("Awaking");
                        bossAnimator.SetTrigger("Awaking");
                        // Debug.Log("set off");
                    }
                }

            }
            else
            {
                if (escapeByDistance || !introCompleted)
                    BossEncountered(true);

                if (!introCompleted)
                {
                    PlayerController.global.characterAnimator.SetBool("Moving", false);
                    introTimer += Time.deltaTime;
                    // Perform the intro animation
                    Vector3 targetPosition = transform.position + introPositionOffset;
                    LevelManager.global.SceneCamera.transform.position = Vector3.Lerp(LevelManager.global.SceneCamera.transform.position, targetPosition - LevelManager.global.SceneCamera.transform.forward, 2 * Time.deltaTime);

                    if (introTimer >= introLength)
                    {
                        introCompleted = true;
                        initialIntro = false;
                    }

                    bool show = introTimer > introSlowTimeStart && initialIntro && !introCompleted && introTimer < introSlowTimeEnd;


                    Time.timeScale = show ? 0.5f : 1f;

                    if (show)
                        introCard.SetActive(true);

                    CameraFollow.global.bossCam = !introCompleted;


                    PlayerController.global.playerCanMove = introCompleted;
                    PlayerController.global.HUDGameObject.SetActive(introCompleted);
                }
            }
        }
        else if (escapeByDistance || health == 0)
        {
            BossEncountered(false);
        }
    }

    public void BossIntro()
    {
        introTimer = 0;
        introCompleted = false;

        if (bossAnimator.enabled)
        {
            bossAnimator.Rebind();
            bossAnimator.Update(0f);
        }
    }

    public bool CheckPlayerDistance()
    {
        return health > 0 && Vector3.Distance(PlayerController.global.transform.position, StartPosition) <= (bossAwakened ? Arenasize : 20);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, Arenasize);
    }
}