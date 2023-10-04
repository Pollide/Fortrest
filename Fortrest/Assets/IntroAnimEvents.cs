using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroAnimEvents : MonoBehaviour
{
    public Animator bossAnimator;

    void ResumeAnim()
    {
        bossAnimator.speed = 1f;
    }
}
