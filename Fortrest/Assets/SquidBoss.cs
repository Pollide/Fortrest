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
    }
    public List<FireBallData> fireballList = new List<FireBallData>();

    private void Update()
    {
        if (bossSpawner.bossEncountered)
        {
            if (fireballTimer > 5)
            {
                fireballTimer = 0;
                FireBallData fireBallData = new FireBallData();
                fireBallData.fireball = Instantiate(fireBallPrefab, transform.position, Quaternion.identity).transform;
                fireBallData.landingPosition = PlayerController.global.transform.position;

                fireBallData.telegraphedCircle = Instantiate(telegraphedCirclePrefab, fireBallData.landingPosition, Quaternion.identity);
                // fireBallData.telegraphedCircle.GetComponentInChildren<TelegraphedAttack>().getRockObject(fireBallData.fireball.gameObject);

                float angle = 40 * Mathf.Deg2Rad;
                float distance = Vector3.Distance(fireBallData.landingPosition, transform.position);

                float horizontalVelocity = distance / (Mathf.Sin(2 * angle) / Physics.gravity.y);
                float verticalVelocity = Mathf.Sqrt(Mathf.Abs(Physics.gravity.y) * distance / Mathf.Sin(2 * angle));

                Vector3 velocity = new Vector3(0, verticalVelocity, horizontalVelocity);
                fireBallData.fireball.GetComponent<Rigidbody>().velocity = fireBallData.fireball.TransformDirection(velocity);

                fireballList.Add(fireBallData);
            }
            else
            {
                fireballTimer += Time.deltaTime;
            }
        }

        /*
        for (int i = 0; i < fireballList.Count; i++)
        {
            float angle = 20 * Mathf.Deg2Rad;
            float distance = Vector3.Distance(fireballList[i].landingPosition, transform.position);

            float horizontalVelocity = distance / (Mathf.Sin(2 * angle) / Physics.gravity.y);
            float verticalVelocity = Mathf.Sqrt(Mathf.Abs(Physics.gravity.y) * distance / Mathf.Sin(2 * angle));

            Vector3 velocity = new Vector3(0, verticalVelocity, horizontalVelocity);
            fireballList[i].fireball.GetComponent<Rigidbody>().velocity = fireballList[i].fireball.TransformDirection(velocity);

        }
        */
    }
}
