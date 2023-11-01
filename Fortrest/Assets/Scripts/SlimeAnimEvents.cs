using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAnimEvents : MonoBehaviour
{
    public EnemyController script;

    void Attack()
    {
        script.AnimationAttack();
    }

    void StartJump()
    {
        script.lavaAttacks = true;
        script.freezeMovement = false;
    }

    void Death()
    {
        script.Death();
    }

    void StepOne()
    {
        script.FirstStep();
    }

    void StepTwo()
    {
        script.SecondStep();
    }
}
