Shader "Hidden/DualBlur" {
    SubShader {
        Cull Off ZWrite Off ZTest Always

        HLSLINCLUDE
        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float4 _MainTex_TexelSize;
        ENDHLSL

        Pass {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag

            // downsample
            half4 frag (VaryingsDefault i) : SV_Target {
                half2 halfPixel = _MainTex_TexelSize.xy * 0.5;
                half2 halfPixel2 = half2(halfPixel.x, -halfPixel.y);
                half2 uv = i.texcoord;
                
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * 4.0;
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + halfPixel);
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - halfPixel);
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + halfPixel2);
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - halfPixel2);
                return color / 8.0;
            }
            ENDHLSL
        }

        Pass {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag

            // upsample
            half4 frag (VaryingsDefault i) : SV_Target {
                half2 pixel = _MainTex_TexelSize.xy;
                half2 halfPixel = pixel * 0.5;
                half2 halfPixel2 = half2(halfPixel.x, -halfPixel.y);
                half2 uv = i.texcoord;

                half4 color = 0;
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(pixel.x, 0));
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - half2(pixel.x, 0));
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(0, pixel.y));
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - half2(0, pixel.y));

                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + halfPixel) * 2.0;
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - halfPixel) * 2.0;
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + halfPixel2) * 2.0;
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - halfPixel2) * 2.0;
                return color / 12.0;
            }
            ENDHLSL
        }
    }
}
