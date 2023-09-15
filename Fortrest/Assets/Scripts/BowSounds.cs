using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowSounds : MonoBehaviour
{
    private void AimSound()
    {
        GameManager.global.SoundManager.PlaySound(GameManager.global.BowAimSound);
    }

    private void FireSound()
    {
        GameManager.global.SoundManager.PlaySound(GameManager.global.BowFireSound);
    }
}
