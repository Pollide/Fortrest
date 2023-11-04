using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveMaterial : MonoBehaviour
{
    // Start is called before the first frame update
    public List<SkinnedMeshRenderer> skinnedMeshRendererList = new List<SkinnedMeshRenderer>();
    public float disolvingTimer = 1;
    void Start()
    {
        skinnedMeshRendererList = GameManager.FindComponent<SkinnedMeshRenderer>(transform);

        for (int i = skinnedMeshRendererList.Count - 1; i >= 0; i--)
        {
            skinnedMeshRendererList[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            for (int j = 0; j < skinnedMeshRendererList[i].materials.Length; j++)
            {
                if (skinnedMeshRendererList[i].materials[j].HasProperty("_DissolveAmount"))
                {
                    skinnedMeshRendererList[i].gameObject.layer = 0;
                    skinnedMeshRendererList[i].materials[j] = new Material(skinnedMeshRendererList[i].materials[j]);
                }
                else
                {
                    skinnedMeshRendererList.RemoveAt(i);
                }
            }
        }
    }

    // Update is called once per frame 
    void Update()
    {

        if (disolvingTimer > 0)
        {
            disolvingTimer -= 0.5f * Time.deltaTime;
        }
        else
        {
            disolvingTimer = 0;
            enabled = false;

            if (!GetComponent<BossSpawner>())
            {
                Destroy(gameObject);
            }
        }

        for (int i = 0; i < skinnedMeshRendererList.Count; i++)
        {
            for (int j = 0; j < skinnedMeshRendererList[i].materials.Length; j++)
            {
                skinnedMeshRendererList[i].materials[j].SetFloat("_DissolveAmount", disolvingTimer);
            }
        }
    }
}
