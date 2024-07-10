Shader "Unlit/WaterMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed("Speed",Float) = 0.1

    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue" = "Transparent"
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
                float3 normal:NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos:TEXCOORD1;
                float3 normalDir:TEXCOORD2;
                float3 objectPos:TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Speed;
            float _Value;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.normalDir = normalize(UnityObjectToWorldNormal(v.normal));
                // 物体模型空间中心点位置，转到世界坐标系下
                o.objectPos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 水痕是垂直于地面也就是Z轴，因此我们使用XY，YZ平面来进行采样，忽略XZ平面即地面
                float2 verticalUV1 = float2(i.objectPos.x - i.worldPos.x,
                                            i.objectPos.y - i.worldPos.y - _Time.y * _Speed);
                float2 verticalUV2 = float2(i.objectPos.z - i.worldPos.z,
                                             i.objectPos.y - i.worldPos.y - _Time.y * _Speed);

                float2 verticalUV3 = float2(i.objectPos.x - i.worldPos.x,
                                                i.objectPos.y - i.worldPos.y);
                float2 verticalUV4 = float2(i.objectPos.z - i.worldPos.z,
                                                      i.objectPos.y - i.worldPos.y);

                float4 col1 = tex2D(_MainTex,TRANSFORM_TEX(verticalUV1, _MainTex)).gggg;
                float4 maskCol1 = tex2D(_MainTex,TRANSFORM_TEX(verticalUV3, _MainTex)).bbbb;
                col1 *= maskCol1;

                float4 col2 = tex2D(_MainTex,TRANSFORM_TEX(verticalUV2, _MainTex)).gggg;
                float4 maskCol2 = tex2D(_MainTex,TRANSFORM_TEX(verticalUV4, _MainTex)).bbbb;
                col2 *= maskCol2;

                float4 finalCol = lerp(col1, col2, abs(i.normalDir.x));
                return finalCol;
            }
            ENDCG
        }
    }
}