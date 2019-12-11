Shader "Hidden/DepthCopy" {
    SubShader {
        Cull Off ZWrite Off ZTest Always

        HLSLINCLUDE
        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
        TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
        ENDHLSL

        // 0 - Depth copy with procedural draw
        Pass {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag

            float4 frag(VaryingsDefault i) : SV_Target {
                return SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoordStereo);
            }
            ENDHLSL
        }
    }
}