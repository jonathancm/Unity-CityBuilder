using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{
    public class CustomCameraEffect : MonoBehaviour
    {
        [Range(0.0f,1.0f)]
        public float effectWeight = 1.0f;
        public Material material = null;

        private void Update()
        {
            material.SetFloat("_Weight", effectWeight);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (material == null)
                return;

            Graphics.Blit(source, destination, material);
        }
    }
}
