Shader "Unlit/WaterMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld,v.vertex);
                o.normalDir =normalize( UnityObjectToWorldNormal(v.normal));
                o.objectPos = mul(unity_ObjectToWorld,float4(0,0,0,1));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 verticalUV1 = float2(i.objectPos.x-i.worldPos.x,i.objectPos.y-i.worldPos.y);
                float2 verticalUV2 = float2(i.objectPos.z-i.worldPos.z,i.objectPos.y-i.worldPos.y);
                float4 col1 = tex2D(_MainTex,TRANSFORM_TEX(verticalUV1,_MainTex)).ggga;
                float4 col2 = tex2D(_MainTex,TRANSFORM_TEX(verticalUV2,_MainTex)).ggga;
                float4 finalCol = lerp(col1,col2,i.normalDir.x);
                return finalCol;
            }
            ENDCG
        }
    }
}
