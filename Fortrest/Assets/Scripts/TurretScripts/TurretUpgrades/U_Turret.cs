using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class U_Turret : MonoBehaviour
{
    // Reference to shooting script 
    private TurretShooting turretScript;
    // Reference to player transform 
    private Transform playerTransform;

    [Header("Button Objects")]
    public List<GameObject> attackSpeedButtons = new();
    public List<GameObject> damageButtons = new();
    public List<GameObject> rangeButtons = new();
    public List<GameObject> allButtons = new();

    public GameObject buyUpgradeButton;
    public GameObject upgradeMenu;
    public GameObject addKnockBackButton;
    public GameObject addChanceToKillButton;
    public GameObject addChanceToMultiShotButton;

    private GameObject currentButton = null;

    [Header("Text Objects")]
    // Reference to text
    public TMP_Text upgradeTextObject;
    public TMP_Text uiText;

    [Header("Range")]
    // The range to start
    public float detectionRange = 10f;

    [Header("Upgrade cost")]
    public int upgradeCostWood = 1;
    public int upgradeCostStone = 1;

    [Header("Initial Upgrade percentage")]
    public float upgradeASPercent = 5f;
    public float upgradeRangePercent = 5f;
    public float upgradeDamagePercent = 5f;

    public float instantKillPercent = 5f;
    public float knockBackPercentage = 5f;
    public float multiShotPercentage = 5f;

    [Header("This scales each subsequent upgrade")]
    public float upgradeRankScaleRange = 1f;
    public float upgradeRankScaleDMG = 1f;
    public float upgradeRankScaleAS = 1f;

    [Header("Special Upgrade Boolean")]
    public bool isInstantKillPercent = false;
    public bool isKnockBackActive = false;
    public bool isMultiShotActive = false;
    public bool specialActive = false;
    public bool canUpgrade = false;

    private void Awake()
    {
        // Get shooting script
        turretScript = GetComponent<TurretShooting>();
        // Get player transform
        playerTransform = PlayerController.global.transform;

        for (int i = 0; i < attackSpeedButtons.Count; i++)
        {
            allButtons.Add(attackSpeedButtons[i]);
        }
        for (int i = 0; i < damageButtons.Count; i++)
        {
            allButtons.Add(damageButtons[i]);
        }
        for (int i = 0; i < rangeButtons.Count; i++)
        {
            allButtons.Add(rangeButtons[i]);
        }

        allButtons.Add(addChanceToKillButton);
        allButtons.Add(addChanceToMultiShotButton);
        allButtons.Add(addKnockBackButton);
    }

    private void Start()
    {
        uiText.text = "";

        upgradeTextObject.enabled = false;
        upgradeTextObject.text = "F";
        upgradeMenu.SetActive(false);

        addKnockBackButton.GetComponent<Button>().interactable = false;
        addChanceToKillButton.GetComponent<Button>().interactable = false;
        addChanceToMultiShotButton.GetComponent<Button>().interactable = false;
        buyUpgradeButton.GetComponent<Button>().interactable = false;
    }

    public void AddAttackSpeed()
    {
        turretScript.fireRate += turretScript.fireRate * (upgradeASPercent / 100f);
    }

    public void AddRange()
    {
        turretScript.shootingRange += turretScript.shootingRange * (upgradeRangePercent / 100f);
    }

    public void AddDamage()
    {
        turretScript.damage += turretScript.damage * (upgradeDamagePercent / 100f);
    }

    public bool IsOpen()
    {
        return upgradeMenu.activeSelf;
    }

    public void RunUpgrade()
    {
        currentButton.GetComponent<Button>().interactable = false;

        InventoryManager.global.RemoveItem("Wood", upgradeCostWood);
        InventoryManager.global.RemoveItem("Stone", upgradeCostStone);

        upgradeCostWood += upgradeCostWood * 2;
        upgradeCostStone += upgradeCostStone * 2;

        if (attackSpeedButtons.Find(button => button.name == currentButton.name))
        {
            AddAttackSpeed();
            upgradeASPercent += upgradeASPercent * upgradeRankScaleAS;

            if (AreAllBaseButtonsInactive(rangeButtons) && AreAllBaseButtonsInactive(damageButtons) && AreAllBaseButtonsInactive(attackSpeedButtons))
            {
                addKnockBackButton.GetComponent<Button>().interactable = true;
                addChanceToKillButton.GetComponent<Button>().interactable = true;
                addChanceToMultiShotButton.GetComponent<Button>().interactable = true;
            }
        }
        else if (rangeButtons.Find(button => button.name == currentButton.name))
        {
            AddRange();
            upgradeRangePercent += upgradeRangePercent * upgradeRankScaleRange;

            if (AreAllBaseButtonsInactive(rangeButtons) && AreAllBaseButtonsInactive(damageButtons) && AreAllBaseButtonsInactive(attackSpeedButtons))
            {
                addKnockBackButton.GetComponent<Button>().interactable = true;
                addChanceToKillButton.GetComponent<Button>().interactable = true;
                addChanceToMultiShotButton.GetComponent<Button>().interactable = true;
            }
        }
        else if (damageButtons.Find(button => button.name == currentButton.name))
        {
            AddDamage();
            upgradeDamagePercent += upgradeDamagePercent * upgradeRankScaleDMG;

            if (AreAllBaseButtonsInactive(rangeButtons) && AreAllBaseButtonsInactive(damageButtons) && AreAllBaseButtonsInactive(attackSpeedButtons))
            {
                addKnockBackButton.GetComponent<Button>().interactable = true;
                addChanceToKillButton.GetComponent<Button>().interactable = true;
                addChanceToMultiShotButton.GetComponent<Button>().interactable = true;
            }
        }
        else if (addChanceToKillButton.name == currentButton.name)
        {
            isInstantKillPercent = true;

            ColorBlock cb = addChanceToKillButton.GetComponent<Button>().colors;
            cb.disabledColor = Color.green;
            addChanceToKillButton.GetComponent<Button>().colors = cb;

            specialActive = true;
        }
        else if (addChanceToMultiShotButton.name == currentButton.name)
        {
            isMultiShotActive = true;

            ColorBlock cb = addChanceToMultiShotButton.GetComponent<Button>().colors;
            cb.disabledColor = Color.green;
            addChanceToMultiShotButton.GetComponent<Button>().colors = cb;

            specialActive = true;
        }
        else if (addKnockBackButton.name == currentButton.name)
        {
            isKnockBackActive = true;

            ColorBlock cb = addKnockBackButton.GetComponent<Button>().colors;
            cb.disabledColor = Color.green;
            addKnockBackButton.GetComponent<Button>().colors = cb;

            specialActive = true;
        }

        if (specialActive)
        {
            addKnockBackButton.GetComponent<Button>().interactable = false;
            addChanceToKillButton.GetComponent<Button>().interactable = false;
            addChanceToMultiShotButton.GetComponent<Button>().interactable = false;
        }

        buyUpgradeButton.GetComponent<Button>().interactable = false;
    }

    public void ChangeUIText(string _text)
    {
        if (currentButton.GetComponent<Image>().color == Color.green)
        {
            if (attackSpeedButtons.Find(button => button.name == currentButton.name))
            {
                uiText.text = _text + upgradeASPercent + "% \nWood: " + upgradeCostWood + "  Stone: " + upgradeCostStone;
            }
            else if (rangeButtons.Find(button => button.name == currentButton.name))
            {
                uiText.text = _text + upgradeRangePercent + "% \nWood: " + upgradeCostWood + "  Stone: " + upgradeCostStone;
            }
            else if (damageButtons.Find(button => button.name == currentButton.name))
            {
                uiText.text = _text + upgradeDamagePercent + "% \nWood: " + upgradeCostWood + "  Stone: " + upgradeCostStone;
            }
            else if (addKnockBackButton == currentButton)
            {
                uiText.text = _text + knockBackPercentage + "% \nWood: " + upgradeCostWood + "  Stone: " + upgradeCostStone;
            }
            else if (addChanceToKillButton == currentButton)
            {
                uiText.text = _text + instantKillPercent + "% \nWood: " + upgradeCostWood + "  Stone: " + upgradeCostStone;
            }
            else if (addChanceToMultiShotButton == currentButton)
            {
                uiText.text = _text + multiShotPercentage + "% \nWood: " + upgradeCostWood + "  Stone: " + upgradeCostStone;
            }
        }

        else if (currentButton.GetComponent<Image>().color != Color.green)
        {
            uiText.text = "";
        }

    }

    public void ChangeColor(GameObject _button)
    {
        Image buttonImage = _button.GetComponent<Image>();

        if (buttonImage.color != Color.green)
        {
            buttonImage.color = Color.green;
            currentButton = _button;
            if (upgradeCostStone <= InventoryManager.global.GetItemQuantity("Stone") && upgradeCostWood <= InventoryManager.global.GetItemQuantity("Wood"))
            {
                buyUpgradeButton.GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            buttonImage.color = Color.white;
            buyUpgradeButton.GetComponent<Button>().interactable = false;
        }

        foreach (GameObject button in allButtons)
        {
            if (button.GetComponent<Button>().interactable && button != currentButton)
            {
                button.GetComponent<Image>().color = Color.white;
            }
        }
    }

    bool AreAllBaseButtonsInactive(List<GameObject> _buttonList)
    {
        foreach (GameObject button in _buttonList)
        {
            if (button.GetComponent<Button>().interactable)
            {
                return false;
            }
        }
        return true;
    }
}
