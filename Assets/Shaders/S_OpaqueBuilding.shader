Shader "Custom/S_OpaqueBuilding"
{
    Properties
    {
        [NoScaleOffset] _MainTex("Albedo (RGB)", 2D) = "white" {}
        [NoScaleOffset] _SmoothMap("Smoothness", 2D) = "white" {}
        [NoScaleOffset] _MetallicMap("Metallic", 2D) = "black" {}
        [NoScaleOffset] _EmissionMap("Emission", 2D) = "black" {}

        _NightEmissionEnable("Night Emission Enable", int) = 0
        [HDR] _HighlightColor("Highlight Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
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
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _HighlightColor)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float4 baseColor = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = baseColor.rgb;

            o.Metallic = tex2D(_MetallicMap, IN.uv_MainTex);
            o.Smoothness = tex2D(_SmoothMap, IN.uv_MainTex);

            o.Emission = max(tex2D(_EmissionMap, IN.uv_MainTex) * UNITY_ACCESS_INSTANCED_PROP(Props, _NightEmissionEnable), UNITY_ACCESS_INSTANCED_PROP(Props, _HighlightColor));
        }
        ENDCG
    }
    FallBack "Diffuse"
}
