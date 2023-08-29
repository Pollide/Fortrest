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

    [SerializeField] private Vector3 downScale = new(0.5f, 0.5f, 1);

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

    private void Update()
    {
        if (PlayerModeHandler.global.playerModes == PlayerModes.CombatMode)
        {
            CombatHUD();
        }
        else if (PlayerModeHandler.global.playerModes == PlayerModes.ResourceMode)
        {
            GatherHUD();
        }
        else if (PlayerModeHandler.global.playerModes == PlayerModes.BuildMode)
        {
            BuildHUD();
        }
        else
        {
            combatIcon.enabled = false;
            gatherIcon.enabled = false;
            isGather = false;
            isCombat = false;
            isBalista = false;
            isGlyph = false;
            isCannon = false;
            isScatter = false;
        }
    }

    public void CombatHUD()
    {
        if (!combatIcon.enabled || !gatherIcon.enabled)
        {
            combatIcon.enabled = true;
            gatherIcon.enabled = true;
        }

        if (!isCombat)
        {
            ActivateObjects(true);

            isGather = false;
            isCombat = true;
            isBalista = false;
            isGlyph = false;
            isCannon = false;
            isScatter = false;
            time = 0f;
        }

        if (time < duration)
        {
            gatherIcon.rectTransform.localScale = Vector3.Lerp(Vector3.one, downScale, time / duration);
            combatIcon.rectTransform.localScale = Vector3.Lerp(downScale, Vector3.one, time / duration);

            time += Time.deltaTime;
        }
    }

    public void GatherHUD()
    {
        if (!combatIcon.enabled || !gatherIcon.enabled)
        {
            combatIcon.enabled = true;
            gatherIcon.enabled = true;
        }

        if (!isGather)
        {
            ActivateObjects(true);

            isGather = true;
            isCombat = false;
            isBalista = false;
            isGlyph = false;
            isCannon = false;
            isScatter = false;
            time = 0f;
        }

        if (time < duration)
        {
            combatIcon.rectTransform.localScale = Vector3.Lerp(Vector3.one, downScale, time / duration);
            gatherIcon.rectTransform.localScale = Vector3.Lerp(downScale, Vector3.one, time / duration);

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

    void LerpRectScale(RectTransform rectTransform, bool flip = false)
    {
        Vector3 a = flip ? downScale : Vector3.one;
        Vector3 b = flip ? Vector3.one : downScale;

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
