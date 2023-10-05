using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private float distance = 5f;
    [SerializeField] public bool hasRun = false;
    [SerializeField] private GameObject boss;
    [SerializeField] private Transform spawnPosition;

    // Holds the current boss type
    [SerializeField] public TYPE bossType;

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

    public float health = 100;

    [HideInInspector]
    public bool canBeDamaged = true;

    private void Start()
    {
        LevelManager.global.bossList.Add(this);
    }

    private void Update()
    {
        if (CheckPlayerDistance() && hasRun == false)
        {
            GameObject bossObject = Instantiate(boss, spawnPosition);

            if (bossObject.GetComponent<BossStateMachine>())
            {
                bossObject.GetComponent<BossStateMachine>().bossSpawner = this;
            }

            if (bossObject.GetComponent<SpiderBoss>())
            {
                bossObject.GetComponent<SpiderBoss>().bossSpawner = this;
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