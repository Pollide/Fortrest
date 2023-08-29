using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    private enum Boss
    {
        Cheiften,
        Basalisk,
        Spider,
        Tier4,
        Tier5,
        Tier6
    }

    [SerializeField] Boss currentBoss;
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
