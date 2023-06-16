using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentToGameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.global)
        {
            transform.SetParent(GameManager.global.transform);
        } 
    }
}
