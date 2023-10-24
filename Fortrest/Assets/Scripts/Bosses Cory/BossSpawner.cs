using UnityEngine;
using System.Collections.Generic;

public class BossSpawner : MonoBehaviour
{
    [HideInInspector]
    public bool bossAwakened;
    bool initialIntro = true;
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
    public float cameraDistance = 5.0f; // Distance between the camera and the enemy
    public Vector3 introPositionOffset = new(0, 2, -2); // Offset from the enemy's position during intro
    public float introTimer = 0.0f;
    //[HideInInspector]
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
                BossEncountered(true);

                if (!introCompleted)
                {
                    PlayerController.global.characterAnimator.SetBool("Moving", false);
                    introTimer += Time.deltaTime;
                    float length = 6;
                    //Perform the intro animation
                    Vector3 targetPosition = transform.position + introPositionOffset;
                    LevelManager.global.SceneCamera.transform.position = Vector3.Lerp(LevelManager.global.SceneCamera.transform.position, targetPosition - LevelManager.global.SceneCamera.transform.forward, 2 * Time.deltaTime);

                    if (introTimer >= length)
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

                    bool show = introTimer > 3 && initialIntro && !introCompleted;


                    bossAnimator.speed = show ? 0 : 1;

                    if (show)
                        introCard.SetActive(true);

                    CameraFollow.global.bossCam = !introCompleted;


                    PlayerController.global.playerCanMove = introCompleted;
                    LevelManager.global.HUD.SetActive(introCompleted);
                }
            }
        }
        else
        {
            BossEncountered(false);
        }
    }

    public void BossIntro()
    {
        introTimer = 0;
        introCompleted = false;

        bossAnimator.Rebind();
        bossAnimator.Update(0f);
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