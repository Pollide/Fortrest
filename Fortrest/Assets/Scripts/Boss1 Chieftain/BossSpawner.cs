using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private float Arenasize = 40f;
    [SerializeField] public bool hasRun = false;
    // Holds the current boss type
    [SerializeField] public TYPE bossType;
    public GameObject BossCanvas;

    // Enum for boss type
    public enum TYPE
    {
        Chieftain,
        Basilisk,
        SpiderQueen,
        Bird,
        Werewolf,
        Squid
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
            GetComponent<BossStateMachine>().bossAnimator.gameObject.SetActive(false);
        }

        if (GetComponent<SpiderBoss>())
        {
            GetComponent<SpiderBoss>().bossSpawner = this;
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
            if (!hasRun)
            {
                if (GetComponent<BossStateMachine>())
                {
                    GetComponent<BossStateMachine>().enabled = true;
                    GetComponent<BossStateMachine>().bossAnimator.gameObject.SetActive(true);
                }

                if (GetComponent<SpiderBoss>())
                {
                    GetComponent<SpiderBoss>().Awaken();
                }
                if (GetComponent<SquidBoss>())
                {
                    GetComponent<SquidBoss>().Awaken();
                }

                hasRun = true;

            }
            else
            {
                BossEncountered(true);
            }
        }
        else
        {
            BossEncountered(false);
        }
    }


    public bool CheckPlayerDistance()
    {

        return health > 0 && Vector3.Distance(PlayerController.global.transform.position, StartPosition) <= (hasRun ? Arenasize : 20);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, Arenasize);
    }
}