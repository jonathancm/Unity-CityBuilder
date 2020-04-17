using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TimeController))]
public class DayNightController : MonoBehaviour
{
    //
    // Configurable Parameters
    //
    [Header("Lighting")]
    public Light mainLight = null;
    public float lighIntensityMin = 0.4f;
    public float lightIntensityMax = 1.4f;

    [Header("Skybox")]
    public Material skyMaterial = null;
    public Color SkyColorDay = new Color(0.8f, 0.88f, 1.0f); // CCE0FF
    public Color SkyColorNight = new Color(0.09019608f, 0.09411765f, 0.1f); // 17181A
    public Color GroundColorDay = new Color(0.0f, 0.4196078f, 1f); // 006BFF
    public Color GroundColorNight = new Color(0.0f, 0.04117646f, 0.1f); // 000A1A
    public float skyExposureMin = 0.6f;
    public float skyExposureMax = 2.0f;

    //
    // Cached References
    //
    private TimeController timeController = null;
    private Color currentAmbientColor = Color.white;
    private Color currentSkyColor = Color.white;
    private Color currentGroundColor = Color.white;
    private float currentExposure = 1.0f;
    private float currentLightIntensity = 1.0f;

    void Start()
    {
        timeController = GetComponent<TimeController>();
    }

    void Update()
    {
        float alpha;
        int timeOfDay = timeController.GetCurrentDayTime();
        if(timeOfDay >= TimeController.MidNight && timeOfDay < TimeController.MidDay)
        {
            alpha = (float)timeOfDay / (float)TimeController.MidDay;
            currentSkyColor = SmoothLerp(SkyColorNight, SkyColorDay, alpha);
            currentGroundColor = SmoothLerp(GroundColorNight, GroundColorDay, alpha);
            currentExposure = SmoothLerp(skyExposureMin, skyExposureMax, alpha);
            currentLightIntensity = SmoothLerp(lighIntensityMin, lightIntensityMax, alpha);
        }
        else
        {
            alpha = (float)(timeOfDay - TimeController.MidDay) / (float)TimeController.MidDay;
            currentSkyColor = SmoothLerp(SkyColorDay, SkyColorNight, alpha);
            currentGroundColor = SmoothLerp(GroundColorDay, GroundColorNight, alpha);
            currentExposure = SmoothLerp(skyExposureMax, skyExposureMin, alpha);
            currentLightIntensity = SmoothLerp(lightIntensityMax, lighIntensityMin, alpha);
        }

        skyMaterial.SetColor("_SkyTint", currentSkyColor);
        skyMaterial.SetColor("_GroundColor", currentGroundColor);
        skyMaterial.SetFloat("_Exposure", currentExposure);
        mainLight.intensity = currentLightIntensity;
    }

    private static Color SmoothLerp(Color start, Color end, float alpha)
    {
        return Color.Lerp(start, end, Mathf.SmoothStep(0, 1, alpha));
    }
    private static float SmoothLerp(float start, float end, float alpha)
    {
        return Mathf.Lerp(start, end, Mathf.SmoothStep(0, 1, alpha));
    }
}
