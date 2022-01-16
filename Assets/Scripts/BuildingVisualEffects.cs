using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace CityBuilder
{
    public class BuildingVisualEffects : MonoBehaviour
    {
        enum ActiveMaterial
        {
            None,
            Opaque,
            Transparent
        }

        //
        // Configurable Parameters
        //
        public Material transparentMaterial = null;
        [Range(0, 1)] public float opacityThreshold = 0.4f;
        [Range(0, 5)] public float transitionSpeed = 1.0f;

        //
        // Cached References
        //
        private MeshRenderer[] renderers = null;
        private Material opaqueMaterial = null;
        private Material transparentMaterialInstance = null;

        //
        // Internal Variables
        //
        private ActiveMaterial activeMaterial = ActiveMaterial.Opaque;
        private float currentOpacity = 1.0f;
        public float targetOpacity = 1.0f;
        private Color currentHighlight = Color.black;
        [ColorUsage(false, true)] public Color targetHighlight = Color.black;

        void Start()
        {
            renderers = GetComponentsInChildren<MeshRenderer>(true);
        }

        void Update()
        {
            //
            // Lerp Values
            //
            currentHighlight = Color.Lerp(currentHighlight, targetHighlight, transitionSpeed * Time.deltaTime);
            currentOpacity = Mathf.Lerp(currentOpacity, targetOpacity, transitionSpeed * Time.deltaTime);
            if (Mathf.Abs(targetOpacity - currentOpacity) < 0.01)
                currentOpacity = targetOpacity;

            //
            // Compute State
            //
            if(currentOpacity < 1.0f && transparentMaterialInstance == null)
            {
                activeMaterial = ActiveMaterial.Transparent;
                opaqueMaterial = renderers[0].material;
                transparentMaterialInstance = new Material(transparentMaterial);
                transparentMaterialInstance.SetTexture("_MainTex", opaqueMaterial.GetTexture("_MainTex"));
                transparentMaterialInstance.SetTexture("_SmoothMap", opaqueMaterial.GetTexture("_SmoothMap"));
                transparentMaterialInstance.SetTexture("_MetallicMap", opaqueMaterial.GetTexture("_MetallicMap"));
                transparentMaterialInstance.SetTexture("_EmissionMap", opaqueMaterial.GetTexture("_EmissionMap"));
                transparentMaterialInstance.SetInt("_NightEmissionEnable", opaqueMaterial.GetInt("_NightEmissionEnable"));
                foreach (var _renderer in renderers)
                    _renderer.material = transparentMaterialInstance;
            }
            else if(currentOpacity == 1.0f && transparentMaterialInstance != null)
            {
                activeMaterial = ActiveMaterial.Opaque;
                foreach (var _renderer in renderers)
                    _renderer.material = opaqueMaterial;
                opaqueMaterial = null;
                transparentMaterialInstance = null;
            }

            //
            // Apply Values
            //
            switch (activeMaterial)
            {
                case ActiveMaterial.Opaque:
                    foreach (var _renderer in renderers)
                        _renderer.material.SetColor("_HighlightColor", currentHighlight);
                    break;

                case ActiveMaterial.Transparent:
                    foreach (var _renderer in renderers)
                        _renderer.material.SetFloat("_Opacity", currentOpacity);
                    break;
            }
        }

        public void SetHighlightColor(Color highlightColor)
        {
            targetHighlight = highlightColor;
        }

        public void FadeBuilding(bool fadeEnable)
        {
            if (fadeEnable)
                targetOpacity = opacityThreshold;
            else
                targetOpacity = 1.0f;
        }
    }
}
