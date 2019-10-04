using System;

namespace UnityEngine.Rendering.PostProcessing {
    [Serializable]
    [PostProcess (typeof (KawaseBlurRenderer), PostProcessEvent.AfterStack, "Custom/Kawase Blur")]
    public sealed class KawaseBlur : PostProcessEffectSettings {
        [Range (0, 6)] public IntParameter downsample = new IntParameter () { value = 0 };
        [Range (1, 5)] public IntParameter iterations = new IntParameter () { value = 1 };
    }

    public sealed class KawaseBlurRenderer : PostProcessEffectRenderer<KawaseBlur> {

        Shader shader;
        int[] nameIds;

        public override void Init () {
            shader = Shader.Find ("Hidden/KawaseBlur");
            nameIds = new [] {
                Shader.PropertyToID ("_Temp1"),
                Shader.PropertyToID ("_Temp2")
            };
        }

        public override void Render (PostProcessRenderContext context) {
            var sheet = context.propertySheets.Get (shader);

            var width = context.width >> settings.downsample;
            var height = context.height >> settings.downsample;
            var cmd = context.command;
            context.GetScreenSpaceTemporaryRT (cmd, nameIds[0], 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, width, height);
            context.GetScreenSpaceTemporaryRT (cmd, nameIds[1], 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, width, height);

            var downsample = settings.downsample > 0;
            if (downsample) {
                cmd.BlitFullscreenTriangle (context.source, nameIds[1]);
            }

            var iterations = settings.iterations;
            if (iterations > 1) {
                cmd.BlitFullscreenTriangle (downsample ? nameIds[1] : context.source, nameIds[0], sheet, 0);
                for (int i = 2; i < iterations; i++) {
                    cmd.BlitFullscreenTriangle (nameIds[i % 2], nameIds[(i + 1) % 2], sheet, i - 1);
                }
                cmd.BlitFullscreenTriangle (nameIds[iterations % 2], context.destination, sheet, iterations - 1);
            } else {
                cmd.BlitFullscreenTriangle (downsample ? nameIds[1] : context.source, context.destination, sheet, 0);
            }

            cmd.ReleaseTemporaryRT (nameIds[0]);
            cmd.ReleaseTemporaryRT (nameIds[1]);
        }
    }
}