Shader "Unlit/Rain"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed("Speed",Float) = 1.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Speed;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                //UV 也偏移一点，让两张图错开一些
                fixed4 col2 = tex2D(_MainTex, i.uv + float2(0.5, 0.5));
                float3 emissive = frac(_Time.y * _Speed);
                //偏移一点时间，让涟漪扩散时间错开
                float3 emissive2 = emissive + 0.5;
                float3 maskCol1 = saturate(1 - distance(emissive.r - (1 - col.r), 0.05) / 0.05);
                float3 maskCol2 = saturate(1 - distance(emissive2.r - (1 - col2.r), 0.05) / 0.05);
                float maskSwitch = saturate(abs(sin((_Time.y * 0.5)))); //两张图交替淡入
                float finalColor = lerp(maskCol1, maskCol2, maskSwitch);
                return float4(finalColor.rrr, 1.0);
                return col;
            }
            ENDCG
        }
    }
}