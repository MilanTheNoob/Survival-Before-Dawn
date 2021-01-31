using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Scriptable Objects/Map Generation/New Height Map Data")]
public class HeightMapSettings : UpdatableSettings
{
    [Header("The Noise Settings used for all of the variables to modify terrain generation")]
    public NoiseSettings noiseSettings;

    [Header("Use falloff map?s")]
    public bool useFalloff;

    [Header("Manipulation of the elevation")]
    public float heightMultiplier;
    public AnimationCurve heightCurve;


    //Set the minimum and maximum height of the terrain for the shader
    public float minHeight
    {
        get
        {
            return heightMultiplier * heightCurve.Evaluate(0);
        }
    }

    public float maxHeight
    {
        get
        {
            return heightMultiplier * heightCurve.Evaluate(1);
        }
    }

#if UNITY_EDITOR

    protected override void OnValidate()
    {
        //Check that all the noise values are correct
        noiseSettings.ValidateValues();
        //Continue the original OnValidate function this overrides
        base.OnValidate();
    }

    #endif
}
