using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private float distance = 20f;
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
        Fire
    }

    [HideInInspector]
    public float health = 100;
    // Holds the bosses max health
    public float maxHealth = 100f;
    [HideInInspector]
    public bool bossEncountered;
    [HideInInspector]
    public bool canBeDamaged = true;

    public void Awake()
    {
        health = maxHealth;//on awake before the game loads
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
        if (CheckPlayerDistance() && hasRun == false)
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


            hasRun = true;
        }
    }


    private bool CheckPlayerDistance()
    {
        return Vector3.Distance(PlayerController.global.transform.position, transform.position) <= distance;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, distance);
    }
}