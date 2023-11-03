using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SquidBoss : MonoBehaviour
{
    [HideInInspector]
    public BossSpawner bossSpawner;

    public bool fireBallAttack;
    public GameObject fireBallPrefab;
    public GameObject telegraphedCirclePrefab;
    float fireballTimer;

    [System.Serializable]
    public class FireBallData
    {
        public Vector3 landingPosition;
        public Transform fireball;
        public GameObject telegraphedCircle;
        public float startTime = Time.time;
    }
    public List<FireBallData> fireballList = new List<FireBallData>();

    private void Update()
    {
        if (bossSpawner.introCompleted && bossSpawner.bossEncountered)
        {
            if (fireballTimer > 1)
            {
                fireballTimer = 0;

                bossSpawner.bossAnimator.ResetTrigger("Fire Vomit");
                bossSpawner.bossAnimator.SetTrigger("Fire Vomit");

                LaunchFireball();
            }
            else
            {
                fireballTimer += Time.deltaTime;
            }
        }


        for (int i = 0; i < fireballList.Count; i++)
        {
            float height = 5.0f;
            float duration = 1.5f;

            float t = (Time.time - fireballList[i].startTime) / duration;
            Vector3 archPosition = Vector3.Lerp(transform.position, fireballList[i].landingPosition, t);
            archPosition.y += height * Mathf.Sin(t * Mathf.PI);
            fireballList[i].fireball.position = archPosition;

            if (Vector3.Distance(fireballList[i].fireball.position, fireballList[i].landingPosition) < 1)
            {
                Destroy(fireballList[i].fireball.gameObject);
                Destroy(fireballList[i].telegraphedCircle.gameObject);
                fireballList.RemoveAt(i);
            }
        }
    }
    public void LaunchFireball()
    {
        FireBallData fireBallData = new FireBallData();
        fireBallData.fireball = Instantiate(fireBallPrefab, transform.position, Quaternion.identity).transform;
        fireBallData.landingPosition = PlayerController.global.transform.position;
        fireBallData.landingPosition.y = 0;
        fireBallData.telegraphedCircle = Instantiate(telegraphedCirclePrefab, fireBallData.landingPosition, Quaternion.identity);
        // fireBallData.telegraphedCircle.GetComponentInChildren<TelegraphedAttack>().getRockObject(fireBallData.fireball.gameObject);
        fireballList.Add(fireBallData);
    }

    private void Damaged(float amount)
    {
        bossSpawner.UpdateHealth(-amount);

        if (bossSpawner.health <= 0)
        {
            bossSpawner.bossAnimator.SetTrigger("Death");
            GameManager.global.SoundManager.PlaySound(GameManager.global.BossRoarSound, 1f, true, 0, false, transform);

        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == PlayerController.global.SwordGameObject)
        {
            if (PlayerController.global.attacking && bossSpawner.canBeDamaged && PlayerController.global.damageEnemy)
            {
                GameObject tempVFX = Instantiate(PlayerController.global.swordVFX.gameObject, ((PlayerController.global.transform.position + transform.position) / 2) + PlayerController.global.transform.forward, Quaternion.identity);
                if (tempVFX.transform.position.y < 0)
                {
                    tempVFX.transform.position = new Vector3(tempVFX.transform.position.x, PlayerController.global.transform.position.y, tempVFX.transform.position.z);
                }
                tempVFX.GetComponent<VisualEffect>().Play();
                Destroy(tempVFX, 1.0f);
                GameManager.global.SoundManager.PlaySound(Random.Range(1, 3) == 1 ? GameManager.global.SpiderBossHit1Sound : GameManager.global.SpiderBossHit2Sound, 1f, true, 0, false, transform);
                bossSpawner.canBeDamaged = false;
                ScreenShake.global.ShakeScreen();
                Damaged(PlayerController.global.attackDamage);
                PlayerController.global.StartCoroutine(PlayerController.global.FreezeTime());
            }
        }
        if (other.gameObject.tag == "Arrow" && other.GetComponent<ArrowTrigger>())
        {
            if (!other.GetComponent<ArrowTrigger>().singleHit)
            {
                GameManager.global.SoundManager.PlaySound(Random.Range(1, 3) == 1 ? GameManager.global.SpiderBossHit1Sound : GameManager.global.SpiderBossHit2Sound, 1f, true, 0, false, transform);
                other.GetComponent<ArrowTrigger>().singleHit = true;
                Damaged(PlayerController.global.bowDamage);
                if (!PlayerController.global.upgradedBow || other.GetComponent<ArrowTrigger>().hitSecondEnemy)
                {
                    Destroy(other.gameObject.transform.parent.gameObject);
                }
            }
        }
    }
}
