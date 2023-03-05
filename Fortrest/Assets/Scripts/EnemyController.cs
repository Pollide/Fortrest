using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // public float lookRadius = 10.0f; // Distance that the enemy will start chasing you

    NavMeshAgent agent; // Nav mesh agent component
    [SerializeField] Transform bestTarget; // Target that the enemy will go towards
    public bool chasing; // Enemy chases the player mode
    private Transform playerPosition;

    public float attackTimer;
    public float attackTimerMax = 200;

    void Start()
    {
        playerPosition = PlayerController.global.transform;
        agent = GetComponent<NavMeshAgent>(); // Finds the component by itself on the object the script is attached to
        PlayerController.global.enemyList.Add(transform); // Adding each object transform with this script attached to the enemy list
    }

    void Update()
    {
        float shortestDistance = 9999; // Assign huge default value

        if (LevelManager.global.BuildingList.Count == 0) // Enemies will chase the player if no structure is left around
        {
            chasing = true;
        }

        if (chasing)
        {
            bestTarget = playerPosition;

            if (LevelManager.global.BuildingList.Count != 0) // If there are still targets other than the player
            {
                Invoke("ChasePlayerTimer", 5.0f); // Enemy stops chasing the player after 5s

                float chaseDistance = Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position); // TEMPORARY EASY CODE

                if (chaseDistance >= 10.0f) // Enemy stops chasing when the player gets too far
                {
                    bestTarget = null;
                    chasing = false;
                }
            }

            if (bestTarget != null)
            {
                agent.SetDestination(bestTarget.position); // Makes the AI move

                if (Vector3.Distance(transform.position, bestTarget.position) <= agent.stoppingDistance + 0.3f) // Checks if enemy reached target
                {
                    FaceTarget();
                }
            }
        }
        else
        {
            if (!bestTarget) // If the enemy does not have a current target
            {
                for (int i = 0; i < LevelManager.global.BuildingList.Count; i++) // Goes through the list of targets
                {
                    if (LevelManager.global.BuildingList[i] != null)
                    {
                        float compare = Vector3.Distance(transform.position, LevelManager.global.BuildingList[i].transform.position); // Distance from enemy to each target

                        if (compare < shortestDistance) // Only true if a new shorter distance is found
                        {
                            shortestDistance = compare; // New shortest distance is assigned
                            bestTarget = LevelManager.global.BuildingList[i].transform; // Enemy's target is now the closest item in the list
                        }
                    }
                }
            }

            else
            {
                agent.SetDestination(bestTarget.position); // Sets the nav mesh agent destination

                if (Vector3.Distance(transform.position, bestTarget.position) <= agent.stoppingDistance + 0.6f) // Checks if enemy reached target
                {
                    attackTimer++;
                    Building building = bestTarget.GetComponent<Building>();
                    if (building)
                    {
                        if (building.GetHealth() > 0 && attackTimer >= attackTimerMax)
                        {
                            building.healthBarImage.fillAmount = Mathf.Clamp(building.GetHealth() / building.maxHealth, 0, 1f);
                            building.TakeDamage(1f);
                            attackTimer = 0;
                        }
                        else if (building.GetHealth() == 0)
                        {
                            RemoveTarget();
                            building.DestroyBuilding();
                        }
                    }
                }
            }
        }
    }

    private void FaceTarget() // Making sure the enemy always faces what it is attacking
    {
        Vector3 direction = (bestTarget.position - transform.position).normalized; // Gets a direction using a normalized vector
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // Obtaining a rotation angle
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f); // Smoothly rotating towards target
    }

    private void ChasePlayerTimer()
    {
        bestTarget = null;
        chasing = false;
    }

    public void RemoveTarget()
    {
        LevelManager.global.BuildingList.Remove(bestTarget); // Removes target from list
    }

    //private void OnDrawGizmosSelected() // Used to draw gizmos
    //{
    //    Gizmos.color = Color.magenta; // Defines the gizmo color
    //    Gizmos.DrawWireSphere(transform.position, lookRadius); // Draws sphere at x position of y radius
    //}
}
