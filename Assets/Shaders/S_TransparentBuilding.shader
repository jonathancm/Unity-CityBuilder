Shader "Custom/S_TransparentBuilding"
{
    Properties
    {
        [NoScaleOffset] _MainTex("Albedo (RGB)", 2D) = "white" {}
        [NoScaleOffset] _SmoothMap("Smoothness", 2D) = "gray" {}
        [NoScaleOffset] _MetallicMap("Metallic", 2D) = "black" {}
        [NoScaleOffset] _EmissionMap("Emission", 2D) = "black" {}

        _NightEmissionEnable ("Night Emission Enable", int) = 0
        _Opacity("Opacity", Range(0.0,1.0)) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            Name "BuildingDepth"
            ZWrite On
            ColorMask 0
        }

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 5.0

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_Emission;
        };

        sampler2D _MainTex;
        sampler2D _SmoothMap;
        sampler2D _MetallicMap;
        sampler2D _EmissionMap;

        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(fixed, _NightEmissionEnable)
            UNITY_DEFINE_INSTANCED_PROP(fixed, _Opacity)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 baseColor = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = baseColor.rgb;
            o.Alpha = UNITY_ACCESS_INSTANCED_PROP(Props,_Opacity);

            o.Metallic = tex2D(_MetallicMap, IN.uv_MainTex);
            o.Smoothness = tex2D(_SmoothMap, IN.uv_MainTex);

            o.Emission = tex2D(_EmissionMap, IN.uv_MainTex) * UNITY_ACCESS_INSTANCED_PROP(Props, _NightEmissionEnable);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
