using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDHandler : MonoBehaviour
{
    public static HUDHandler global;

    [Header("Parent Game Objects")]
    public GameObject toolbarParrent;
    public GameObject subQ;
    public GameObject subR;

    [Header("Main Tool Images")]
    public Image buildingM;
    public Image upgradeM;
    public Image resourceM;
    public Image combatM;
    public Image repairM;

    [Header("Sub Tool Images Q")]
    public Image resourceQ;
    public Image combatQ;
    public Image upgradeQ;
    public Image repairQ;
    public Image buildingQ;

    [Header("Sub Tool Images R")]
    public Image upgradeR;
    public Image repairR;
    public Image buildingR;

    private void Awake()
    {
        global = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        subR.SetActive(false);
    }

    public void BuildModeHUD()
    {
        subR.SetActive(true);

        buildingM.enabled = true;
        upgradeM.enabled = false;
        resourceM.enabled = false;
        combatM.enabled = false;
        repairM.enabled = false;

        resourceQ.enabled = false; 
        combatQ.enabled = false; 
        upgradeQ.enabled = true; 
        repairQ.enabled = false; 
        buildingQ.enabled = false;

        upgradeR.enabled = false;
        repairR.enabled = true;
        buildingR.enabled = false;
    }

    public void ResourceModeHUD()
    {
        subR.SetActive(false);

        buildingM.enabled = false;
        upgradeM.enabled = false;
        resourceM.enabled = true;
        combatM.enabled = false;
        repairM.enabled = false;

        resourceQ.enabled = false;
        combatQ.enabled = true;
        upgradeQ.enabled = false;
        repairQ.enabled = false;
        buildingQ.enabled = false;

        upgradeR.enabled = false;
        repairR.enabled = false;
        buildingR.enabled = false;
    }

    public void CombatModeHUD()
    {
        buildingM.enabled = false;
        upgradeM.enabled = false;
        resourceM.enabled = false;
        combatM.enabled = true;
        repairM.enabled = false;

        resourceQ.enabled = true;
        combatQ.enabled = false;
        upgradeQ.enabled = false;
        repairQ.enabled = false;
        buildingQ.enabled = false;

        upgradeR.enabled = false;
        repairR.enabled = false;
        buildingR.enabled = false;
    }

    public void RepairModeHUD()
    {
        buildingM.enabled = false;
        upgradeM.enabled = false;
        resourceM.enabled = false;
        combatM.enabled = false;
        repairM.enabled = true;

        resourceQ.enabled = false;
        combatQ.enabled = false;
        upgradeQ.enabled = false;
        repairQ.enabled = false;
        buildingQ.enabled = true;

        upgradeR.enabled = true;
        repairR.enabled = false;
        buildingR.enabled = false;
    }

    public void UpgradeModeHUD()
    {
        buildingM.enabled = false;
        upgradeM.enabled = true;
        resourceM.enabled = false;
        combatM.enabled = false;
        repairM.enabled = false;

        resourceQ.enabled = false;
        combatQ.enabled = false;
        upgradeQ.enabled = false;
        repairQ.enabled = true;
        buildingQ.enabled = false;

        upgradeR.enabled = false;
        repairR.enabled = false;
        buildingR.enabled = true;
    }
}
