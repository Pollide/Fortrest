using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoData : MonoBehaviour
{
    public Text titleText;
    public Image currentFill;
    public Image upgradeFill;

    public void InfoRefresh(float current, float change, float max)
    {
        gameObject.SetActive(true);
        currentFill.fillAmount = current / max;
        upgradeFill.fillAmount = (current + change) / max;
    }
}
