using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public GameObject arrowPrefab;
    public GameObject enemy;

    private void LateUpdate()
    {
        Draw();
    }

    private void Draw()
    {
        //for (int i = 0; i < LevelManager.global.enemyList.Count; i++)
        //{
            //Vector3 screenpos = Camera.main.WorldToScreenPoint(LevelManager.global.enemyList[i].transform.position);
        Vector3 screenpos = Camera.main.WorldToScreenPoint(enemy.transform.position);

        if (screenpos.z < 0)
            {
                screenpos *= -1;
            }

            Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;

            screenpos -= screenCenter;

            float angle = Mathf.Atan2(screenpos.y, screenpos.x);
            angle -= 90 * Mathf.Deg2Rad;

            float cos = Mathf.Cos(angle);
            float sin = -Mathf.Sin(angle);

            screenpos = screenCenter + new Vector3(sin * 150, cos * 150, 0);

            float m = cos / sin;

            Vector3 screenBounds = screenCenter * 0.9f;

            if (cos > 0)
            {
                screenpos = new Vector3(screenBounds.y / m, screenBounds.y, 0);
            }
            else
            {
                screenpos = new Vector3(-screenBounds.y / m, -screenBounds.y, 0);
            }

            if (screenpos.x > screenBounds.x)
            {
                screenpos = new Vector3(screenBounds.x, screenBounds.x * m, 0);
            }
            else if (screenpos.x < -screenBounds.x)
            {
                screenpos = new Vector3(-screenBounds.x, -screenBounds.x * m, 0);
            }

            screenpos += screenCenter;

            arrowPrefab.transform.localPosition = screenpos;
            arrowPrefab.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        //}
    }
}
