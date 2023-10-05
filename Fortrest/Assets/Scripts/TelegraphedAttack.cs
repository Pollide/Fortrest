using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelegraphedAttack : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Transform innerShape;

    public float size = 0.0f;
    public float timer = 0.0f;
    private float duration = 0.0f;

    public bool isJumpIndicator;

    void Start()
    {
        duration = 2.2f;
        spriteRenderer = transform.parent.GetComponentInChildren<SpriteRenderer>();
        innerShape = transform.parent.GetChild(1);
    }

    void Update()
    {
        if (isJumpIndicator)
            Indicator(ref SpiderBoss.global.jumpAttackIndicator);
        else
            Indicator(ref SpiderBoss.global.webAttackIndicator);
    }

    void Indicator(ref bool indicator)
    {
        if (indicator)
            timer += Time.deltaTime;

        size = timer / duration;

        bool active = timer > 0 && size < 1.0f;

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
        if (other.gameObject == PlayerController.global.gameObject)
        {
            PlayerController.global.rooted = true;
        }
    }
}