using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeGlobalArenaChief : MonoBehaviour
{

    public static MakeGlobalArenaChief global;

    // Start is called before the first frame update
    void Start()
    {
        global = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
