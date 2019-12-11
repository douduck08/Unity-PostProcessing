Shader "Hidden/Outline" {
    SubShader {
        Cull Off ZWrite Off ZTest Always

        HLSLINCLUDE
        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        TEXTURE2D_SAMPLER2D(_OutlineMaskTex, sampler_OutlineMaskTex);
        TEXTURE2D_SAMPLER2D(_BluredMaskTex, sampler_BluredMaskTex);
        half4 _OutlineColor;

        ENDHLSL

        Pass {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag

            // flat color
            half4 frag (VaryingsDefault i) : SV_Target {
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
                color = step(0.00001, dot(color, half4(1,1,1,1)));
                return color;
            }
            ENDHLSL
        }

        Pass {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag

            // render outline
            half4 frag (VaryingsDefault i) : SV_Target {
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
                half mask = SAMPLE_TEXTURE2D(_OutlineMaskTex, sampler_OutlineMaskTex, i.texcoord).r;
                half outline = SAMPLE_TEXTURE2D(_BluredMaskTex, sampler_BluredMaskTex, i.texcoord).r * _OutlineColor.a * (1.0 - mask);
                color.rgb = _OutlineColor.rgb * outline + color.rgb * (1 - outline);
                return color;
            }
            ENDHLSL
        }
    }
}
