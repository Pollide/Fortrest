using UnityEngine;
using System.Collections.Generic;

public class BossSpawner : MonoBehaviour
{
    [HideInInspector]
    public bool bossAwakened;

    [SerializeField] public float Arenasize = 40f;
    // Holds the current boss type
    public TYPE bossType;
    public GameObject BossCanvas;

    // Enum for boss type
    public enum TYPE
    {
        Chieftain,
        Basilisk,
        SpiderQueen,
        Hrafn,
        Lycan,
        Squid
    }

    // [HideInInspector]
    public float health = 100;
    // Holds the bosses max health
    public float maxHealth = 100f;
    //[HideInInspector]
    public bool bossEncountered;
    [HideInInspector]
    public bool canBeDamaged = true;
    [HideInInspector]
    public Vector3 StartPosition;
    public float introDuration = 3.0f; // Duration of the intro animation
    public float cameraDistance = 5.0f; // Distance between the camera and the enemy
    public Vector3 introPositionOffset = new(0, 2, -2); // Offset from the enemy's position during intro
    private Transform initialCameraTransform;
    private float introTimer = 0.0f;
    [HideInInspector]
    public bool introCompleted = false;
    [SerializeField] private GameObject introCard;
    [SerializeField] public Animator bossAnimator;

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

            if (bossType == TYPE.Chieftain)
                GetComponent<BossStateMachine>().BossAnimator.gameObject.SetActive(false);
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
        LevelManager.global.dayPaused = open;

        if (bossEncountered != open)
        {
            if (health <= 0)
            {
                GameManager.PlayAnimation(BossCanvas.GetComponent<Animation>(), "Boss Health Death");
            }
            else
            {
                GameManager.PlayAnimation(BossCanvas.GetComponent<Animation>(), "Boss Health Appear", open);
            }

            BossMusicBegin(open);
        }

        UpdateHealth();


        bossEncountered = open;
    }

    public void BossMusicBegin(bool open)
    {
        LevelManager.global.activeBossSpawner = open ? this : null;

        LevelManager.global.SetGameMusic();
    }


    private void Update()
    {
        if (CheckPlayerDistance())
        {
            if (!bossAwakened)
            {
                if (GetComponent<BossStateMachine>())
                {
                    GetComponent<BossStateMachine>().enabled = true;
                    GetComponent<BossStateMachine>().BossAnimator.gameObject.SetActive(true);
                }
                else
                {
                    bossAnimator.SetTrigger("Awaking");
                }

                if (GetComponent<BirdBoss>())
                {
                    GameManager.global.SoundManager.PlaySound(GameManager.global.BirdBossEncounterSound);
                    bossAnimator.SetTrigger("TakeOff");
                }

                bossAwakened = true;

            }
            else
            {
                BossEncountered(true);

                if (!introCompleted)
                {
                    if (!CameraFollow.global.bossCam)
                    {
                        ScreenShake.global.duration = 3f;
                        bossAnimator.Rebind();
                        bossAnimator.Update(0f);
                    }

                    PlayerController.global.characterAnimator.SetBool("Moving", false);
                    introTimer += Time.deltaTime;



                    //Calculate the interpolation factor
                    float introProgress = Mathf.Clamp01(introTimer / introDuration);

                    //Perform the intro animation
                    Vector3 targetPosition = transform.position + introPositionOffset;
                    Vector3 cameraPosition = Vector3.Lerp(initialCameraTransform.position, targetPosition - initialCameraTransform.forward, introProgress);
                    LevelManager.global.SceneCamera.transform.position = cameraPosition;

                    if (introProgress >= 1.0f)
                    {
                        introCompleted = true;

                        if (GetComponent<BossStateMachine>())
                        {
                            if (GetComponent<IdleState>().IdleRuns < 1 && bossType == BossSpawner.TYPE.Lycan)
                            {
                                GetComponent<BossStateMachine>().ChangeState(GetComponent<IdleState>());
                            }
                            else if (GetComponent<IdleState>().IdleRuns >= 1 && bossType == BossSpawner.TYPE.Lycan)
                            {
                                GetComponent<BossStateMachine>().ChangeState(GetComponent<IdleState>());
                            }
                            else if (bossType != BossSpawner.TYPE.Lycan)
                            {
                                GetComponent<BossStateMachine>().ChangeState(GetComponent<IdleState>());
                            }
                        }
                    }

                    bossAnimator.speed = introCompleted ? 1 : 0;
                    PlayerController.global.playerCanMove = !introCompleted;
                    CameraFollow.global.bossCam = introCompleted;
                    LevelManager.global.HUD.SetActive(!introCompleted);
                    introCard.SetActive(!introCompleted);
                }
            }
        }
        else
        {
            BossEncountered(false);
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