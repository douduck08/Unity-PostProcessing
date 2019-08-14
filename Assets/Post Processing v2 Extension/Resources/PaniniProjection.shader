Shader "Hidden/PaniniProjection" {
    SubShader {
        Cull Off ZWrite Off ZTest Always
        Pass {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag

            // #include "UnityCg.cginc"
            #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

            // sampler2D _MainTex;
            TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

            // float4x4 _InverseProjectionMatrix; // use to calculate viewDir
            float _Distance;
            float _Scale;

            float2 PaniniForward (float2 direction, float d) {
                float xzInvLength = rsqrt(1.0 + direction.x * direction.x);
                float cosPhi = xzInvLength;
                float sinPhi = direction.x * xzInvLength;
                float tanTheta = direction.y * xzInvLength;
                float S = (d + 1.0) / (d + cosPhi);
                return S * float2(sinPhi, tanTheta / cosPhi);
            }

            float2 PaniniInverse (float2 xy, float d) {
                float k = xy.x * xy.x / ((d + 1) * (d + 1));
                float dscr = k * k * d * d - (k + 1) * (k * d * d - 1);
                float cosPhi = (-k * d + sqrt(dscr)) / (k + 1);
                float S = (d + 1.0) / (d + cosPhi);

                float phi = atan2(xy.x, S * cosPhi);
                float theta = atan2(xy.y / cosPhi, S);
                return float2(tan(phi), tan(theta));
            }

            half4 frag (VaryingsDefault i) : SV_Target {
                // float2 uv = PaniniForward(-1 * i.viewDir.xy / i.viewDir.z) * 0.5 + 0.5;
                float2 uv = PaniniInverse(i.texcoord * 2 - 1, _Distance) / _Scale * 0.5 + 0.5;

                // check edge
                // if (uv.x > 1 || uv.x < 0 || uv.y > 1 || uv.y < 0 ) {
                    //     return 0;
                // }
                
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                return col;
            }
            ENDHLSL
        }
    }
}
