Shader "Hidden/KawaseBlur" {
    SubShader {
        Cull Off ZWrite Off ZTest Always

        HLSLINCLUDE
        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float4 _MainTex_TexelSize;

        // static const half Iteration[5] = { 0, 1, 2, 3, 4 }; // by Chris Oat, ATI Research.
        static const half Iteration[5] = { 0, 1, 2, 2, 3 }; // by Filip Strugar, Intel. similar to 35x35 Gauss blur

        half4 KawaseBlur(half2 uv, half iteration) {
            half2 offset = _MainTex_TexelSize.xy * (iteration + 0.5);
            half2 offset2 = half2(offset.x, -offset.y);

            half4 color = 0;
            color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + offset);
            color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - offset);
            color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + offset2);
            color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - offset2);
            return color * 0.25;
        }

        ENDHLSL

        Pass {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag

            half4 frag (VaryingsDefault i) : SV_Target {
                return KawaseBlur(i.texcoord, Iteration[0]);
            }
            ENDHLSL
        }

        Pass {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag

            half4 frag (VaryingsDefault i) : SV_Target {
                return KawaseBlur(i.texcoord, Iteration[1]);
            }
            ENDHLSL
        }

        Pass {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag

            half4 frag (VaryingsDefault i) : SV_Target {
                return KawaseBlur(i.texcoord, Iteration[2]);
            }
            ENDHLSL
        }

        Pass {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag

            half4 frag (VaryingsDefault i) : SV_Target {
                return KawaseBlur(i.texcoord, Iteration[3]);
            }
            ENDHLSL
        }

        Pass {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag

            half4 frag (VaryingsDefault i) : SV_Target {
                return KawaseBlur(i.texcoord, Iteration[4]);
            }
            ENDHLSL
        }
    }
}
