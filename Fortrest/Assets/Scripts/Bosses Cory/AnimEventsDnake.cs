using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventsDnake : MonoBehaviour
{
    [SerializeField] private BossStateMachine stateMachine;
    [SerializeField] private AudioClip enrageAudio;
    [SerializeField] private GameObject introCard;


    void PlayEnrageSound()
    {
        GameManager.global.SoundManager.PlaySound(enrageAudio);
    }

    void PlayScreenShake()
    {
        ScreenShake.global.shake = true;
    }

    void ActivateIntroCard()
    {
        introCard.SetActive(true);
        stateMachine.BossAnimator.speed = 0f;
    }
}
