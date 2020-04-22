Shader "Hidden/HueShift" {
    SubShader {
        Cull Off ZWrite Off ZTest Always
        Pass {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag

            #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

            TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
            float4 _MainTex_TexelSize;

            float _Scalar;

            // ref: https://gist.github.com/mairod/a75e7b44f68110e1576d77419d608786
            float3 FastHueShift(float3 color, float hueAdjust) {
                const float3 k = float3(0.57735, 0.57735, 0.57735);
                half cosAngle = cos(hueAdjust);
                return color * cosAngle + cross(k, color) * sin(hueAdjust) + k * dot(k, color) * (1.0 - cosAngle);
            }

            // ref: https://docs.unity3d.com/Packages/com.unity.shadergraph@6.9/manual/Hue-Node.html
            float3 Unity_Hue_Radians_float(float3 color, float Offset) {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 P = lerp(float4(color.bg, K.wz), float4(color.gb, K.xy), step(color.b, color.g));
                float4 Q = lerp(float4(P.xyw, color.r), float4(color.r, P.yzx), step(P.x, color.r));
                float D = Q.x - min(Q.w, Q.y);
                float E = 1e-10;
                float3 hsv = float3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);

                float hue = hsv.x + Offset;
                hsv.x = (hue < 0) ? hue + 1 : (hue > 1) ? hue - 1 : hue;

                // HSV to RGB
                float4 K2 = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 P2 = abs(frac(hsv.xxx + K2.xyz) * 6.0 - K2.www);
                return hsv.z * lerp(K2.xxx, saturate(P2 - K2.xxx), hsv.y);
            }

            half4 frag (VaryingsDefault i) : SV_Target {
                float2 uv = i.texcoord;
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

                const float twoPI = 6.28318530718;
                color.rgb = FastHueShift(color.rgb, _Scalar * twoPI);
                // color.rgb = Unity_Hue_Radians_float(color.rgb, _Scalar);
                return color;
            }
            ENDHLSL
        }
    }
}
