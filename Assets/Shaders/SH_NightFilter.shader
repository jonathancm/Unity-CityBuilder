Shader "Hidden/SH_NightFilter"
{
    Properties
    {
        _Tint ("Tint", Color) = (1,1,1,1)
        _Weight("Weight", Range(0.0, 1.0)) = 1.0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            fixed4 _Tint;
            fixed _Weight;

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 baseColor = tex2D(_MainTex, i.uv);
                fixed4 col = ((1 - _Weight) * baseColor) + (_Weight * (baseColor * _Tint));
                col.a = 1;
                return col;
            }
            ENDCG
        }
    }
}
