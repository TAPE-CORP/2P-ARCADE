Shader "Custom/ScrollingLit2D"
{
    Properties
    {
        _MainTex("Base Texture", 2D) = "white" {}
        _MaskTex("Mask Texture", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _ScrollSpeed("Scroll Speed (UV/sec)", Vector) = (0.5, 0, 0, 0)
    }
    SubShader
    {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }
        LOD 200

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "Universal2D"
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

            // 텍스처 & 프로퍼티
            TEXTURE2D(_MainTex);    SAMPLER(sampler_MainTex);
            TEXTURE2D(_MaskTex);    SAMPLER(sampler_MaskTex);
            TEXTURE2D(_NormalMap);  SAMPLER(sampler_NormalMap);
            float4 _MainTex_ST;
            float4 _MaskTex_ST;
            float4 _NormalMap_ST;
            float4 _BaseColor;
            float2 _ScrollSpeed;
            float4 _RendererColor; // SpriteRenderer.tint

            struct Attributes
            {
                float3 positionOS : POSITION;
                float4 color      : COLOR;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS  : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float2 maskUV      : TEXCOORD1;
                float2 normalUV    : TEXCOORD2;
                float4 color       : COLOR;
                half2 lightingUV   : TEXCOORD3;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                // 클립 공간 위치
                OUT.positionCS = TransformObjectToHClip(IN.positionOS);

                // 컬러(스프라이트 Tint 포함)
                OUT.color = IN.color * _BaseColor * _RendererColor;

                // UV 스크롤
                float2 offset = _ScrollSpeed * _Time.y;
                OUT.uv       = TRANSFORM_TEX(IN.uv, _MainTex)   + offset;
                OUT.maskUV   = TRANSFORM_TEX(IN.uv, _MaskTex)   + offset;
                OUT.normalUV = TRANSFORM_TEX(IN.uv, _NormalMap) + offset;

                // 2D 라이트용 스크린 UV
                OUT.lightingUV = half2(ComputeScreenPos(OUT.positionCS / OUT.positionCS.w).xy);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 텍스처 샘플링
                half4 baseCol   = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half4 maskCol   = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, IN.maskUV);
                half4 normalCol = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, IN.normalUV);

                baseCol *= IN.color;
                maskCol *= IN.color;

                // 서피스/입력 데이터 초기화
                SurfaceData2D surfaceData;
                InputData2D   inputData;
                InitializeSurfaceData(baseCol.rgb, baseCol.a, surfaceData);
                InitializeInputData(IN.uv, inputData);

                // 마스크 및 노멀맵 연결
                inputData.mask      = maskCol;
                inputData.normalMap = normalCol;
                inputData.screenPos = IN.lightingUV;

                // 2D 라이트 계산 적용
                half4 lit = CombinedShapeLightShared(surfaceData, inputData);

                return lit;
            }
            ENDHLSL
        }
    }
    FallBack "Sprites/Default"
}
