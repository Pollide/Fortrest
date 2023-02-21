using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public static Menu global;
    public GameObject OptionsCanvas;
    public GameObject MenuCanvas;

    private void Awake()
    {
        global = this;
    }
}
