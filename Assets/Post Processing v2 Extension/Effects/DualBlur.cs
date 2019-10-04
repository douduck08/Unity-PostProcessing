using System;

namespace UnityEngine.Rendering.PostProcessing {
    [Serializable]
    [PostProcess (typeof (DualBlurRenderer), PostProcessEvent.AfterStack, "Custom/Dual Blur")]
    public sealed class DualBlur : PostProcessEffectSettings {
        // [Range (0, 6)] public IntParameter downsample = new IntParameter () { value = 0 };
        [Range (1, 5)] public IntParameter iterations = new IntParameter () { value = 1 };
    }

    public sealed class DualBlurRenderer : PostProcessEffectRenderer<DualBlur> {

        Shader shader;
        int[] nameIds;

        public override void Init () {
            shader = Shader.Find ("Hidden/DualBlur");
            nameIds = new [] {
                Shader.PropertyToID ("_Temp1"),
                Shader.PropertyToID ("_Temp2"),
                Shader.PropertyToID ("_Temp3"),
                Shader.PropertyToID ("_Temp4"),
                Shader.PropertyToID ("_Temp5")
            };
        }

        public override void Render (PostProcessRenderContext context) {
            var sheet = context.propertySheets.Get (shader);

            var cmd = context.command;
            var iterations = settings.iterations;
            for (int i = 0; i < iterations; i++) {
                var width = context.width >> (i + 1);
                var height = context.height >> (i + 1);
                context.GetScreenSpaceTemporaryRT (cmd, nameIds[i], 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, width, height);
            }

            // downsample
            cmd.BlitFullscreenTriangle (context.source, nameIds[0], sheet, 0);
            for (int i = 1; i < iterations; i++) {
                cmd.BlitFullscreenTriangle (nameIds[i - 1], nameIds[i], sheet, 0);
            }

            // upsample
            for (int i = iterations - 1; i >= 1; i--) {
                cmd.BlitFullscreenTriangle (nameIds[i], nameIds[i - 1], sheet, 1);
            }
            cmd.BlitFullscreenTriangle (nameIds[0], context.destination, sheet, 1);

            // release tempRT
            for (int i = 0; i < iterations; i++) {
                cmd.ReleaseTemporaryRT (nameIds[i]);
            }
        }
    }
}