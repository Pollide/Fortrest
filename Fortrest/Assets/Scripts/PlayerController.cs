using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController global;

    public float moveSpeed = 5.0f;
    public Rigidbody rb;
    Vector3 movement;

    public bool attacking;

    public List<Transform> enemyList = new List<Transform>();

    void Awake()
    {
        global = this;
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.z = Input.GetAxisRaw("Vertical");

        if (Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < enemyList.Count; i++) // Goes through the list of targets
            {
                if (Vector3.Distance(transform.position, enemyList[i].transform.position) <= 1.5f) // Distance from player to enemy
                {
                    attacking = true;
                    enemyList[i].GetComponent<EnemyController>().chasing = true;
                    break;
                }
            }
        }
    }

    void FixedUpdate() // For physics to avoid framerate change issues
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
