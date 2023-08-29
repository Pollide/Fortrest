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

    private void BuildHUD()
    {
        if (PlayerModeHandler.global.buildType == BuildType.Turret)
        {
            if (!isBalista)
            {
                ActivateObjects(false);
                balistaIcon.GetComponentInChildren<RectTransform>().localScale = balistaIcon.rectTransform.localScale / 2;
                isGather = false;
                isCombat = false;
                isBalista = true;
                isGlyph = false;
                isCannon = false;
                isScatter = false;
                time = 0f;
            }

            if (time < duration)
            {
                glyphIcon.rectTransform.localScale = Vector3.Lerp(Vector3.one, downScale, time / duration);
                cannonIcon.rectTransform.localScale = Vector3.Lerp(Vector3.one, downScale, time / duration);
                scatterIcon.rectTransform.localScale = Vector3.Lerp(Vector3.one, downScale, time / duration);
                balistaIcon.rectTransform.localScale = Vector3.Lerp(downScale, Vector3.one, time / duration);

                time += Time.deltaTime;
            }
        }
        else if (PlayerModeHandler.global.buildType == BuildType.Cannon)
        {
            if (!isCannon)
            {
                ActivateObjects(false);
                cannonIcon.GetComponentInChildren<RectTransform>().localScale = cannonIcon.rectTransform.localScale / 2;
                isGather = false;
                isCombat = false;
                isBalista = false;
                isGlyph = false;
                isCannon = true;
                isScatter = false;
                time = 0f;
            }

            if (time < duration)
            {
                glyphIcon.rectTransform.localScale = Vector3.Lerp(Vector3.one, downScale, time / duration);
                balistaIcon.rectTransform.localScale = Vector3.Lerp(Vector3.one, downScale, time / duration);
                scatterIcon.rectTransform.localScale = Vector3.Lerp(Vector3.one, downScale, time / duration);
                cannonIcon.rectTransform.localScale = Vector3.Lerp(downScale, Vector3.one, time / duration);

                time += Time.deltaTime;
            }
        }
        else if (PlayerModeHandler.global.buildType == BuildType.Slow)
        {
            if (!isGlyph)
            {
                ActivateObjects(false);
                glyphIcon.GetComponentInChildren<RectTransform>().localScale = glyphIcon.rectTransform.localScale / 2;
                isGather = false;
                isCombat = false;
                isBalista = false;
                isGlyph = true;
                isCannon = false;
                isScatter = false;
                time = 0f;
            }

            if (time < duration)
            {
                balistaIcon.rectTransform.localScale = Vector3.Lerp(Vector3.one, downScale, time / duration);
                cannonIcon.rectTransform.localScale = Vector3.Lerp(Vector3.one, downScale, time / duration);
                scatterIcon.rectTransform.localScale = Vector3.Lerp(Vector3.one, downScale, time / duration);
                glyphIcon.rectTransform.localScale = Vector3.Lerp(downScale, Vector3.one, time / duration);

                time += Time.deltaTime;
            }
        }
        else if (PlayerModeHandler.global.buildType == BuildType.Scatter)
        {
            if (!isScatter)
            {
                ActivateObjects(false);
                scatterIcon.GetComponentInChildren<RectTransform>().localScale = scatterIcon.rectTransform.localScale / 2;
                isGather = false;
                isCombat = false;
                isBalista = false;
                isGlyph = false;
                isCannon = false;
                isScatter = true;
                time = 0f;
            }

            if (time < duration)
            {
                glyphIcon.rectTransform.localScale = Vector3.Lerp(Vector3.one, downScale, time / duration);
                cannonIcon.rectTransform.localScale = Vector3.Lerp(Vector3.one, downScale, time / duration);
                balistaIcon.rectTransform.localScale = Vector3.Lerp(Vector3.one, downScale, time / duration);
                scatterIcon.rectTransform.localScale = Vector3.Lerp(downScale, Vector3.one, time / duration);

                time += Time.deltaTime;
            }
        }
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
