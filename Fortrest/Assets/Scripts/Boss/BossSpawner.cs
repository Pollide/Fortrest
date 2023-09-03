using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    private enum Bosses
    {
        Cheiften,
        Snake,
    }

    [SerializeField] private Bosses bossName;
    [SerializeField] private float distance = 5f;
    [SerializeField] private Transform spawnPos;
    [SerializeField] private GameObject bossPrefab;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (CheckPlayerDistance() && Input.GetKeyDown(KeyCode.B))
        {
            Instantiate(bossPrefab, spawnPos);
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
