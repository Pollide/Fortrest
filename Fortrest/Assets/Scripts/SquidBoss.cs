using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
