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
                float4 color : COLOR;
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                fixed4 color : COLOR;
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
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                // 圆形遮罩
                float distToCenter = distance(i.worldPos.xy, _MaskInfo.xy);
                float circleMask = step(_MaskInfo.z, distToCenter);

                // 矩形遮罩
                float2 rectCenter = _MaskInfo.xy;
                float2 halfSize = _MaskInfo.zw;
                float2 dis = abs(i.worldPos.xy - rectCenter);
                float rectMask = step(halfSize.x, dis.x) || step(halfSize.y, dis.y);

                // 根据_MaskType选择哪个遮罩
                float maskAlpha = 1.0;
                if (_MaskType == 0) // 圆形遮罩
                {
                    maskAlpha = circleMask;
                }
                else if (_MaskType == 1) // 矩形遮罩
                {
                    maskAlpha = rectMask;
                }
                else // 图形遮罩
                {
                    float2 remapUV = (i.worldPos.xy - _MaskInfo.xy) * _MaskTex_TexelSize.xy / _MaskInfo.z + 0.5;
                    if (all(remapUV >= 0) && all(remapUV <= 1))
                    {
                        maskAlpha = 1 - tex2D(_MaskTex, remapUV).a;
                    }
                }

                // 应用遮罩
                col.a *= maskAlpha;

                return col;
            }
            ENDCG
        }
    }
}