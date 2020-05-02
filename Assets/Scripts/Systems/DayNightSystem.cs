using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{
    [RequireComponent(typeof(TimeKeeperSystem))]
    public class DayNightSystem : MonoBehaviour
    {
        public enum AnimationState
        {
            Idle,
            Sunrise,
            Sunset
        }

        //
        // Configurable Parameters
        //
        [Header("Animation")]
        public float transitionDurationInHours = 2;
        public AnimationState animState = AnimationState.Idle;
        [Range(0, 1)] public float alpha = 0;

        [Header("Camera Tint")]
        public CustomCameraEffect cameraEffect = null;
        [Range(0, 1)] public float effectWeightNight = 1.0f;
        [Range(0, 1)] public float effectWeightDay = 0.0f;

        [Header("Skybox")]
        public Material skyMaterial = null;
        public Color SkyColorDay = new Color(0.8f, 0.88f, 1.0f); // CCE0FF
        public Color SkyColorNight = new Color(0.098f, 0.094f, 0.1f); // 17181A
        public Color GroundColorDay = new Color(0.0f, 0.420f, 1f); // 006BFF
        public Color GroundColorNight = new Color(0.0f, 0.041f, 0.1f); // 000A1A
        public float skyExposureNight = 0.6f;
        public float skyExposureDay = 2.0f;


        //
        // Cached References
        //
        private TimeKeeperSystem timeController = null;


        //
        // Internal Variables
        //
        private float transitionStartTime = 0;
        private float currentCameraEffectWeight = 0.0f;
        private Color currentSkyColor = Color.white;
        private Color currentGroundColor = Color.white;
        private float currentExposure = 1.0f;


        void Start()
        {
            cameraEffect = Camera.main.GetComponent<CustomCameraEffect>();
            timeController = GetComponent<TimeKeeperSystem>();
            timeController.eventSunrise.AddListener(OnSunrise);
            timeController.eventSunset.AddListener(OnSunset);

            //
            // Initialize Scene Lighting
            //
            if (timeController.GetDayPeriod() == TimeKeeperSystem.DayPeriod.Day)
                SetDayLighting();
            else
                SetNightLighting();
        }

        void Update()
        {
            switch (animState)
            {
                case AnimationState.Sunrise:
                    alpha = Mathf.Clamp(CalculateTransitionProgress(), 0, 1);
                    AnimateSunrise(alpha);
                    if (alpha == 1)
                        animState = AnimationState.Idle;
                    break;

                case AnimationState.Sunset:
                    alpha = Mathf.Clamp(CalculateTransitionProgress(), 0, 1);
                    AnimateSunset(alpha);
                    if (alpha == 1)
                        animState = AnimationState.Idle;
                    break;
            }

            skyMaterial.SetColor("_SkyTint", currentSkyColor);
            skyMaterial.SetColor("_GroundColor", currentGroundColor);
            skyMaterial.SetFloat("_Exposure", currentExposure);
            cameraEffect.effectWeight = currentCameraEffectWeight;
        }


        private void SetNightLighting()
        {
            alpha = 1;
            currentSkyColor = SkyColorNight;
            currentGroundColor = GroundColorNight;
            currentExposure = skyExposureNight;
            currentCameraEffectWeight = effectWeightNight;
        }


        private void SetDayLighting()
        {
            alpha = 1;
            currentSkyColor = SkyColorDay;
            currentGroundColor = GroundColorDay;
            currentExposure = skyExposureDay;
            currentCameraEffectWeight = effectWeightDay;
        }


        private void AnimateSunrise(float alpha)
        {
            currentSkyColor = SmoothLerp(SkyColorNight, SkyColorDay, alpha);
            currentGroundColor = SmoothLerp(GroundColorNight, GroundColorDay, alpha);
            currentExposure = SmoothLerp(skyExposureNight, skyExposureDay, alpha);
            currentCameraEffectWeight = SmoothLerp(1, 0, alpha);
        }


        private void AnimateSunset(float alpha)
        {
            currentSkyColor = SmoothLerp(SkyColorDay, SkyColorNight, alpha);
            currentGroundColor = SmoothLerp(GroundColorDay, GroundColorNight, alpha);
            currentExposure = SmoothLerp(skyExposureDay, skyExposureNight, alpha);
            currentCameraEffectWeight = SmoothLerp(0, 1, alpha);
        }


        private void OnSunrise()
        {
            transitionStartTime = timeController.GetCurrentDayTime();
            animState = AnimationState.Sunrise;
        }


        private void OnSunset()
        {
            transitionStartTime = timeController.GetCurrentDayTime();
            animState = AnimationState.Sunset;
        }

        private float CalculateTransitionProgress()
        {
            float currentDayTime = timeController.GetCurrentDayTime();

            if (currentDayTime < transitionStartTime)
                currentDayTime += TimeKeeperSystem.SECONDS_PER_DAY;

            return (currentDayTime - transitionStartTime) / (transitionDurationInHours * TimeKeeperSystem.SECONDS_PER_HOUR);
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
