using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseSounds : MonoBehaviour
{
    private void OpenSound()
    {
        GameManager.global.SoundManager.PlaySound(GameManager.global.HouseOpenSound, 1.0f, true, 0, false, transform);
    }

    private void CloseSound()
    {
        GameManager.global.SoundManager.PlaySound(GameManager.global.HouseCloseSound, 1.0f, true, 0, false, transform);
    }
}
