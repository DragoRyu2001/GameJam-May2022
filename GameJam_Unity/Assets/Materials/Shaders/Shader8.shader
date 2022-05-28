Shader "Unlit/Shader7"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BackTex ("Texture", 2D) = "black" {}
        _Health("Health Bar", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #define TAU 6.2831853071
            struct Meshdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _BackTex;
            float4 _BackTex_ST;
            float _Health;
            Interpolators vert (Meshdata v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            float InverseLerp(float a, float b, float v)
            {
                return (v-a)/(b-a);
            }
            fixed4 frag (Interpolators i) : SV_Target
            {
                // sample the texture
                float healthBarMask = _Health>i.uv.x;
                float3 healthBarColor = tex2D(_MainTex, float2(_Health, i.uv.y));
                float3 backColor = tex2D(_BackTex, i.uv);
                float3 output = lerp(backColor, healthBarColor+backColor, healthBarMask);
                return float4(output,0);
            }
            ENDCG
        }
    }
}
