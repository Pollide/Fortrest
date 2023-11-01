using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Creature : MonoBehaviour
{
    /* up to you if you want to use bring this back, its nice code
    enum MatType
    {
        White,
        Brown,
        Black,
        Cream,
        Grey
    }

    // [SerializeField] private MatType rabitType;
    // [SerializeField] private Material[] currentMat;
       */

    public NavMeshAgent navMeshAgent;
    public Animator animator;
    // Update is called once per frame

    float idleThreshold;
    float idleTimer;

    [SerializeField] private SkinnedMeshRenderer mesh;

    private void Start()
    {
        //  mesh.material = currentMat[(int)rabitType];

        if (LevelManager.global && Physics.Raycast(transform.position + Vector3.up * 2, -Vector3.up, out RaycastHit raycastHit, Mathf.Infinity, GameManager.ReturnBitShift(new string[] { "Terrain" })))
        {
            for (int i = 0; i < LevelManager.global.terrainDataList.Count; i++)
            {
                if (LevelManager.global.terrainDataList[i].terrain && LevelManager.global.terrainDataList[i].terrain.transform == raycastHit.transform)
                {
                    mesh.material = LevelManager.global.terrainDataList[i].rabbitMaterial;
                }
            }
        }
    }

    void Update()
    {
        float distance = PlayerController.global ? Vector3.Distance(transform.position, PlayerController.global.transform.position) : 999999;
        idleTimer += Time.deltaTime;
        bool run = distance < 12;

        if (idleTimer > idleThreshold || run)
        {
            idleThreshold = Random.Range(2, 5);
            idleTimer = 0;

            float range = 5;
            float x = Random.Range(-range, range);
            float z = Random.Range(-range, range);

            Vector3 destination = transform.position + new Vector3(x, 0f, z);

            if (run)
            {
                destination += transform.position - PlayerController.global.transform.position;
                navMeshAgent.speed = 10;
            }
            else
            {
                navMeshAgent.speed = 3.5f;
            }

            navMeshAgent.SetDestination(destination);
        }



        animator.SetBool("Run", 1 < Vector3.Distance(transform.position, navMeshAgent.destination));
    }

    private void OnTriggerStay(Collider other)
    {
        if (PlayerController.global)
        {
            if (other.gameObject == PlayerController.global.SwordGameObject || other.gameObject.tag == "Arrow")
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.ArrowHitBuildingSound, 1.0f);
                LevelManager.global.DeathParticle(transform);
                Destroy(gameObject);
            }
        }
    }
}