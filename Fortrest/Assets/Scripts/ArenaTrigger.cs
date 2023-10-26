using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaTrigger : MonoBehaviour
{
    public BossSpawner bossSpawner;
    public bool EnteringBool;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Boar"))
        {
            if (Boar.global.mounted)
            {
                Boar.global.Mount();
            }

            Boar.global.currentSpeed = -50;
            Boar.global.MoveBoar();
        }

        bossSpawner.BossEncountered(EnteringBool);
    }
}