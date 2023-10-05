using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelegraphedAttack : MonoBehaviour
{
    public GameObject circleIndicator;
    public GameObject coneIndicator;
    private SpriteRenderer circleRenderer;
    private SpriteRenderer coneRenderer;
    private Transform innerShapeCircle;
    private Transform innerShapeCone;

    public float size;
    public float timer = 0.0f;
    private float duration = 0.0f;

    void Start()
    {
        duration = 3.0f;
        circleRenderer = circleIndicator.GetComponentInChildren<SpriteRenderer>();
        coneRenderer = coneIndicator.GetComponentInChildren<SpriteRenderer>();
        circleRenderer.enabled = false;
        coneRenderer.enabled = false;
        innerShapeCircle = circleIndicator.transform.GetChild(1);
        innerShapeCircle.localScale = Vector3.zero;
        innerShapeCone = coneIndicator.transform.GetChild(1);
        innerShapeCone.localScale = Vector3.zero;
    }

    void Update()
    {
        if (SpiderBoss.global.jumpAttackIndicator)
        {
            Indicate(ref SpiderBoss.global.jumpAttackIndicator, ref circleRenderer, ref innerShapeCircle);
        }
        else if (SpiderBoss.global.webAttackIndicator)
        {
            Indicate(ref SpiderBoss.global.webAttackIndicator, ref coneRenderer, ref innerShapeCone);
        }      
    }

    private void Indicate(ref bool specialAttacking, ref SpriteRenderer spriteRenderer, ref Transform innerShape)
    {
        spriteRenderer.enabled = true;
        innerShape.localScale = new Vector3(size, size, 0);

        timer += Time.deltaTime;
        size = Mathf.Clamp01(timer / duration);

        if (size  == 1.0f)
        {
            specialAttacking = false;
            spriteRenderer.enabled = false;
            innerShape.localScale = new Vector3(0, 0, 0);        
            size = 0;
            timer = 0;
        }
    }
}