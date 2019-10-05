Shader "Hidden/CircleBlur" {
    SubShader {
        Cull Off ZWrite Off ZTest Always

        HLSLINCLUDE
        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float4 _MainTex_TexelSize;
        half _Scale;

        static const half downSinArray[14] = { 0.06407, 0.49072, 0.82017, 0.98718, 0.95867, 0.74028, 0.37527, -0.06407, -0.49072, -0.82017, -0.98718, -0.95867, -0.74028, -0.37527 };
        static const half downCosArray[14] = { 0.99795, 0.87131, 0.57212, 0.15960, -0.28452, -0.67230, -0.92692, -0.99795, -0.87131, -0.57212, -0.15960, 0.28452, 0.67230, 0.92692 };
        static const half upSinArray[7] = { 0.25365, 0.91441, 0.88660, 0.19116, -0.64823, -0.99949, -0.59811 };
        static const half upCosArray[7] = { 0.96729, 0.40478, -0.46254, -0.98156, -0.76145, 0.03205, 0.80141 };
        ENDHLSL

        Pass {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag

            // downsample
            half4 frag (VaryingsDefault i) : SV_Target {
                half2 uv = i.texcoord;
                half2 pixelSize = _MainTex_TexelSize.xy;
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                for (int i = 0; i < 14; i++) {
                    color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + pixelSize * half2(downSinArray[i], downCosArray[i]) * _Scale);
                }
                return color / 15.0;
            }
            ENDHLSL
        }

        Pass {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag

            // upsample
            half4 frag (VaryingsDefault i) : SV_Target {
                half2 uv = i.texcoord;
                half2 pixelSize = _MainTex_TexelSize.xy;
                half4 color = 0;
                for (int i = 0; i < 7; i++) {
                    color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + pixelSize * half2(upSinArray[i], upCosArray[i]) * _Scale);
                }
                return color / 7.0;
            }
            ENDHLSL
        }
    }
}
