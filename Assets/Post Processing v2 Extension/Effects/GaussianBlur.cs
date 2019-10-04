using System;

namespace UnityEngine.Rendering.PostProcessing {
    [Serializable]
    [PostProcess (typeof (GaussianBlurRenderer), PostProcessEvent.AfterStack, "Custom/Gaussian Blur")]
    public sealed class GaussianBlur : PostProcessEffectSettings {
        [Range (0, 6)] public IntParameter downsample = new IntParameter () { value = 0 };
        [Range (1f, 10f)] public FloatParameter radius = new FloatParameter () { value = 1f };
        [Range (1, 25)] public IntParameter iterations = new IntParameter () { value = 1 };
    }

    public sealed class GaussianBlurRenderer : PostProcessEffectRenderer<GaussianBlur> {

        Shader shader;
        int[] nameIds;

        public override void Init () {
            shader = Shader.Find ("Hidden/GaussianBlur");
            nameIds = new [] {
                Shader.PropertyToID ("_Temp1"),
                Shader.PropertyToID ("_Temp2")
            };
        }

        public override void Render (PostProcessRenderContext context) {
            var sheet = context.propertySheets.Get (shader);
            sheet.properties.SetFloat ("_Radius", settings.radius / (1 << settings.downsample));

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
                cmd.BlitFullscreenTriangle (nameIds[0], nameIds[1], sheet, 1);

                for (int i = 2; i < iterations; i++) {
                    cmd.BlitFullscreenTriangle (nameIds[1], nameIds[0], sheet, 0);
                    cmd.BlitFullscreenTriangle (nameIds[0], nameIds[1], sheet, 1);
                }

                cmd.BlitFullscreenTriangle (nameIds[1], nameIds[0], sheet, 0);
                cmd.BlitFullscreenTriangle (nameIds[0], context.destination, sheet, 1);
            } else {
                cmd.BlitFullscreenTriangle (downsample ? nameIds[1] : context.source, nameIds[0], sheet, 0);
                cmd.BlitFullscreenTriangle (nameIds[0], context.destination, sheet, 1);
            }

            cmd.ReleaseTemporaryRT (nameIds[0]);
            cmd.ReleaseTemporaryRT (nameIds[1]);
        }
    }
}