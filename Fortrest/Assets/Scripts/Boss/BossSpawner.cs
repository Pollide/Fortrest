using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public enum Boss
    {
        Chieftain,
        Basilisk,
        Spider,
        SpiderQueen,
        Tier5,
        Tier6
    }

    public Boss currentBoss;
    [SerializeField] private float distance = 5f;
    [SerializeField] private bool hasRun = false;

    private void Start()
    {
      
    }
    private void Update()
    {
        if (CheckPlayerDistance())
        {

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
