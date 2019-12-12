Shader "Hidden/DepthCopy" {
    SubShader {
        Cull Off ZWrite Off ZTest Always

        HLSLINCLUDE
        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
        ENDHLSL

        // https://support.unity3d.com/hc/en-us/articles/115000229323-Graphics-Blit-does-not-copy-RenderTexture-depth
        // 0 - Camera Depth copy with procedural draw
        Pass {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag

            half4 frag(VaryingsDefault i, out float outDepth : SV_Depth) : SV_Target {
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoordStereo);
                outDepth = depth;
                return depth;
            }
            ENDHLSL
        }

        // 1 - Depth copy with procedural draw
        Pass {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag

            half4 frag(VaryingsDefault i, out float outDepth : SV_Depth) : SV_Target {
                float depth = SAMPLE_DEPTH_TEXTURE(_MainTex, sampler_MainTex, i.texcoordStereo);
                outDepth = depth;
                return depth;
            }
            ENDHLSL
        }
    }
}