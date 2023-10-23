using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquidBoss : MonoBehaviour
{
    [HideInInspector]
    public BossSpawner bossSpawner;

    public void Awaken()
    {
        bossSpawner.bossAnimator.SetTrigger("Awaking");
    }

}
