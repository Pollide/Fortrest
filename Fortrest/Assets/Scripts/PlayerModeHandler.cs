using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public enum PlayerModes
{
    BuildMode,
    ResourceMode,
    CombatMode,
    Paused,
}

public enum BuildType
{
    Turret,
    Cannon,
    Slow,
    Scatter,
    None,
}

public class PlayerModeHandler : MonoBehaviour
{
    public static PlayerModeHandler global;
    public PlayerModes playerModes;
    private PlayerModes lastMode;
    public BuildType buildType;
    public GameObject[] turretPrefabs;
    [HideInInspector]
    public GameObject turretBlueprint;
    Transform KeyHint;
    public Material turretBlueprintRed;
    public Material turretBlueprintBlue;
    //public Vector2 distanceAwayFromPlayerX;
    //public Vector2 distanceAwayFromPlayerZ;
    public LayerMask buildingLayer;
    public Grid buildGrid;

    public GameObject House;
    public GameObject selectionGrid;
    GameObject newSelectionGrid;
    public GameObject KeyBlueprintHintPrefab;

    public bool inTheFortress;
    private bool centerMouse;
    Vector2 cursorPosition;
    Vector3 entryPosition;

    public bool canInteractWithHouse;

    private float minDistanceBetweenTurrets = 1.0f;

    bool runOnce;

    public Vector3 HintOffset;

    public Building SelectedTurret;
    public bool hoveringTurret;
    public bool[,] occupied; //this isnt in use anymore, i know its big brain and stuff but it works without it and was causing errors with grid index
    private bool cantPlace;
    public bool buildingWithController;
    bool turretMenuOpened;
    Vector3 defaultPosition;
    public GameObject upgradeTimerPrefab;

    private void Awake()
    {
        if (global)
        {
            //destroys the duplicate
            Destroy(gameObject);
            //  GrassComputeScript.global.interactors.Remove(GetComponentInChildren<ShaderInteractor>());
        }
        else
        {
            //itself doesnt exist so set it
            global = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        buildType = BuildType.Turret;
        SwitchToBuildMode(false);
        SwitchToResourceMode();
        entryPosition = PlayerController.global.transform.position;
        defaultPosition = entryPosition;
        occupied = new bool[20, 20];
    }

    private void Update()
    {
        if (PlayerController.global.pausedBool || PlayerController.global.mapBool)
        {
            return;
        }

        inTheFortress = playerModes == PlayerModes.BuildMode;

        if (inTheFortress)
        {
            if (Weather.global.gameObject.activeSelf)
            {
                Weather.global.gameObject.SetActive(false);
            }
            if (!centerMouse)
            {
                Mouse.current.WarpCursorPosition(new Vector2(Screen.width / 2, Screen.height / 2));
                cursorPosition = new Vector2(Screen.width / 2, Screen.height / 2);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                centerMouse = true;
            }
            if (GameManager.global.moveCTRL.x != 0 || GameManager.global.moveCTRL.y != 0)
            {
                buildingWithController = true;
                cursorPosition += GameManager.global.moveCTRL * 4.0f;
                Mouse.current.WarpCursorPosition(cursorPosition);
            }
            else
            {
                buildingWithController = false;
            }
            if (GameManager.global.KeyboardBool)
            {
                cursorPosition = Input.mousePosition;
            }

            BuildMode();

            ScrollSwitchTurret();
        }
        else
        {
            buildingWithController = false;
            if (!Weather.global.gameObject.activeSelf)
            {
                Weather.global.gameObject.SetActive(true);
            }
            if ((Input.GetKeyDown(KeyCode.Q) || PlayerController.global.swapCTRL) && PlayerController.global.playerCanMove)
            {
                PlayerController.global.swapCTRL = false;
                GameManager.global.SoundManager.PlaySound(GameManager.global.ModeChangeClickSound);
                switch (playerModes)
                {
                    case PlayerModes.CombatMode:
                        PlayerController.global.canShoot = false;
                        PlayerController.global.characterAnimator.SetBool("Aiming", false);
                        PlayerController.global.lunge = false;
                        SwitchToResourceMode();
                        break;
                    case PlayerModes.ResourceMode:
                        SwitchToCombatMode();
                        break;
                    default:
                        break;
                }
            }
        }

        if (House.GetComponent<Building>().playerinRange && !Boar.global.mounted && (Boar.global.closerToHouse || (!Boar.global.closerToHouse && !Boar.global.inRange)) && !PlayerController.global.playerDead && !PlayerController.global.teleporting && !PlayerController.global.respawning)
        {
            canInteractWithHouse = true;
            PlayerController.global.needInteraction = true;
        }
        else
        {
            canInteractWithHouse = false;
            if (!Boar.global.canInteractWithBoar && !PlayerController.global.canTeleport && PlayerController.global.playerRespawned && !PlayerController.global.bridgeInteract)
            {
                PlayerController.global.needInteraction = false;
            }
        }

        bool walkedthroughdoor = Vector3.Distance(PlayerController.global.transform.position, House.transform.position - House.transform.forward * 3) < 3f && !inTheFortress;

        if ((Input.GetKeyDown(KeyCode.E) || PlayerController.global.interactCTRL || walkedthroughdoor) && canInteractWithHouse)
        {
            PlayerController.global.interactCTRL = false;

            if (!inTheFortress)
            {
                entryPosition = walkedthroughdoor ? defaultPosition : PlayerController.global.transform.position;
                SwitchToBuildMode();
            }
            else
            {
                CameraMovement.global.ResetAll();
                GameManager.global.SoundManager.PlaySound(GameManager.global.ExitHouseSound);
                if (!House.GetComponent<Building>().textDisplayed)
                {
                    LevelManager.FloatingTextChange(House.GetComponent<Building>().interactText.gameObject, true);
                    House.GetComponent<Building>().textDisplayed = true;
                }

                ExitHouseCleanUp();
            }
        }
    }

    public bool MouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public void TurretMenuSet(bool open)
    {
        if (open != turretMenuOpened)
        {
            turretMenuOpened = open;

            if (!open)
            {
                SelectedTurret = null;

                if (buildGrid.gameObject.activeSelf)
                    PlayerController.global.UpdateResourceHolder(new PlayerController.ResourceData() { buildType = buildType });
            }
            Debug.Log("OPEN: " + open);
            GameManager.PlayAnimation(PlayerController.global.UIAnimation, "TurretMenuUI", open);
        }

    }



    public bool SetTeir(Image fillImage, ref float buttonTier, ref float defenceTier, bool upgrade, bool active)
    {
        int tier = SelectedTurret.GetComponent<Defence>().CurrentTier;
        float max = buttonTier * 5;
        bool next = tier > 0;
        float shift = (next ? max : 0);


        bool ReturnMax(ref float defenceTier)
        {
            return (defenceTier - shift) >= max;
        }

        fillImage.GetComponentInParent<TurretStats>(true).gameObject.SetActive(active);

        if (buttonTier != 0 && active) //active in hiarachy as some tiers are hidden depending on the turret
        {

            if (upgrade && !ReturnMax(ref defenceTier) && PlayerController.global.CheckSufficientResources())
            {
                defenceTier += buttonTier;
                GameManager.global.SoundManager.PlaySound(GameManager.global.UpgradeMenuClickSound);
                GameManager.PlayAnimation(fillImage.GetComponentInParent<Animation>(), "TierUpgrade");
                PlayerController.global.UpdateResourceHolder(PlayerController.global.previousResourceData); //so the bars update with the new defenceTier addition

            }

            fillImage.fillAmount = ((float)defenceTier - shift) / max;
            fillImage.color = next ? Color.magenta : Color.cyan;


            return ReturnMax(ref defenceTier);

        }
        else
        {
            return true;
        }
    }

    void TierChange(bool instant)
    {
        Defence defence = SelectedTurret.GetComponent<Defence>();

        PlayerController.global.turretImageIcon.sprite = defence.spriteTierList[Mathf.Clamp(defence.CurrentTier, 0, defence.spriteTierList.Count - 1)];
        PlayerController.global.turretMenuTitle.text = SelectedTurret.buildingObject.ToString();
        PlayerController.global.turretTierOne.SetActive(false);
        PlayerController.global.turretTierTwo.SetActive(false);
        PlayerController.global.turretBoarderImage.color = Color.white;
        //  GameManager.PlayAnimation(PlayerController.global.UIAnimation, "TurretTierTwo", false, true);

        if (defence.CurrentTier == 1)
        {
            GameManager.PlayAnimation(PlayerController.global.UIAnimation, "TurretTierOne", true, instant);
        }
        if (defence.CurrentTier >= 2)
        {
            GameManager.PlayAnimation(PlayerController.global.UIAnimation, "TurretTierTwo", true, instant);
        }

    }

    public void UpdateTier(TurretStats buttonStat = null)
    {
        if (PlayerModeHandler.global.SelectedTurret)
        {
            List<TurretStats> turretStats = GameManager.FindComponent<TurretStats>(PlayerController.global.turretMenuHolder.transform);
            Defence defence = SelectedTurret.GetComponent<Defence>();

            int complete = 0;

            for (int i = 0; i < turretStats.Count; i++)
            {
                bool upgrade = turretStats[i] == buttonStat;

                bool ballista = SelectedTurret.buildingObject == Building.BuildingType.Ballista;
                bool cannon = SelectedTurret.buildingObject == Building.BuildingType.Cannon;
                bool slow = SelectedTurret.buildingObject == Building.BuildingType.Slow;
                bool scatter = SelectedTurret.buildingObject == Building.BuildingType.Scatter;

                bool damage = SetTeir(turretStats[i].fillImage, ref turretStats[i].changeTier.damageTier, ref defence.changeTier.damageTier, upgrade, ballista || cannon || scatter);
                bool health = SetTeir(turretStats[i].fillImage, ref turretStats[i].changeTier.healthTier, ref defence.changeTier.healthTier, upgrade, cannon || slow || scatter);
                bool range = SetTeir(turretStats[i].fillImage, ref turretStats[i].changeTier.rangeTier, ref defence.changeTier.rangeTier, upgrade, ballista || cannon || slow);
                bool rate = SetTeir(turretStats[i].fillImage, ref turretStats[i].changeTier.rateTier, ref defence.changeTier.rateTier, upgrade, ballista || slow || scatter);


                if (damage && range && rate && health)
                {
                    complete++;
                }
            }

            if (complete == turretStats.Count && defence.CurrentTier < 2)
            {
                defence.CurrentTier++;
                defence.ReturnAnimator();

                TierChange(false);
                GameManager.global.SoundManager.PlaySound(GameManager.global.UpgradeMenuClickSound);
                UpdateTier(); //updates fill
            }
        }

    }

    public GameObject ReturnTurretPrefab()
    {
        GameObject turretPrefab = turretPrefabs[0];

        if (buildType == BuildType.Slow)
        {
            turretPrefab = turretPrefabs[1];
        }
        else if (buildType == BuildType.Cannon)
        {
            turretPrefab = turretPrefabs[2];
        }
        else if (buildType == BuildType.Scatter)
        {
            turretPrefab = turretPrefabs[3];
        }
        return turretPrefab;
    }

    private void BuildMode()
    {
        Ray ray = LevelManager.global.SceneCamera.ScreenPointToRay(cursorPosition);

        bool placing = Physics.Raycast(ray, out RaycastHit hitData, Mathf.Infinity, GameManager.ReturnBitShift(new string[] { "Grid" })) && !MouseOverUI();



        if (!turretMenuOpened && placing)
        {
            GameObject turretPrefab = ReturnTurretPrefab();

            Vector3 worldPos = hitData.point;
            Vector3 gridPos = buildGrid.GetCellCenterWorld(buildGrid.WorldToCell(worldPos));
            worldPos = new Vector3(gridPos.x, 0, gridPos.z);
            Vector2 gridNumber = new Vector2();
            gridNumber.x = (worldPos.x) / 4.0f;
            gridNumber.y = (worldPos.z) / 4.0f;

            Collider[] colliders = Physics.OverlapSphere(worldPos, minDistanceBetweenTurrets, GameManager.ReturnBitShift(new string[] { "Building", "Resource", "Boar" }));

            cantPlace = false;
            hoveringTurret = false;

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].tag == "Turret")
                {
                    hoveringTurret = true;

                    if (colliders[i].GetComponent<Defence>() && colliders[i].GetComponent<Defence>().enabled) //finished building
                    {
                        bool enter = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || GameManager.global.selectCTRL;

                        Building building = colliders[i].GetComponentInParent<Building>();

                        if (!SelectedTurret)
                        {
                            if (enter)
                            {
                                ClearBlueprint();
                                Debug.Log("GO");
                                PlayerController.global.turretMenuHolder.GetChild(0).position = LevelManager.global.SceneCamera.WorldToScreenPoint(hitData.point);

                                SelectedTurret = building;

                                TierChange(true);

                                UpdateTier();
                                TurretMenuSet(true);
                            }

                        }
                        else if (SelectedTurret != building)
                        {
                            SelectedTurret = null;
                        }


                        return;
                    }
                }
                if (colliders[i].tag == "Resource" || colliders[i].tag == "BoarTurret")
                {
                    cantPlace = true;
                }
            }
            SelectedTurret = null;

            if (!turretBlueprint)
            {
                turretBlueprint = Instantiate(turretPrefab);


                //turretBlueprint.GetComponent<Building>().resourceObject = Building.BuildingType.DefenseBP;
                turretBlueprint.GetComponent<Building>().enabled = false;
                turretBlueprint.GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = false;
                turretBlueprint.tag = "BuildingBP";
                turretBlueprint.layer = LayerMask.NameToLayer("BuildingBP");
                //NOT IN USE RIGHT NOW but basically it would hover over the building to say to place
                //KeyHint = Instantiate(KeyBlueprintHintPrefab).transform;

                turretBlueprint.GetComponent<Defence>().enabled = false;

            }

            turretBlueprint.transform.position = worldPos;

            if (KeyHint)
                KeyHint.position = worldPos + HintOffset;

            bool selectBool = Input.GetMouseButtonDown(0) || PlayerController.global.selectCTRL;

            if (selectBool)
            {
                PlayerController.global.selectCTRL = false;
            }
            else
            {
                runOnce = false;
            }

            if (!hoveringTurret && !cantPlace) // && sufficient false just so it turns red
            {
                BluePrintSet(PlayerController.global.CheckSufficientResources(false) ? turretBlueprintBlue : turretBlueprintRed);

                if (selectBool && PlayerController.global.CheckSufficientResources()) //but here is where you purchase
                {
                    float timer;
                    switch (buildType)
                    {
                        case BuildType.Turret:
                            timer = 10.0f;
                            break;
                        case BuildType.Cannon:
                            timer = 15.0f;
                            break;
                        case BuildType.Slow:
                            timer = 20.0f;
                            break;
                        case BuildType.Scatter:
                            timer = 10.0f;
                            break;
                        default:
                            timer = 0f;
                            break;
                    }
                    //occupied[(int)gridNumber.x, (int)gridNumber.y] = true;
                    StartCoroutine(TurretConstructing(timer, turretPrefab, worldPos, new Vector2((int)gridNumber.x, (int)gridNumber.y)));
                }
            }
            else
            {
                BluePrintSet(turretBlueprintRed);

                if (selectBool)
                {
                    if (!runOnce)
                    {
                        GameManager.global.SoundManager.PlaySound(GameManager.global.CantPlaceSound);
                        runOnce = true;
                    }
                }
            }
        }
    }

    public GameObject ReturnVFXBuilding(Transform turret, float destroy = 3)
    {
        GameManager.global.SoundManager.PlaySound(GameManager.global.TurretConstructingSound);
        GameObject tempVFX1 = Instantiate(LevelManager.global.VFXBuilding.gameObject, turret.position + new Vector3(0f, 0.7f, 0f), Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f)));
        tempVFX1.GetComponent<VisualEffect>().Play();
        Destroy(tempVFX1, destroy);

        return tempVFX1;
    }

    private IEnumerator TurretConstructing(float turretTimer, GameObject prefab, Vector3 position, Vector2 gridPos)
    {
        GameObject newTurret = Instantiate(prefab, position, Quaternion.identity);

        newTurret.transform.localScale = Vector3.zero;
        newTurret.GetComponent<Building>().gridLocation = gridPos;

        ReturnVFXBuilding(newTurret.transform, turretTimer);


        newTurret.GetComponent<Defence>().enabled = false;
        GameObject upgradeTimer = Instantiate(upgradeTimerPrefab, position, Quaternion.identity);
        Text upgradeText = upgradeTimer.transform.GetChild(0).GetComponent<Text>();
        float start = turretTimer;

        while (turretTimer > 0)
        {
            turretTimer -= Time.deltaTime;
            newTurret.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, start - turretTimer);
            upgradeText.text = turretTimer.ToString("N2") + "s";
            yield return null;
        }
        upgradeText.text = "DONE!";

        Destroy(upgradeTimer, GameManager.PlayAnimation(upgradeTimer.GetComponent<Animation>(), "Upgrade Timer Appear", false).length);

        LevelManager.global.AddBuildingVoid(newTurret.transform);
        newTurret.GetComponent<Defence>().enabled = true;

        if (prefab == turretPrefabs[0])
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.BallistaSpawnedSound, 1.0f, true, 0, false, inTheFortress ? null : newTurret.transform);
        }
        else if (prefab == turretPrefabs[1])
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.SlowSpawnedSound, 1.0f, true, 0, false, inTheFortress ? null : newTurret.transform);
        }
        else if (prefab == turretPrefabs[2])
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.CannonSpawnedSound, 1.0f, true, 0, false, inTheFortress ? null : newTurret.transform);
        }
        else if (prefab == turretPrefabs[3])
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.ScatterSpawnedSound, 1.0f, true, 0, false, inTheFortress ? null : newTurret.transform);
        }

        GameObject tempVFX2 = Instantiate(LevelManager.global.VFXSmokeRing.gameObject, newTurret.transform.position + new Vector3(0, .5f, 0), Quaternion.identity);
        tempVFX2.GetComponent<VisualEffect>().Play();
        Destroy(tempVFX2, 2.0f);
        //!hitData.transform.CompareTag("Player") && !hitData.transform.CompareTag("Building") && !hitData.transform.CompareTag("Resource")        
    }
    BuildType oldbuildType;
    private void ScrollSwitchTurret()
    {
        if ((Input.mouseScrollDelta.y > 0f || PlayerController.global.scrollCTRL) && playerModes == PlayerModes.BuildMode)
        {
            PlayerController.global.scrollCTRL = false;
            GameManager.global.SoundManager.StopSelectedSound(GameManager.global.SwapTurretSound);
            GameManager.global.SoundManager.PlaySound(GameManager.global.SwapTurretSound);
            if (buildType == BuildType.Turret)
            {
                SwitchBuildTypeCannon();
            }
            else if (buildType == BuildType.Cannon)
            {
                SwitchBuildTypeSlow();
            }
            else if (buildType == BuildType.Slow)
            {
                SwitchBuildTypeScatter();
            }
            else if (buildType == BuildType.Scatter)
            {
                SwitchBuildTypeTurret();
            }
        }
        if ((Input.mouseScrollDelta.y < 0f || PlayerController.global.scrollCTRL) && playerModes == PlayerModes.BuildMode)
        {
            PlayerController.global.scrollCTRL = false;
            GameManager.global.SoundManager.PlaySound(GameManager.global.SwapTurretSound);
            if (buildType == BuildType.Turret)
            {
                SwitchBuildTypeScatter();
            }
            else if (buildType == BuildType.Cannon)
            {
                SwitchBuildTypeTurret();
            }
            else if (buildType == BuildType.Slow)
            {
                SwitchBuildTypeCannon();
            }
            else if (buildType == BuildType.Scatter)
            {
                SwitchBuildTypeSlow();
            }
        }

        if (oldbuildType != buildType)
        {
            oldbuildType = buildType;
            PlayerController.global.UpdateResourceHolder(new PlayerController.ResourceData() { buildType = buildType });
        }
    }

    IEnumerator PlayerAwake()
    {
        yield return new WaitForSeconds(0.1f);
        PlayerController.global.playerCanMove = true;
    }

    void ClearBlueprint()
    {
        if (turretBlueprint)
        {
            Destroy(turretBlueprint);

            if (KeyHint)
                Destroy(KeyHint.gameObject);
        }
        TurretMenuSet(false);
    }

    void ClearSelectionGrid()
    {
        if (newSelectionGrid)
        {
            Destroy(newSelectionGrid);
        }
    }

    public void ExitHouseCleanUp()
    {
        ModeSwitchText.global.ResetText();
        ClearSelectionGrid();
        ClearBlueprint();
        SwitchToBuildMode(false);
        StartCoroutine(PlayerAwake());
        PlayerController.global.TeleportPlayer(entryPosition, true);
        if (lastMode == PlayerModes.ResourceMode)
        {
            SwitchToResourceMode();
        }
        else if (lastMode == PlayerModes.CombatMode)
        {
            SwitchToCombatMode();
        }
    }

    public void SwitchToBuildMode(bool active = true)
    {
        buildGrid.gameObject.SetActive(active);
        PlayerController.global.characterAnimator.gameObject.SetActive(!active);
        if (active)
        {
            PlayerController.global.evading = false;
            lastMode = playerModes;
            GameManager.global.SoundManager.PlaySound(GameManager.global.EnterHouseSound);
            if (House.GetComponent<Building>().textDisplayed)
            {
                LevelManager.FloatingTextChange(House.GetComponent<Building>().interactText.gameObject, false);
                House.GetComponent<Building>().textDisplayed = false;
            }
            centerMouse = false;

            ModeSwitchText.global.ResetText();
            ClearSelectionGrid();
            PlayerController.global.TeleportPlayer(PlayerController.global.house.transform.position, true);
            PlayerController.global.playerCanMove = false;

            playerModes = PlayerModes.BuildMode;
            PlayerController.global.ChangeTool(new PlayerController.ToolData() { HammerBool = true });
        }
        else
        {
            TurretMenuSet(false);
        }

        PlayerController.global.UpdateResourceHolder(new PlayerController.ResourceData() { buildType = buildType }, open: active);
    }

    public void SwitchToResourceMode()
    {
        PlayerController.global.ChangeTool(new PlayerController.ToolData() { AxeBool = PlayerController.global.lastWasAxe, PickaxeBool = !PlayerController.global.lastWasAxe });
        playerModes = PlayerModes.ResourceMode;
    }

    public void SwitchToCombatMode()
    {
        PlayerController.global.ChangeTool(new PlayerController.ToolData() { SwordBool = true });
        playerModes = PlayerModes.CombatMode;

    }


    void BluePrintSet(Material colorMaterial)
    {
        List<Renderer> meshRenderers = GameManager.FindComponent<Renderer>(turretBlueprint.transform);

        for (int i = 0; i < meshRenderers.Count; i++)
        {
            meshRenderers[i].material = colorMaterial;
        }
    }

    public static void SetMouseActive(bool isActive, bool buildMode)
    {
        //  Debug.Log("Cursor " + isActive);

        if (isActive && GameManager.global.KeyboardBool)
        {
            if (!Cursor.visible)
            {
                Cursor.visible = true;

                Cursor.lockState = CursorLockMode.None;
            }

        }
        else
        {
            if (Cursor.visible)
            {
                if (!buildMode)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }
    }

    public void SwitchBuildTypeSlow()
    {
        buildType = BuildType.Slow;
        ClearBlueprint();
    }

    public void SwitchBuildTypeCannon()
    {
        buildType = BuildType.Cannon;
        ClearBlueprint();
    }

    public void SwitchBuildTypeTurret()
    {
        buildType = BuildType.Turret;
        ClearBlueprint();
    }

    public void SwitchBuildTypeScatter()
    {
        buildType = BuildType.Scatter;
        ClearBlueprint();
    }
}