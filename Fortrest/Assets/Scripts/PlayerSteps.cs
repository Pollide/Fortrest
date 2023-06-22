using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSteps : MonoBehaviour
{
    void Attack()
    {
        
    }

    void StepOne()
    {
        PlayerController.global.FirstStep();
    }

    void StepTwo()
    {
        PlayerController.global.SecondStep();
    }
}
