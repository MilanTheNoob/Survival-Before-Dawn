using UnityEngine;

[ExecuteAlways]
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

    // Called every frame
    void Update()
    {
        // Return if the preset is null
        if (preset == null)
            return;

        // Check if the game is playing
        if (Application.isPlaying)
        {
            // TEMP code to state the time of day
            TimeOfDay += Time.deltaTime / CycleSpeed;
            TimeOfDay %= 24; // Modulus to ensure always between 0-24

            // Update lighting
            UpdateLighting(TimeOfDay / 24f);
        }
        else
        {
            // Update lighting
            UpdateLighting(TimeOfDay / 24f);
        }
    }

    // Called to update lighting
    private void UpdateLighting(float timePercent)
    {
        // Set ambient and fog
        RenderSettings.ambientLight = preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = preset.FogColor.Evaluate(timePercent);

        // If the directional light is set then rotate and set it's color
        if (DirectionalLight != null)
        {
            DirectionalLight.color = preset.DirectionalColor.Evaluate(timePercent);
            Quaternion q = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
            DirectionalLight.transform.rotation = q;
        }

    }

    //Try to find a directional light to use if we haven't set one
    private void OnValidate()
    {
        // Is the directional light null? If so then return
        if (DirectionalLight != null)
            return;

        // Search for lighting tab sun
        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
    }
}