using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private float distance = 5f;
    [SerializeField] private bool hasRun = false;
    [SerializeField] private GameObject boss;
    [SerializeField] private Transform spawnPosition;

    private void Update()
    {
        if (CheckPlayerDistance() && hasRun == false)
        {
            Instantiate(boss, spawnPosition);
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