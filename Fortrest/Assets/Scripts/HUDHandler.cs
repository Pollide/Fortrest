using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDHandler : MonoBehaviour
{
    public static HUDHandler global;

    [SerializeField] private Image combatIcon;
    [SerializeField] private Image gatherIcon;
    [SerializeField] private Image balistaIcon;
    [SerializeField] private Image glyphIcon;
    [SerializeField] private Image cannonIcon;
    [SerializeField] private Image scatterIcon;
    [SerializeField] private GameObject[] objectsToDisableBuild;
    [SerializeField] private GameObject[] objectsToDisableNonBuild;

    [SerializeField] private float time = 0;
    [SerializeField] private float duration = 1;
    [SerializeField] private bool isCombat = false;
    [SerializeField] private bool isGather = false;
    [SerializeField] private bool isBalista = false;
    [SerializeField] private bool isGlyph = false;
    [SerializeField] private bool isCannon = false;
    [SerializeField] private bool isScatter = false;

    private void Awake()
    {
        global = this;
    }

    private void Start()
    {
        LerpRectScale(gatherIcon.rectTransform, true, 1f, 0.52f);
        LerpRectScale(combatIcon.rectTransform, false, 1f, 0.52f);
    }

    private void Update()
    {
        if (PlayerModeHandler.global.playerModes == PlayerModes.CombatMode)
        {
            ResourceAndCombatHUD(false);
        }
        else if (PlayerModeHandler.global.playerModes == PlayerModes.ResourceMode)
        {
            ResourceAndCombatHUD(true);
        }
        else if (PlayerModeHandler.global.playerModes == PlayerModes.BuildMode)
        {
            BuildHUD();
        }
        else
        {
            combatIcon.enabled = false;
            gatherIcon.enabled = false;
            ResetIcons();
        }
    }

    public void ResourceAndCombatHUD(bool gather)
    {
        if (!isGather && gather || !isCombat && !gather)
        {
            combatIcon.enabled = true;
            gatherIcon.enabled = true;

            ResetIcons();
            ActivateObjects(true);
            isGather = gather;
            isCombat = !gather;
        }

        if (time < duration)
        {
            LerpRectScale(gatherIcon.rectTransform, gather, 1f, 0.52f);
            LerpRectScale(combatIcon.rectTransform, !gather, 1f, 0.52f);

            time += Time.deltaTime;
        }
    }

    void ResetIcons()
    {
        ActivateObjects(false);
        isGather = false;
        isCombat = false;
        isBalista = false;
        isGlyph = false;
        isCannon = false;
        isScatter = false;
        time = 0f;
    }

    private void BuildHUD()
    {
        if (PlayerModeHandler.global.buildType == BuildType.Turret)
        {
            if (!isBalista)
            {
                balistaIcon.GetComponentInChildren<RectTransform>().localScale = balistaIcon.rectTransform.localScale / 2;
                ResetIcons();
                isBalista = true;

            }
        }
        else if (PlayerModeHandler.global.buildType == BuildType.Cannon)
        {
            if (!isCannon)
            {
                cannonIcon.GetComponentInChildren<RectTransform>().localScale = cannonIcon.rectTransform.localScale / 2;
                ResetIcons();
                isCannon = true;
            }
        }
        else if (PlayerModeHandler.global.buildType == BuildType.Slow)
        {
            if (!isGlyph)
            {
                glyphIcon.GetComponentInChildren<RectTransform>().localScale = glyphIcon.rectTransform.localScale / 2;
                ResetIcons();
                isGlyph = true;
            }
        }
        else if (PlayerModeHandler.global.buildType == BuildType.Scatter)
        {
            if (!isScatter)
            {
                scatterIcon.GetComponentInChildren<RectTransform>().localScale = scatterIcon.rectTransform.localScale / 2;
                ResetIcons();
                isScatter = true;
            }
        }


        if (time < duration)
        {
            LerpRectScale(glyphIcon.rectTransform, isGlyph);
            LerpRectScale(cannonIcon.rectTransform, isCannon);
            LerpRectScale(balistaIcon.rectTransform, isBalista);
            LerpRectScale(scatterIcon.rectTransform, isScatter);

            time += Time.deltaTime;
        }
    }

    void LerpRectScale(RectTransform rectTransform, bool flip = false, float size = 1, float scaleMulti = 0.7f)
    {
        Vector3 scale = new(scaleMulti, scaleMulti, 1);

        Vector3 a = scale * (flip ? size : size * 2);
        Vector3 b = scale * (flip ? size * 2 : size);

        if (Mathf.Abs(rectTransform.localScale.x - b.x) > 0.1f) //prevents icons that are already small from being shrinked again
            rectTransform.localScale = Vector3.Lerp(a, b, time / duration);
    }

    private void ActivateObjects(bool _active)
    {
        for (int i = 0; i < objectsToDisableBuild.Length; i++)
        {
            if (objectsToDisableBuild[i].activeSelf == !_active)
            {
                objectsToDisableBuild[i].SetActive(_active);
            }

        }

        for (int i = 0; i < objectsToDisableNonBuild.Length; i++)
        {
            if (objectsToDisableNonBuild[i].activeSelf == _active)
            {
                objectsToDisableNonBuild[i].SetActive(!_active);
            }
        }
    }
}
