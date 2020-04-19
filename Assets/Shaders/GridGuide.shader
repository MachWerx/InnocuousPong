Shader "Unlit/GridGuide"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent"  "Queue"="Transparent"}
        LOD 100
            ZWrite Off
            Cull Off
            Blend One One
        Pass
        {
            CGPROGRAM


            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float4 posObj : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = v.normal;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.posObj = v.vertex;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                col *= fixed4(.02, .04, 1, 0);
                float gridX = smoothstep(0.7, 1.0, 2 * abs(abs((10 * i.posObj.x) % 1) - 0.5));
                float gridY = smoothstep(0.7, 1.0, 2 * abs(abs((10 * i.posObj.y) % 1) - 0.5));
                float gridZ = smoothstep(0.7, 1.0, 2 * abs(abs((10 * i.posObj.z) % 1) - 0.5));
                float grid
                    = gridX * (1 - abs(i.normal.x))
                    + gridY * (1 - abs(i.normal.y))
                    + gridZ * (1 - abs(i.normal.z));
                col *= 20*grid;

                //col.rgb = 0.5*i.normal + float3(0.5, 0.5, 0.5);

                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
