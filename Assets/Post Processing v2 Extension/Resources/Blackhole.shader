Shader "Hidden/Blackhole" {
    SubShader {
        Cull Off ZWrite Off ZTest Always
        Pass {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag

            #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

            TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
            float4 _MainTex_TexelSize;

            float2 _Center;
            float _Radias;
            float _Range;
            float _Strength;

            static const float halfPI = 1.57079632679;

            half4 frag (VaryingsDefault i) : SV_Target {
                float2 viewPos = _MainTex_TexelSize.zw * (i.texcoord - 0.5);

                float2 direction = _Center - viewPos;
                float ratio = (_Radias * _Radias) / dot(direction, direction);
                if (ratio > 1){
                    return 0;
                }

                viewPos += _Strength * tan(halfPI * pow(ratio, 1.0 / _Range)) * normalize(direction);

                float2 uv = viewPos * _MainTex_TexelSize.xy + 0.5;
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                return color;
            }
            ENDHLSL
        }
    }
}
