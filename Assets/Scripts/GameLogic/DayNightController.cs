using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{
    [RequireComponent(typeof(TimeController))]
    public class DayNightController : MonoBehaviour
    {
        //
        // Configurable Parameters
        //
        [Header("Lighting")]
        public bool enableLightCycle = true;
        public Light mainLight = null;
        public float lighIntensityMin = 0.4f;
        public float lightIntensityMax = 1.4f;

        [Header("Skybox")]
        public bool enableSkyCycle = true;
        public Material skyMaterial = null;
        public Color SkyColorDay = new Color(0.8f, 0.88f, 1.0f); // CCE0FF
        public Color SkyColorNight = new Color(0.098f, 0.094f, 0.1f); // 17181A
        public Color GroundColorDay = new Color(0.0f, 0.420f, 1f); // 006BFF
        public Color GroundColorNight = new Color(0.0f, 0.041f, 0.1f); // 000A1A
        public float skyExposureMin = 0.6f;
        public float skyExposureMax = 2.0f;

        [Header("Camera Tint")]
        public bool enableCameraTintCycle = true;


        //
        // Cached References
        //
        private TimeController timeController = null;
        private float currentLightIntensity = 1.0f;

        private Color currentSkyColor = Color.white;
        private Color currentGroundColor = Color.white;
        private float currentExposure = 1.0f;

        private CustomCameraEffect cameraEffect = null;
        private float currentCameraEffectWeight = 0.0f;


        void Start()
        {
            timeController = GetComponent<TimeController>();
            cameraEffect = Camera.main.GetComponent<CityBuilder.CustomCameraEffect>();
        }

        void Update()
        {
            float alpha;
            bool isSunRising;
            float timeOfDay;

            timeOfDay = (float)timeController.GetCurrentDayTime();
            if (timeOfDay >= TimeController.TIME_MIDNIGHT && timeOfDay < TimeController.TIME_MIDDAY)
            {
                isSunRising = true;
                alpha = timeOfDay / TimeController.TIME_MIDDAY;
            }
            else
            {
                isSunRising = false;
                alpha = (timeOfDay - TimeController.TIME_MIDDAY) / TimeController.TIME_MIDDAY;
            }

            if (enableLightCycle)
                UpdateSunIntensity(isSunRising, alpha);
            if (enableSkyCycle)
                UpdateSkyboxColor(isSunRising, alpha);
            if (enableCameraTintCycle)
                UpdateCameraTint(isSunRising, alpha);
        }


        private void UpdateSkyboxColor(bool isSunRising, float alpha)
        {
            if (isSunRising)
            {
                currentSkyColor = SmoothLerp(SkyColorNight, SkyColorDay, alpha);
                currentGroundColor = SmoothLerp(GroundColorNight, GroundColorDay, alpha);
                currentExposure = SmoothLerp(skyExposureMin, skyExposureMax, alpha);
            }
            else
            {
                currentSkyColor = SmoothLerp(SkyColorDay, SkyColorNight, alpha);
                currentGroundColor = SmoothLerp(GroundColorDay, GroundColorNight, alpha);
                currentExposure = SmoothLerp(skyExposureMax, skyExposureMin, alpha);
            }
            skyMaterial.SetColor("_SkyTint", currentSkyColor);
            skyMaterial.SetColor("_GroundColor", currentGroundColor);
            skyMaterial.SetFloat("_Exposure", currentExposure);
        }


        private void UpdateSunIntensity(bool isSunRising, float alpha)
        {
            if (isSunRising)
            {
                currentLightIntensity = SmoothLerp(lighIntensityMin, lightIntensityMax, alpha);
            }
            else
            {
                currentLightIntensity = SmoothLerp(lightIntensityMax, lighIntensityMin, alpha);
            }
            mainLight.intensity = currentLightIntensity;
        }

        private void UpdateCameraTint(bool isSunRising, float alpha)
        {
            if (isSunRising)
            {
                currentCameraEffectWeight = SmoothLerp(1, 0, alpha);
            }
            else
            {
                currentCameraEffectWeight = SmoothLerp(0, 1, alpha);
            }
            cameraEffect.effectWeight = currentCameraEffectWeight;
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
}
