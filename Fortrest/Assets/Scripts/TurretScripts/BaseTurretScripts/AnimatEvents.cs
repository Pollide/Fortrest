using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatEvents : MonoBehaviour
{
    public GameObject Bolt;

    public void BoltAppear()
    {
        Bolt.SetActive(true);
    }

    public void BoltDisappear()
    {
        Bolt.SetActive(false);
    }

    public void SpawnBolt()
    {
        transform.GetComponentInParent<Defence>().ProjectileEvent();
    }
}
