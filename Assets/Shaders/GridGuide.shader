Shader "Unlit/GridGuide"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent"  "Queue"="Transparent"  "DisableBatching" = "True"}
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
            fixed4 _PuckPosition;
            fixed _PuckTargetOpacity;
            fixed4 _PaddlePosition;
            matrix _QuadAdjust;

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex = mul(_QuadAdjust, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = mul(_QuadAdjust, v.normal);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.posObj = v.vertex;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                float gridX = sin(60 * i.posObj.x);
                float gridY = sin(60 * i.posObj.y);
                float gridZ = sin(60 * i.posObj.z);

                float grid
                    = max(max(gridX * (1 - abs(i.normal.x)), 
                    gridY * (1 - abs(i.normal.y))),
                    gridZ * (1 - abs(i.normal.z)));

                float hightlightX = max(abs(i.posObj.y - _PaddlePosition.y), abs(i.posObj.z - _PaddlePosition.z));
                float hightlightY = max(abs(i.posObj.x - _PaddlePosition.x), abs(i.posObj.z - _PaddlePosition.z));
                float hightlightZ = max(abs(i.posObj.x - _PaddlePosition.x), abs(i.posObj.y - _PaddlePosition.y));
                float backFade;
                backFade = 3 + .5*sin(2 * 3.1415926 * (.25*_Time.y + 0));
                hightlightX = max(smoothstep(0, 0.9, 1 - 6 * hightlightX), .05 * (1 - backFade * hightlightX));
                hightlightY = max(smoothstep(0, 0.9, 1 - 6 * hightlightY), .05 * (1 - backFade * hightlightY));
                hightlightZ = max(smoothstep(0, 0.9, 1 - 6 * hightlightZ), .05 * (1 - backFade * hightlightZ));
                float highlight
                    = abs(i.normal.x) * hightlightX
                    + abs(i.normal.y) * hightlightY
                    + abs(i.normal.z) * hightlightZ;

                float puckDeltaX = _PuckPosition.x - i.posObj.x;
                float puckDeltaY = _PuckPosition.y - i.posObj.y;
                float puckDeltaZ = _PuckPosition.z - i.posObj.z;
                puckDeltaX *= puckDeltaX;
                puckDeltaY *= puckDeltaY;
                puckDeltaZ *= puckDeltaZ;
                float puckDistX = sqrt(puckDeltaY + puckDeltaZ);
                float puckDistY = sqrt(puckDeltaX + puckDeltaZ);
                float puckDistZ = sqrt(puckDeltaX + puckDeltaY);
                float puckTargetX = (1 - .9 * puckDistX) * sin(30 * puckDistX - 3 * _Time.y);
                float puckTargetY = (1 - .9 * puckDistY) * sin(30 * puckDistY - 3 * _Time.y);
                float puckTargetZ = (1 - .9 * puckDistZ) * sin(30 * puckDistZ - 3 * _Time.y);
                float puckTarget
                    = abs(i.normal.x) * puckTargetX
                    + abs(i.normal.y) * puckTargetY
                    + abs(i.normal.z) * puckTargetZ;
                puckTarget *= _PuckTargetOpacity;
                float puckTargetPow = puckTarget * puckTarget;
                puckTargetPow = puckTargetPow * puckTargetPow;
                puckTargetPow = puckTargetPow * puckTargetPow;
                puckTargetPow = puckTargetPow * puckTargetPow;
                puckTargetPow = puckTargetPow * puckTargetPow;

                //grid = max(grid, puckTarget);

                float gridPow = grid * grid;
                gridPow = gridPow * gridPow;
                gridPow = gridPow * gridPow;
                gridPow = gridPow * gridPow;
                gridPow = gridPow * gridPow;
                col = float4(.1 * gridPow * gridPow, .6 * gridPow, gridPow, 0);
                col = (.9 * highlight)* col;

                col.rgb = max(col.rgb, float3(puckTargetPow, puckTargetPow, .2 * puckTargetPow * puckTargetPow));

                //UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
