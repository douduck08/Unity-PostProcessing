Shader "Hidden/Noise" {
    SubShader {
        Cull Off ZWrite Off ZTest Always
        Pass {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag

            #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

            TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
            float4 _MainTex_TexelSize;

            float _Scale;

            float Hash (float2 seed) {
                return frac(sin(dot(seed, float2(12.9898, 78.233))) * 43758.5453);
            }

            half4 frag (VaryingsDefault i) : SV_Target {
                float2 uv = i.texcoord * _Scale;
                half4 color = Hash(uv);
                return color;
            }
            ENDHLSL
        }
    }
}
