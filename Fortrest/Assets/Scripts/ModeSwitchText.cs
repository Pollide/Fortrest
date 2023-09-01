using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModeSwitchText : MonoBehaviour
{
    public static ModeSwitchText global;

    [SerializeField] private float upTimeMax = 1;
    public float upTime = 0;
    public bool isActive = false;

    private void Awake()
    {
        global = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TMP_Text>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            GetComponent<TMP_Text>().enabled = true;

            upTime += Time.deltaTime;

            if (upTime >= upTimeMax)
            {
                isActive = false;
                upTime = 0;
            }
        }
        else
        {
            GetComponent<TMP_Text>().enabled = false;
        }

        switch (PlayerModeHandler.global.playerModes)
        {
            case PlayerModes.BuildMode:
                GetComponent<TMP_Text>().text = "Build Mode";
                break;
            case PlayerModes.ResourceMode:
                GetComponent<TMP_Text>().text = "Resource Mode";
                break;
            case PlayerModes.CombatMode:
                GetComponent<TMP_Text>().text = "Combat Mode";
                break;
            case PlayerModes.Paused:
                GetComponent<TMP_Text>().text = "Paused Mode";
                break;
            default:
                break;
        }
    }

    public void ResetText()
    {
        isActive = true;
        upTime = 0;
    }
}
