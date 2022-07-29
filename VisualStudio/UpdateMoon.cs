﻿extern alias Hinterland;
using Hinterland;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace BetterNightSky;

[MelonLoader.RegisterTypeInIl2Cpp]
internal sealed class UpdateMoon : MonoBehaviour
{
    public const int MOON_CYCLE_DAYS = 29;

    public Texture2D[] MoonPhaseTextures;

    private Color baseColor;
    private int forcedPhase = -1;
    private float lastAlpha = -1;
    private int lastPhaseTextureIndex = -1;
    private Material material;

    public UpdateMoon(System.IntPtr intPtr) : base(intPtr) { }

    [HideFromIl2Cpp]
    public void SetForcedPhase(int forcedPhase)
    {
        this.forcedPhase = Mathf.Clamp(forcedPhase, 0, MoonPhaseTextures.Length);
        UpdatePhase();
    }

    public void Start()
    {
        material = GetComponentInChildren<Renderer>().material;
        baseColor = material.GetColor("_TintColor");
        UpdatePhase();
    }

    public void Update()
    {
        UpdateAlpha();
        UpdateDirection();
    }

    public void UpdatePhase()
    {
        if (MoonPhaseTextures == null || material == null)
        {
            return;
        }

        int phaseTextureIndex = GetPhaseTextureIndex();
        if (lastPhaseTextureIndex == phaseTextureIndex)
        {
            return;
        }

        lastPhaseTextureIndex = phaseTextureIndex;
        //material.mainTexture = MoonPhaseTextures[lastPhaseTextureIndex];
        material.mainTexture = Implementation.GetMoonPhaseTexture(lastPhaseTextureIndex);
    }

    [HideFromIl2Cpp]
    private int GetPhaseTextureIndex()
    {
        if (forcedPhase >= 0)
        {
            return forcedPhase;
        }

        UniStormWeatherSystem uniStormWeatherSystem = GameManager.GetUniStorm();
        int day = uniStormWeatherSystem.GetDayNumber() + uniStormWeatherSystem.m_MoonCycleStartDay;
        return day % MOON_CYCLE_DAYS * MoonPhaseTextures.Length / MOON_CYCLE_DAYS;
    }

    private void UpdateAlpha()
    {
        float currentAlpha = GameManager.GetUniStorm().GetActiveTODState().m_MoonAlpha;
        if (Mathf.Approximately(lastAlpha, currentAlpha))
        {
            return;
        }

        lastAlpha = currentAlpha;
        material.SetColor("_TintColor", baseColor * lastAlpha);
    }

    private void UpdateDirection()
    {
        transform.forward = -GameManager.GetUniStorm().m_MoonLight.transform.forward;
    }
}