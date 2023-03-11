using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltScript : MonoBehaviour
{
    public float speed = 1f;

    // Update is called once per frame
    void Update()
    {
        var step = speed * Time.deltaTime;
        List<GameObject> enemy = gameObject.GetComponentInParent<TurretShooting>().enemies;

        if (enemy.Count > 0)
        {
            transform.LookAt(enemy[0].transform);

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(enemy[0].transform.position.x, enemy[0].transform.position.y + 0.1f, enemy[0].transform.position.z), step);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyController>().Damaged();
            Destroy(gameObject);
        }
    }
}
