Shader "Unlit/GuideMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex("MaskTex",2D) = "white"{}
        _MaskType ("MaskType",Int) = 2
        _MaskInfo("MaskInfo",Vector) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue" = "Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha

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
                float3 worldPos:TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MaskTex;
            float4 _MaskTex_TexelSize;
            float4 _MaskInfo;
            int _MaskType;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                // 圆形遮罩
                if (_MaskType == 0)
                {
                    col.a *= distance(i.worldPos.xy, _MaskInfo.xy) > _MaskInfo.z;
                }
                else if (_MaskType == 1)
                {
                    // 矩形遮罩
                    float2 dis = i.worldPos.xy - _MaskInfo.xy;
                    col.a *= abs(dis.x) > _MaskInfo.z || abs(dis.y) > _MaskInfo.w;
                }
                else
                {
                    float halfWidth = 1 / _MaskTex_TexelSize.x / 2;
                    float halfHeight = 1 / _MaskTex_TexelSize.y / 2;

                    float remapU = 0.5 / halfWidth * i.worldPos.x + 0.5 - 0.5 * _MaskInfo.x / halfWidth;
                    float remapV = 0.5 / halfHeight * i.worldPos.y + 0.5 - 0.5 * _MaskInfo.y / halfHeight;

                    if (remapU >= 0 && remapU <= 1 && remapV >= 0 && remapV <= 1)
                    {
                        // 图形遮罩
                        float4 maskCol = tex2D(_MaskTex, float2(remapU, remapV));
                        col.a *= (1 - step(0.01, maskCol.a));
                    }
                }
                return col;
            }
            ENDCG
        }
    }
}