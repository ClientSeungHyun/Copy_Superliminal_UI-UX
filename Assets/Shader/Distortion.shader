Shader "Custom/Distortion"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _DistortionStrength("Distortion Strength", Range(0, 1)) = 0.3
        _NoiseScale("Noise Scale", Float) = 100.0
        _Speed("Noise Speed", Float) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "DistortionPass"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            float _NoiseScale;
            float _Speed;
            float _DistortionStrength;
            sampler2D _MainTex;
            sampler2D _CameraOpaqueTexture;

            // Gradient noise function
            float random(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            float gradientNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);

                float a = random(i);
                float b = random(i + float2(1.0, 0.0));
                float c = random(i + float2(0.0, 1.0));
                float d = random(i + float2(1.0, 1.0));

                float2 u = f * f * (3.0 - 2.0 * f);

                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = IN.uv;
                OUT.screenPos = OUT.positionCS;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {

                float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
                screenUV = screenUV * 0.5 + 0.5;
                screenUV.y = 1 - screenUV.y;

                float time = _Time.y * _Speed;

                float noise = gradientNoise(screenUV * _NoiseScale );

                float2 distortion = (noise - 0.5) * _DistortionStrength;

                float4 sceneColor = tex2D(_CameraOpaqueTexture, screenUV + distortion);

                return sceneColor;
            }
            ENDHLSL
        }
    }
}
