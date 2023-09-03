using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossState : MonoBehaviour
{
    protected BossStateMachine stateMachine;

    // Holds target transform
    protected Transform target;
    // Holds the initial spawn position of the boss
    protected Transform initialSpawn;

    private void Start()
    {
        // Populate target transform for targeting
        target = PlayerController.global.transform;

        initialSpawn = gameObject.transform.parent;
    }


    public bool PlayerInArena(float _radius)
    {
        return Vector3.Distance(initialSpawn.position, target.position) < _radius;
    }

    public void Initialize(BossStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public abstract void EnterState();

    public abstract void UpdateState();
    
    public abstract void ExitState();
}
