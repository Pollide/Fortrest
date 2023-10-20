using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelegraphedAttack : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Transform innerShape;
    public PhaseTwoAttack snakeSweep;

    public float size = 0.0f;
    public float timer = 0.0f;

    public bool isJumpIndicator;
    public bool isWebIndicator;
    public bool isSnakeIndicator;
    public bool isBirdIndicatorRectangle;
    public bool isBirdIndicatorCircle;
    private bool appearOnStart = true;

    void Start()
    {
        spriteRenderer = transform.parent.GetComponentInChildren<SpriteRenderer>();
        innerShape = transform.parent.GetChild(1);
    }

    void Update()
    {
        if (isJumpIndicator)
            Indicator(ref SpiderBoss.global.jumpAttackIndicator, 1.0f, 4.0f);
        if (isWebIndicator)
            Indicator(ref SpiderBoss.global.webAttackIndicator, 1.5f, 2.2f);
        if (isSnakeIndicator)
            Indicator(ref snakeSweep.coneIndicator, 1.5f, 2.2f);
        if (isBirdIndicatorRectangle)
            Indicator(ref BirdBoss.global.normalAttackIndicator, 2f, 2.5f, true);
        if (isBirdIndicatorCircle)
            Indicator(ref appearOnStart, 0.75f, 2f);
    }

    void Indicator(ref bool indicator, float multiplier, float duration, bool unique = false)
    {
        if (indicator)
            timer += Time.deltaTime;

        size = (timer * multiplier) / duration;

        bool active = timer > 0 && size < multiplier;

        if (unique)
        {
            innerShape.localScale = new Vector3(1, size, 0);
        }
        else
        {
            innerShape.localScale = new Vector3(size, size, 0);
        }
        
        spriteRenderer.enabled = active;

        if (!active)
        {
            size = 0;
            timer = 0;

            indicator = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isJumpIndicator && SpiderBoss.global.rootNow && other.gameObject == PlayerController.global.gameObject)
        {
            PlayerController.global.rooted = true;
            SpiderBoss.global.rootNow = false;
        }
        else if (isJumpIndicator && SpiderBoss.global.slamNow && other.gameObject == PlayerController.global.gameObject)
        {
            PlayerController.global.TakeDamage(20.0f, true);
            SpiderBoss.global.slamNow = false;
            int randomInt = Random.Range(0, 3);
            AudioClip temp = null;
            switch (randomInt)
            {
                case 0:
                    temp = GameManager.global.PlayerHit1Sound;
                    break;
                case 1:
                    temp = GameManager.global.PlayerHit2Sound;
                    break;
                case 2:
                    temp = GameManager.global.PlayerHit3Sound;
                    break;
                default:
                    break;
            }
        }
    }
}