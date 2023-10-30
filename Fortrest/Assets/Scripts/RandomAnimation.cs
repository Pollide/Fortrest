using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.ChangeAnimationLayers(GetComponent<Animation>(), true);

        if (Random.Range(0, 2) == 0)
        {
            GameManager.PlayAnimation(GetComponent<Animation>(), "Birds Rotate", false);
        }
    }
}
