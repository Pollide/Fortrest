using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveMaterial : MonoBehaviour
{
    // Start is called before the first frame update
    public List<SkinnedMeshRenderer> skinnedMeshRendererList = new List<SkinnedMeshRenderer>();
    float disolvingTimer = 1;
    void Start()
    {
        skinnedMeshRendererList = GameManager.FindComponent<SkinnedMeshRenderer>(transform);

        for (int i = skinnedMeshRendererList.Count - 1; i >= 0; i--)
        {
            if (skinnedMeshRendererList[i].material.HasProperty("_DissolveAmount"))
                skinnedMeshRendererList[i].material = new Material(skinnedMeshRendererList[i].material);
            else
            {
                skinnedMeshRendererList.RemoveAt(i);
            }
        }
    }

    // Update is called once per frame 
    void Update()
    {

        if (disolvingTimer > 0)
        {
            disolvingTimer -= (GetComponent<BossSpawner>() ? 0.2f : 0.5f) * Time.deltaTime;
        }
        else
        {
            disolvingTimer = 0;
            enabled = false;

            if (GetComponent<EnemyController>())
            {
                Destroy(gameObject);
            }
        }

        for (int i = 0; i < skinnedMeshRendererList.Count; i++)
        {
            skinnedMeshRendererList[i].material.SetFloat("_DissolveAmount", disolvingTimer);
        }
    }
}
