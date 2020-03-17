Shader "Hidden/VisualizeWorldPosition" {
    SubShader {
        Cull Off ZWrite Off ZTest Always
        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // #include "UnityCg.cginc"
            #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

            struct Varyings {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float2 texcoordStereo : TEXCOORD1;
                float3 ray : TEXCOORD2;
                #if STEREO_INSTANCING_ENABLED
                uint stereoTargetEyeIndex : SV_RenderTargetArrayIndex;
                #endif
            };

            TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
            TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);

            float4x4 unity_CameraInvProjection;
            float4x4 _InverseView;
            float _Grid;
            float _Width;

            float3 SetupRay(float3 vpos) {
                // Render settings
                float far = _ProjectionParams.z;
                float2 orthoSize = unity_OrthoParams.xy;
                float isOrtho = unity_OrthoParams.w; // 0: perspective, 1: orthographic

                // Perspective: view space vertex position of the far plane
                float3 rayPers = mul(unity_CameraInvProjection, vpos.xyzz * far).xyz;

                // Orthographic: view space vertex position
                float3 rayOrtho = float3(orthoSize * vpos.xy, 0);

                return lerp(rayPers, rayOrtho, isOrtho);;
            }

            float3 ComputeViewSpacePosition(Varyings input) {
                // Render settings
                float near = _ProjectionParams.y;
                float far = _ProjectionParams.z;
                float isOrtho = unity_OrthoParams.w; // 0: perspective, 1: orthographic

                // Z buffer sample
                float z = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, input.texcoord);

                // Far plane exclusion
                #if !defined(EXCLUDE_FAR_PLANE)
                float mask = 1;
                #elif defined(UNITY_REVERSED_Z)
                float mask = z > 0;
                #else
                float mask = z < 1;
                #endif

                // Perspective: view space position = ray * depth
                float3 vposPers = input.ray * Linear01Depth(z);

                // Orthographic: linear depth (with reverse-Z support)
                #if defined(UNITY_REVERSED_Z)
                float depthOrtho = -lerp(far, near, z);
                #else
                float depthOrtho = -lerp(near, far, z);
                #endif

                // Orthographic: view space position
                float3 vposOrtho = float3(input.ray.xy, depthOrtho);

                // Result: view space position
                return lerp(vposPers, vposOrtho, isOrtho) * mask;
            }

            Varyings vert(AttributesDefault v) {
                Varyings o;
                o.vertex = float4(v.vertex.xy, 1, 1); // in post-prpcessing stack v2, full triangle vertex pos: (-1,-1) (3,-1) (-1,3)
                o.texcoord = TransformTriangleVertexToUV(v.vertex.xy);

                #if UNITY_UV_STARTS_AT_TOP
                o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
                #endif

                o.texcoordStereo = TransformStereoScreenSpaceTex(o.texcoord, 1.0);

                float3 vpos = float3(v.vertex.x, -v.vertex.y, 1); // vpos = full triangle vertex pos: (-1,1) (3,1) (-1,-3)
                o.ray = SetupRay(vpos);
                return o;
            }

            half4 frag (Varyings i) : SV_Target {
                float3 vpos = ComputeViewSpacePosition(i);
                float3 wpos = mul(_InverseView, float4(vpos, 1)).xyz;

                wpos *= _Grid;
                float3 fw = fwidth(wpos);
                half3 grid = saturate(_Width - abs(0.5 - frac(wpos)) / fw);

                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
                c.rgb *= grid;
                return c;
            }
            ENDHLSL
        }
    }
}
