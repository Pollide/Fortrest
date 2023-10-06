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
    public bool canBeDamaged = true;

    public void Awake()
    {
        health = maxHealth;//on awake before the game loads
    }

    public void UpdateHealth()
    {
        BossCanvas.GetComponentInChildren<HealthBar>().SetHealth(health, maxHealth);
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
        BossCanvas.SetActive(open);
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

            GameManager.global.MusicManager.PlayMusic(GameManager.global.BossMusic);

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