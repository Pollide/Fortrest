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
    }

    void Indicator(ref bool indicator, float multiplier, float duration)
    {
        if (indicator)
            timer += Time.deltaTime;

        size = (timer * multiplier) / duration;

        bool active = timer > 0 && size < multiplier;

        innerShape.localScale = new Vector3(size, size, 0);
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
        }
    }
}