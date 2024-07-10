Shader "Unlit/Rain"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed("Speed",Float) = 1.0
        _WaterMaskSpeed("WaterMaskSpeed",Float) =1.0
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 objectPos : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Speed;
            float _WaterMaskSpeed;
            float _Value;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.normalDir = normalize(UnityObjectToWorldNormal(v.normal));
                // 物体模型空间中心点位置，转到世界坐标系下
                o.objectPos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                //UV 也偏移一点，让两张图错开一些
                fixed4 col2 = tex2D(_MainTex, i.uv + 0.5);
                float3 emissive = frac(_Time.y * _Speed);
                //偏移一点时间，让涟漪扩散时间错开
                float3 emissive2 = frac(_Time.y * _Speed + 0.5);
                float raindrop1 = saturate(1 - distance(emissive.r - 1 + col.r, 0.05) / 0.05);
                float raindrop2 = saturate(1 - distance(emissive2.r - 1 + col2.r, 0.05) / 0.05);
                //两张图交替淡入
                float raindropFinalCol = lerp(raindrop1, raindrop2, abs(0.5 - emissive.r) * 2.0);

                // 水痕是垂直于地面也就是Z轴，因此我们使用XY，YZ平面来进行采样，忽略XZ平面即地面
                float2 verticalUV1 = float2(i.objectPos.x - i.posWorld.x,
                                            i.objectPos.y - i.posWorld.y);
                float2 verticalUV2 = float2(i.objectPos.z - i.posWorld.z,
                                            i.objectPos.y - i.posWorld.y);

                float2 verticalUV3 = float2(i.objectPos.x - i.posWorld.x,
                                            i.objectPos.y - i.posWorld.y - _Time.y * _WaterMaskSpeed);
                float2 verticalUV4 = float2(i.objectPos.z - i.posWorld.z,
                                            i.objectPos.y - i.posWorld.y - _Time.y * _WaterMaskSpeed);

                float4 waterCol1 = tex2D(_MainTex,TRANSFORM_TEX(verticalUV1, _MainTex)).gggg;
                float4 waterMaskCol1 = tex2D(_MainTex,TRANSFORM_TEX(verticalUV3, _MainTex)).bbbb;
                waterCol1 -= 1 - waterMaskCol1;

                float4 waterCol2 = tex2D(_MainTex,TRANSFORM_TEX(verticalUV2, _MainTex)).gggg;
                float4 waterMaskCol2 = tex2D(_MainTex,TRANSFORM_TEX(verticalUV4, _MainTex)).bbbb;
                waterCol2 -= 1 - waterMaskCol2;

                float4 waterFinCol = lerp(waterCol1, waterCol2, abs(i.normalDir.x));
                float4 finalCol = lerp(waterFinCol, raindropFinalCol, i.normalDir.y);
                return float4(finalCol.rrr, 1.0);
            }
            ENDCG
        }
    }
}