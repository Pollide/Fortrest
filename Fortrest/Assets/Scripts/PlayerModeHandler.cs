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
    GameObject turretBlueprint;
    Transform KeyHint;
    public Material turretBlueprintRed;
    public Material turretBlueprintBlue;
    public Vector2 distanceAwayFromPlayerX;
    public Vector2 distanceAwayFromPlayerZ;
    public LayerMask buildingLayer;
    public Grid buildGrid;

    public GameObject House;
    public GameObject selectionGrid;
    GameObject newSelectionGrid;
    public GameObject KeyBlueprintHintPrefab;
    private HUDHandler HUD;

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
    public bool[,] occupied;
    private bool cantPlace;
    public bool buildingWithController;
    bool turretMenuOpened;
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
        HUD = HUDHandler.global;
        SwitchToBuildMode(false);
        SwitchToResourceMode();
        entryPosition = PlayerController.global.transform.position;
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
                        PlayerController.global.CharacterAnimator.SetBool("Aiming", false);
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

        if ((Input.GetKeyDown(KeyCode.E) || PlayerController.global.interactCTRL) && canInteractWithHouse)
        {
            PlayerController.global.interactCTRL = false;

            if (!inTheFortress)
            {
                SwitchToBuildMode();
            }
            else
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.ExitHouseSound);
                if (!House.GetComponent<Building>().textDisplayed)
                {
                    LevelManager.FloatingTextChange(House.GetComponent<Building>().interactText.gameObject, true);
                    House.GetComponent<Building>().textDisplayed = true;
                }
                PlayerModeHandler.global.TurretMenuSet(false);
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

                if (SelectedTurret)
                    PlayerController.global.UpdateResourceHolder(); //so the turret costs update

                SelectedTurret = null;
            }

            GameManager.PlayAnimation(PlayerController.global.UIAnimation, "TurretMenuUI", open);
        }

    }

    public bool SetTeir(Image fillImage, ref int buttonTier, ref int defenceTier, bool upgrade)
    {
        int tier = SelectedTurret.GetComponent<Defence>().CurrentTier;
        int max = 5;
        bool next = tier > 0;
        int shift = (next ? max : 0);

        if (buttonTier != 0)
        {
            if (upgrade)
            {
                defenceTier += buttonTier;
            }

            fillImage.fillAmount = ((float)defenceTier - shift) / max;
            fillImage.color = next ? Color.magenta : Color.cyan;
        }

        return defenceTier - shift == max;
    }

    void TierChange(bool instant)
    {
        Defence defence = SelectedTurret.GetComponent<Defence>();

        GameManager.PlayAnimation(PlayerController.global.UIAnimation, "TurretTierOne", defence.CurrentTier > 0, instant);
        GameManager.PlayAnimation(PlayerController.global.UIAnimation, "TurretTierTwo", defence.CurrentTier > 1, instant);


    }

    public void UpdateTier(TurretStats buttonStat = null)
    {
        List<TurretStats> turretStats = GameManager.FindComponent<TurretStats>(PlayerController.global.turretMenuHolder.transform);
        Defence defence = SelectedTurret.GetComponent<Defence>();

        for (int i = 0; i < turretStats.Count; i++)
        {
            bool upgrade = turretStats[i] == buttonStat;

            if (SetTeir(turretStats[i].fillImage, ref turretStats[i].changeTier.damageTier, ref defence.changeTier.damageTier, upgrade)
            && SetTeir(turretStats[i].fillImage, ref turretStats[i].changeTier.healthTier, ref defence.changeTier.healthTier, upgrade)
            && SetTeir(turretStats[i].fillImage, ref turretStats[i].changeTier.rangeTier, ref defence.changeTier.rangeTier, upgrade)
            && SetTeir(turretStats[i].fillImage, ref turretStats[i].changeTier.rateTier, ref defence.changeTier.rateTier, upgrade))
            {
                if (buttonStat && SelectedTurret.GetComponent<Defence>().CurrentTier < 2)
                {
                    SelectedTurret.GetComponent<Defence>().CurrentTier++;

                    TierChange(false);
                    GameManager.global.SoundManager.PlaySound(GameManager.global.UpgradeMenuClickSound);
                    UpdateTier(); //updates fill
                }
            }
        }

    }

    private void BuildMode()
    {
        Ray ray = LevelManager.global.SceneCamera.ScreenPointToRay(cursorPosition);

        TurretMenuSet(SelectedTurret);

        if (!MouseOverUI() && Physics.Raycast(ray, out RaycastHit hitData, Mathf.Infinity, GameManager.ReturnBitShift(new string[] { "Grid" })))
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

            Vector3 worldPos = hitData.point;
            Vector3 gridPos = buildGrid.GetCellCenterWorld(buildGrid.WorldToCell(worldPos));
            worldPos = new Vector3(gridPos.x, 0, gridPos.z);
            Vector2 gridNumber = new Vector2();
            gridNumber.x = (worldPos.x + 57.20f) / 4.0f;
            gridNumber.y = (worldPos.z + 145.30f) / 4.0f;

            Collider[] colliders = Physics.OverlapSphere(worldPos, minDistanceBetweenTurrets, GameManager.ReturnBitShift(new string[] { "Building", "Resource", "Boar" }));

            cantPlace = false;
            hoveringTurret = false;

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].tag == "Turret" && colliders[i].gameObject.transform.localScale == Vector3.one)
                {
                    hoveringTurret = true;

                    bool enter = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || GameManager.global.selectCTRL;


                    if (!SelectedTurret && enter)
                    {
                        ClearBlueprint();

                        PlayerController.global.turretMenuHolder.position = LevelManager.global.SceneCamera.WorldToScreenPoint(hitData.point);

                        SelectedTurret = colliders[i].GetComponentInParent<Building>();

                        Defence defence = SelectedTurret.GetComponent<Defence>();
                        PlayerController.global.turretImageIcon.sprite = defence.spriteTierList[defence.CurrentTier];
                        PlayerController.global.turretMenuTitle.text = SelectedTurret.buildingObject.ToString();

                        TierChange(true);

                        UpdateTier();

                    }


                    return;
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

                Defence defence = turretBlueprint.GetComponent<Defence>();
                defence.enabled = false;
                Destroy(defence);

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
            if (IsInRange(worldPos) && PlayerController.global.CheckSufficientResources() && !occupied[(int)gridNumber.x, (int)gridNumber.y] && !cantPlace)
            {
                BluePrintSet(turretBlueprintBlue);

                if (selectBool)
                {
                    float timer = 0f;
                    switch (buildType)
                    {
                        case BuildType.Turret:
                            timer = 5.0f;
                            break;
                        case BuildType.Cannon:
                            timer = 5.0f;
                            break;
                        case BuildType.Slow:
                            timer = 5.0f;
                            break;
                        case BuildType.Scatter:
                            timer = 10.0f;
                            break;
                        default:
                            timer = 0f;
                            break;
                    }
                    occupied[(int)gridNumber.x, (int)gridNumber.y] = true;
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

                        if (!PlayerController.global.CheckSufficientResources())
                        {
                            PlayerController.global.ShakeResourceHolder();
                        }
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
        PlayerController.global.CheckSufficientResources(true);
        GameObject newTurret = Instantiate(prefab, position, Quaternion.identity);

        newTurret.transform.localScale = Vector3.zero;
        newTurret.GetComponent<Building>().gridLocation = gridPos;

        ReturnVFXBuilding(newTurret.transform, turretTimer);


        newTurret.GetComponent<Defence>().enabled = false;

        float timer = 0f;
        while (timer < 1.0f)
        {
            timer += Time.deltaTime / turretTimer;
            newTurret.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, timer);
            yield return null;
        }

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
        PlayerModeHandler.global.TurretMenuSet(false);
        PlayerController.global.UpdateResourceHolder();
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
        PlayerController.global.OpenResourceHolder(active);

        PlayerController.global.CharacterAnimator.gameObject.SetActive(!active);
        if (active)
        {
            PlayerController.global.evading = false;
            lastMode = playerModes;
            // entryPosition = PlayerController.global.transform.position;
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
            PlayerController.global.UpdateResourceHolder();
            PlayerController.global.ChangeTool(new PlayerController.ToolData() { HammerBool = true });

            // Debug.Log("Build");
        }
        else
        {

        }
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

    //// Check building distance
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;

    //    Vector3 house = House.transform.position;

    //    float top = house.x + distanceAwayFromPlayerX.x;
    //    float bot = house.x - distanceAwayFromPlayerX.y;
    //    float right = house.z + distanceAwayFromPlayerZ.x;
    //    float left = house.z - distanceAwayFromPlayerZ.y;

    //    Vector3 houseX1 = new(top, house.y, house.z);
    //    Vector3 houseX2 = new(bot, house.y, house.z);
    //    Vector3 houseZ1 = new(house.x, house.y, right);
    //    Vector3 houseZ2 = new(house.x, house.y, left);
    //    Gizmos.DrawLine(house, houseX1);
    //    Gizmos.DrawLine(house, houseX2);
    //    Gizmos.DrawLine(house, houseZ1);
    //    Gizmos.DrawLine(house, houseZ2);
    //}

    public bool IsInRange(Vector3 currentTarget)
    {
        Vector3 playerPos = PlayerController.global.transform.position;

        float top = playerPos.x + distanceAwayFromPlayerX.x;
        float bot = playerPos.x - distanceAwayFromPlayerX.y;
        float right = playerPos.z + distanceAwayFromPlayerZ.x;
        float left = playerPos.z - distanceAwayFromPlayerZ.y;

        if (currentTarget.x < top && currentTarget.x > bot && currentTarget.z < right && currentTarget.z > left)
        {
            return true;
        }

        return false;
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