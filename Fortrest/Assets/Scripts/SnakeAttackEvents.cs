using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeAttackEvents : MonoBehaviour
{
    public EnemyController script;

    void Attack()
    {
        script.AnimationAttack();
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
