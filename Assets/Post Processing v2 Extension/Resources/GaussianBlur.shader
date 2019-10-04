Shader "Hidden/GaussianBlur" {
    SubShader {
        Cull Off ZWrite Off ZTest Always

        HLSLINCLUDE
        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float4 _MainTex_TexelSize;
        float _Radius;
        
        // static const half GaussWeight[5] = { 0.06136, 0.24477, 0.38774, 0.24477, 0.06136 };
        static const half GaussWeight[7] = { 0.00598, 0.060626, 0.241843, 0.383103, 0.241843, 0.060626, 0.00598 };
        ENDHLSL

        Pass {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag

            half4 frag (VaryingsDefault i) : SV_Target {
                half2 offset = half2(_MainTex_TexelSize.x, 0) * _Radius;
                half2 uv = i.texcoord - offset * 3;

                half4 color = 0;
                for (int idx = 0; idx < 7; idx++) {
                    color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * GaussWeight[idx];
                    uv += offset;
                }
                
                return color;
            }
            ENDHLSL
        }

        Pass {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag

            half4 frag (VaryingsDefault i) : SV_Target {
                half2 offset = half2(0, _MainTex_TexelSize.y) * _Radius;
                half2 uv = i.texcoord - offset * 3;

                half4 color = 0;
                for (int idx = 0; idx < 7; idx++) {
                    color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * GaussWeight[idx];
                    uv += offset;
                }
                
                return color;
            }
            ENDHLSL
        }
    }
}
