﻿Shader "Custom/Portal"
{
    Properties
    {
        _InactiveColor ("Inactive Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _InactiveColor; //비활성화 색상
            int DisplayMask; //1이면 포탈 0이면 비활성화 색상
            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.screenPos.xy / i.screenPos.w;
                fixed4 portalCol = tex2D(_MainTex, uv);
                return portalCol * DisplayMask + _InactiveColor * (1-DisplayMask);
            }
            ENDCG
        }
    }
    Fallback "Standard" // for shadows
}
