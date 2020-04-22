Shader "Hidden/HueChange" {
    SubShader {
        Cull Off ZWrite Off ZTest Always
        Pass {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag

            #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

            TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
            float4 _MainTex_TexelSize;

            float3 _Color;

            // ref: https://cmwdexint.com/2015/02/03/awkward-but-a-cheaper-option-instead-of-hue-shift-shader/
            float3 HueChange(float3 color, float3 factor){
                return float3(
                color.r - factor.r * (2 * color.r - color.g - color.b),
                color.g - factor.g * (2 * color.g - color.r - color.b),
                color.b - factor.b * (2 * color.b - color.r - color.g)
                );
            }

            half4 frag (VaryingsDefault i) : SV_Target {
                float2 uv = i.texcoord;
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

                color.rgb = HueChange(color.rgb, _Color);
                return color;
            }
            ENDHLSL
        }
    }
}
