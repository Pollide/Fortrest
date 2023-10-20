using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquidBoss : MonoBehaviour
{
    public Animator animator;
    public void Awaken()
    {
        animator.SetTrigger("Awaking");
    }

}
