using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // public float lookRadius = 10.0f; // Distance that the enemy will start chasing you

    NavMeshAgent agent; // Nav mesh agent component
    Transform bestTarget; // Target that the enemy will go towards
    public List<Transform> targetsList; // List of possible targets for the enemy

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Finds the component by itself on the object the script is attached to
    }

    void Update()
    {
        // float distance = Vector3.Distance(target.position, transform.position);

        // if (distance <= lookRadius)
        // {
        //     agent.SetDestination(target.position);
        // }

        float shortestDistance = 9999; // Assign huge value

        if (!bestTarget) // If the enemy does not have a current target
        {
            for (int i = 0; i < targetsList.Count; i++) // Goes through the list of targets
            {
                float compare = Vector3.Distance(transform.position, targetsList[i].transform.position); // Distance from enemy to each target

                if (compare < shortestDistance) // Only true if a new shorter distance is found
                {
                    shortestDistance = compare; // New shortest distance is assigned
                    bestTarget = targetsList[i].transform; // Enemy's target is now the closest item in the list
                }
            }
        }

        else
        {
            agent.SetDestination(bestTarget.position); // Sets the nav mesh agent destination

            if (Vector3.Distance(transform.position, bestTarget.position) <= GetComponent<NavMeshAgent>().stoppingDistance + 0.3f) // Checks if enemy reached target
            {
                targetsList.Remove(bestTarget); // Removes target from list
                Destroy(bestTarget.gameObject); // Destroys target object
            }
        }
    }

    //private void OnDrawGizmosSelected() // Used to draw gizmos
    //{
    //    Gizmos.color = Color.magenta; // Defines the gizmo color
    //    Gizmos.DrawWireSphere(transform.position, lookRadius); // Draws sphere at x position of y radius
    //}
}
