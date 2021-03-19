using UnityEngine;

public class LightingManager : MonoBehaviour
{
    [Header("Weather settings")]
    public LightingSettings preset;
    public Light DirectionalLight;

    [Header("The cycle speed")]
    public int CycleSpeed;

    [Header("The time of the day")]
    [Space, Range(0, 24)]
    public float TimeOfDay;

    [Header("Skyboxe Times")]
    [Range(0, 24)]
    public float ActivateNight;
    [Range(0, 24)]
    public float ActivateDay;

    void FixedUpdate()
    {
        TimeOfDay += Time.deltaTime / CycleSpeed;
        TimeOfDay %= 24;

        float timePercent = TimeOfDay / 24f;

        RenderSettings.ambientLight = preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = preset.FogColor.Evaluate(timePercent);

        if (TimeOfDay > ActivateNight && RenderSettings.skybox != preset.NightSkybox) { RenderSettings.skybox = preset.NightSkybox; }
        if (TimeOfDay > ActivateDay && TimeOfDay < ActivateNight && RenderSettings.skybox != preset.DaySkybox) { RenderSettings.skybox = preset.DaySkybox; }

        if (DirectionalLight != null)
        {
            DirectionalLight.color = preset.DirectionalColor.Evaluate(timePercent);
            Quaternion q = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
            DirectionalLight.transform.rotation = q;
        }
    }

    private void OnValidate()
    {
        if (DirectionalLight != null)
            return;

        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
    }
}