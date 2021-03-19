using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Scriptable Objects/Weather & Lighting/New Lighting Settings")]
public class LightingSettings : ScriptableObject
{
    public Gradient AmbientColor;
    public Gradient DirectionalColor;
    public Gradient FogColor;

    [Space]

    public Material DaySkybox;
    public Material NightSkybox;
}
