using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class U_Cannon : MonoBehaviour
{
    // Reference to shooting script 
    private TurretCannon turretScript;
    // Reference to player transform 
    private Transform playerTransform;

    [Header("Button Objects")]
    public List<GameObject> explosianRadiusButtons = new();
    public List<GameObject> damageButtons = new();
    public List<GameObject> rangeButtons = new();
    public List<GameObject> allButtons = new();

    public GameObject buyUpgradeButton;
    public GameObject upgradeMenu;

    public GameObject addHPButton;
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

    [Header("Initial Upgrade percentage")]
    public float upgradeExplosionRadiusPercentage = 5f;
    public float upgradeRangePercent = 5f;
    public float upgradeDamagePercent = 5f;

    public float instantKillPercent = 5f;
    public float increasedHPPercentage = 5f;
    public float multiShotPercentage = 5f;

    [Header("This scales each subsequent upgrade")]
    public float upgradeRankScaleRange = 1f;
    public float upgradeRankScaleDMG = 1f;
    public float upgradeRankScaleExplosionRadius = 1f;

    [Header("Special Upgrade Boolean(Do Not Touch)")]
    public bool isInstantKillPercent = false;
    public bool isHPActive = false;
    public bool isMultiShotActive = false;
    public bool specialActive = false;

    private void Awake()
    {
        // Get shooting script
        turretScript = GetComponent<TurretCannon>();
        // Get player transform
        playerTransform = GameObject.Find("Player").transform;

        for (int i = 0; i < explosianRadiusButtons.Count; i++)
        {
            allButtons.Add(explosianRadiusButtons[i]);
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
        allButtons.Add(addHPButton);
    }

    private void Start()
    {
        uiText.text = "";

        upgradeTextObject.enabled = false;
        upgradeTextObject.text = "F";
        upgradeMenu.SetActive(false);

        addHPButton.GetComponent<Button>().interactable = false;
        addChanceToKillButton.GetComponent<Button>().interactable = false;
        addChanceToMultiShotButton.GetComponent<Button>().interactable = false;
    }

    private void Update()
    {
        // Get the distance from the player transform
        float distanceFromPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceFromPlayer < detectionRange)
        {
            upgradeTextObject.enabled = true;

            if (Input.GetKeyDown(KeyCode.F))
            {
                // Run Upgerade screen
                if (upgradeMenu.activeSelf)
                {
                    upgradeMenu.SetActive(false);
                    PlayerModeHandler.global.SetMouseActive(false);
                    PlayerModeHandler.global.playerModes = PlayerModes.ResourceMode;
                    Time.timeScale = 1f;
                }
                else
                {
                    upgradeMenu.SetActive(true);
                    PlayerModeHandler.global.SetMouseActive(true);
                    PlayerModeHandler.global.playerModes = PlayerModes.UpgradeMenu;
                    Time.timeScale = 0f;
                }
            }
        }
        else
        {
            upgradeTextObject.enabled = false;
        }
    }

    public void AddExplosionRadius()
    {
        turretScript.explosionRadius += turretScript.explosionRadius * (upgradeExplosionRadiusPercentage / 100f);
    }

    public void AddRange()
    {
        turretScript.shootingRange += turretScript.shootingRange * (upgradeRangePercent / 100f);
    }

    public void AddDamage()
    {
        turretScript.damage += turretScript.damage * (upgradeDamagePercent / 100f);
    }

    public void RunUpgrade()
    {
        currentButton.GetComponent<Button>().interactable = false;

        if (explosianRadiusButtons.Find(button => button.name == currentButton.name))
        {
            AddExplosionRadius();
            upgradeExplosionRadiusPercentage += upgradeExplosionRadiusPercentage * upgradeRankScaleExplosionRadius;

            if (AreAllBaseButtonsInactive(rangeButtons) && AreAllBaseButtonsInactive(damageButtons) && AreAllBaseButtonsInactive(explosianRadiusButtons))
            {
                addHPButton.GetComponent<Button>().interactable = true;
                addChanceToKillButton.GetComponent<Button>().interactable = true;
                addChanceToMultiShotButton.GetComponent<Button>().interactable = true;
            }
        }
        else if (rangeButtons.Find(button => button.name == currentButton.name))
        {
            AddRange();
            upgradeRangePercent += upgradeRangePercent * upgradeRankScaleRange;

            if (AreAllBaseButtonsInactive(rangeButtons) && AreAllBaseButtonsInactive(damageButtons) && AreAllBaseButtonsInactive(explosianRadiusButtons))
            {
                addHPButton.GetComponent<Button>().interactable = true;
                addChanceToKillButton.GetComponent<Button>().interactable = true;
                addChanceToMultiShotButton.GetComponent<Button>().interactable = true;
            }
        }
        else if (damageButtons.Find(button => button.name == currentButton.name))
        {
            AddDamage();
            upgradeDamagePercent += upgradeDamagePercent * upgradeRankScaleDMG;

            if (AreAllBaseButtonsInactive(rangeButtons) && AreAllBaseButtonsInactive(damageButtons) && AreAllBaseButtonsInactive(explosianRadiusButtons))
            {
                addHPButton.GetComponent<Button>().interactable = true;
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
        else if (addHPButton.name == currentButton.name)
        {
            isHPActive = true;

            ColorBlock cb = addHPButton.GetComponent<Button>().colors;
            cb.disabledColor = Color.green;
            addHPButton.GetComponent<Button>().colors = cb;

            GetComponent<Building>().maxHealth += GetComponent<Building>().maxHealth * (increasedHPPercentage / 100);

            specialActive = true;
        }

        if (specialActive)
        {
            addHPButton.GetComponent<Button>().interactable = false;
            addChanceToKillButton.GetComponent<Button>().interactable = false;
            addChanceToMultiShotButton.GetComponent<Button>().interactable = false;
        }

        buyUpgradeButton.GetComponent<Button>().interactable = false;
    }

    public void ChangeUIText(string _text)
    {
        if (currentButton.GetComponent<Image>().color == Color.green)
        {
            if (explosianRadiusButtons.Find(button => button.name == currentButton.name))
            {
                uiText.text = _text + upgradeExplosionRadiusPercentage + "%";
            }
            else if (rangeButtons.Find(button => button.name == currentButton.name))
            {
                uiText.text = _text + upgradeRangePercent + "%";
            }
            else if (damageButtons.Find(button => button.name == currentButton.name))
            {
                uiText.text = _text + upgradeDamagePercent + "%";
            }
            else if(addHPButton == currentButton)
            {
                uiText.text = _text + increasedHPPercentage + "%";
            }
            else if (addChanceToKillButton == currentButton)
            {
                uiText.text = _text + instantKillPercent + "%";
            }
            else if (addChanceToMultiShotButton == currentButton)
            {
                uiText.text = _text + multiShotPercentage + "%";
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
            buyUpgradeButton.GetComponent<Button>().interactable = true;
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
