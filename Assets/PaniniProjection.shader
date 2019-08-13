Shader "Hidden/PaniniProjection" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
        Cull Off ZWrite Off ZTest Always
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4x4 _InverseProjectionMatrix;
            float _Scale;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                float4 viewDir = mul(_InverseProjectionMatrix, float4(2.0 * v.uv - 1.0, 0.0, 1.0));
                o.viewDir = viewDir.xyz;
                return o;
            }

            float2 PaniniForward (float2 direction) {
                const float d = 1.0;
                float xzInvLength = rsqrt(1.0 + direction.x * direction.x);
                float cosPhi = xzInvLength;
                float sinPhi = direction.x * xzInvLength;
                float tanTheta = direction.y * xzInvLength;
                float S = (d + 1.0) / (d + cosPhi);
                return S * float2(sinPhi, tanTheta / cosPhi);
            }

            float2 PaniniInverse (float2 xy) {
                const float d = 1.0;
                float k = xy.x * xy.x / ((d + 1) * (d + 1));
                float dscr = k * k * d * d - (k + 1) * (k * d * d - 1);
                float cosPhi = (-k * d + sqrt(dscr)) / (k + 1);
                float S = (d + 1.0) / (d + cosPhi);

                float phi = atan2(xy.x, S * cosPhi);
                float theta = atan2(xy.y / cosPhi, S);
                return float2(tan(phi), tan(theta));
            }

            half4 frag (v2f i) : SV_Target {
                // float2 uv = PaniniForward(-1 * i.viewDir.xy / i.viewDir.z) * 0.5 + 0.5;
                float2 uv = PaniniInverse(i.uv * 2 - 1) / _Scale * 0.5 + 0.5;
                if (uv.x > 1 || uv.x < 0 || uv.y > 1 || uv.y < 0 ) {
                    return 0;
                }
                
                half4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDCG
        }
    }
}
