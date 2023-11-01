using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelegraphLycan : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Transform innerShape;
    private Transform outerShape;
    public PhaseThreeLycan lycanPhaseThree;
    public FrenzyMode frenzyMode;
    public PhaseThreeAttack slam;
    public AttackManagerState atkManager;
    public float multiplier = 0.0f;
    public float duration = 0.0f;
    public float size = 0.0f;
    public float timer = 0.0f;
    public float pushDuration = 0.5f;
    public float pushForce = 3f;
    public float damage = 5f;
    public bool sweepIndicator;
    public bool lungeIndicator;
    public bool chiefIndicatorSwipe;
    public bool chiefIndicatorSlam;
    private bool doDamage;

    void Start()
    {
        spriteRenderer = transform.parent.GetComponentInChildren<SpriteRenderer>();
        innerShape = transform.parent.GetChild(1);
        outerShape = transform.parent.GetChild(0);
        doDamage = false;
    }

    void Update()
    {
        if (sweepIndicator)
        {
            Indicator(ref lycanPhaseThree.telegraph);
        }

        if (lungeIndicator)
        {
            Indicator(ref frenzyMode.telegraph, true);
            transform.parent.SetParent(null);
            transform.parent.position = frenzyMode.transform.position;
            transform.parent.localEulerAngles = new(0, frenzyMode.transform.localEulerAngles.y, frenzyMode.transform.localEulerAngles.z);
        }

        if (chiefIndicatorSwipe)
        {
            if (atkManager.inSwipe)
            {
                Indicator(ref atkManager.isAttacking);
            } 
        }

        if (chiefIndicatorSlam)
        {
            Indicator(ref slam.telegraphBool);
        }
    }

    void Indicator(ref bool indicator, bool unique = false)
    {
        if (indicator)
        {
            timer += Time.deltaTime;
            outerShape.gameObject.SetActive(true);
        }
        else
        {
            timer = 0f;
            outerShape.gameObject.SetActive(false);
        }

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

            if (indicator)
            {
                if (chiefIndicatorSwipe)
                {
                    atkManager.StateMachine.BossAnimator.speed = 1f;
                }
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
                PlayerController.global.TakeDamage(damage);
                StartCoroutine(DoAreaDamage());
            }
        }
    }

    public IEnumerator DoAreaDamage()
    {
        yield return new WaitForSeconds(0);

        Vector3 pushDirection = innerShape.transform.position - PlayerController.global.transform.position;
        float distanceToEnemy = pushDirection.magnitude;
        // Calculate the push force based on the distance
        float calculatedPushForce = pushForce / distanceToEnemy;
        Debug.Log(calculatedPushForce);
        float angle = Vector3.Angle(pushDirection, PlayerController.global.transform.position - innerShape.transform.position);
        pushDirection = Quaternion.Euler(0f, angle, 0f) * pushDirection;
        Debug.Log(pushDirection);
        PlayerController.global.SetPushDirection(pushDirection, calculatedPushForce);
        StartCoroutine(PlayerController.global.PushPlayer(pushDuration));
    }
}
