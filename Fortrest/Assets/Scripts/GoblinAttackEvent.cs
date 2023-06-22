using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinAttackEvent : MonoBehaviour
{
    public EnemyController script;

    void Attack()
    {
        script.AnimationAttack();
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
