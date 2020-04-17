// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/S_SkyboxBlend"
{
    Properties
    {
        _Tint ("Tint", Color) = (1,1,1,1)
        _NightTex ("Night Texture", Cube) = "white" {}
        _DayTex ("Day Texture", Cube) = "white" {}
        _Blend ("Blend", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        Tags { "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            samplerCUBE _NightTex;
            samplerCUBE _DayTex;
            float _Blend;
            float4 _Tint;

            struct vertexInput
            {
                float4 vertex: POSITION;
                float3 coord: TEXCOORD0;
            };

            struct vertexOutput
            {
                float4 vertex : SV_POSITION;
                float3 coord: TEXCOORD0;
            };

            vertexOutput vert(vertexInput input)
            {
                vertexOutput output;
                output.vertex = UnityObjectToClipPos(input.vertex);
                output.coord = input.coord;
                return output;
            }

            fixed4 frag(vertexOutput i) : SV_Target
            {
                fixed4 night = texCUBE(_NightTex, i.coord);
                fixed4 day = texCUBE(_DayTex, i.coord);
                fixed4 col = day * _Blend + (night * (1 - _Blend));
                return col;
            }
            ENDCG
        }
    }
}
