using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelegraphLycan : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Transform innerShape;
    public PhaseThreeLycan lycanPhaseThree;
    public float multiplier = 0.0f;
    public float duration = 0.0f;
    public float size = 0.0f;
    public float timer = 0.0f;
    public float pushDuration = 0.5f;

    public bool sweepIndicator;
    public bool lungeIndicator;

    private bool pushBack;
    private bool doDamage;

    void Start()
    {
        spriteRenderer = transform.parent.GetComponentInChildren<SpriteRenderer>();
        innerShape = transform.parent.GetChild(1);
        doDamage = false;
    }

    void Update()
    {
        if (sweepIndicator)
        {
            if (pushBack)
            {
                SetPushBack(lycanPhaseThree.DirectionToPlayer, 10);
            }
            Indicator(ref lycanPhaseThree.telegraph);
        }
           
        if (lungeIndicator)
            Indicator(ref SpiderBoss.global.webAttackIndicator);

     
    }

    void Indicator(ref bool indicator)
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

            if (indicator)
            {
                doDamage = true;
            }
            else
            {
                doDamage = false;
            }

            indicator = false;
        }


    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == PlayerController.global.gameObject && PlayerController.global.playerCanBeDamaged)
        {
            if (doDamage)
            {
                PlayerController.global.TakeDamage(5);
                StartCoroutine(PushBack());
            }
        }
    }

    private void SetPushBack(Vector3 pushDirection, float pushForce)
    {
        PlayerController.global.playerCanMove = false;
        PlayerController.global.playerCC.Move(pushDirection * pushForce * Time.deltaTime);
    }

    public IEnumerator PushBack()
    {
        yield return new WaitForSeconds(pushDuration);

        pushBack = false;
        PlayerController.global.playerCanMove = true;
    }
}
