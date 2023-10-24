using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelegraphedAttack : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Transform innerShape;
    public float size = 0.0f;
    public float timer = 0.0f;

    public bool isJumpIndicator;
    public bool isWebIndicator;
    public bool isSnakeIndicator;
    public bool isBirdIndicatorCircle;
    public bool isBirdIndicatorBigCircle;
    public bool isAsileMakerFireballIndicator;
    private bool appearOnStart = true;
    private GameObject rockObject;
    private bool damageNow;
    private bool pushBack;

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
        if (isBirdIndicatorCircle)
            Indicator(ref appearOnStart, 0.75f, 1.5f);
        if (isBirdIndicatorBigCircle)
            Indicator(ref BirdBoss.global.circleAttackIndicator, 0.75f, 1.65f);

        if (isAsileMakerFireballIndicator)
            Indicator(ref appearOnStart, 0.75f, 1.5f);

        if (pushBack)
        {
            PlayerController.global.playerCanMove = false;
            PlayerController.global.playerCC.Move(BirdBoss.global.directionToPlayer * 10.0f * Time.deltaTime);
        }
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
            if (isBirdIndicatorBigCircle && indicator)
            {
                StartCoroutine(TriggerDamage());
                GameManager.global.SoundManager.PlaySound(GameManager.global.BirdBossAttack3Sound);
            }
            if (rockObject)
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.BirdBossRockSound, 1f, true, 0, false, rockObject.transform);
                StartCoroutine(TriggerDamage());
                Destroy(rockObject);
            }
            size = 0;
            timer = 0;

            indicator = false;
        }
    }

    private IEnumerator TriggerDamage()
    {
        damageNow = true;
        yield return new WaitForFixedUpdate();
        damageNow = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == PlayerController.global.gameObject && PlayerController.global.playerCanBeDamaged)
        {
            if (!isWebIndicator && SpiderBoss.global.rootNow)
            {
                PlayerController.global.rooted = true;
                SpiderBoss.global.rootNow = false;
            }
            else if (isJumpIndicator && SpiderBoss.global.slamNow)
            {
                PlayerController.global.TakeDamage(20.0f);
                SpiderBoss.global.slamNow = false;
            }
            else if ((isBirdIndicatorCircle || isBirdIndicatorBigCircle) && damageNow)
            {
                if (isBirdIndicatorBigCircle && PlayerController.global.playerCanBeDamaged)
                {
                    pushBack = true;
                    StartCoroutine(PushBack());
                }
                PlayerController.global.TakeDamage(20.0f);
                damageNow = false;
            }
        }
    }

    public void getRockObject(GameObject rock)
    {
        rockObject = rock;
    }

    public IEnumerator PushBack()
    {
        yield return new WaitForSeconds(0.5f);

        pushBack = false;
        PlayerController.global.playerCanMove = true;
    }
}