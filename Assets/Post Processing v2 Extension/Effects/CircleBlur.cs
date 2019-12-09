using System;

namespace UnityEngine.Rendering.PostProcessing {
    [Serializable]
    [PostProcess (typeof (CircleBlurRenderer), PostProcessEvent.AfterStack, "Custom/Circle Blur")]
    public sealed class CircleBlur : PostProcessEffectSettings {
        [Range (0, 6)] public IntParameter downsample = new IntParameter () { value = 3 };
        [Range (1f, 10f)] public FloatParameter scale = new FloatParameter () { value = 1f };
    }

    public sealed class CircleBlurRenderer : PostProcessEffectRenderer<CircleBlur> {

        Shader shader;
        int[] nameIds;

        public override void Init () {
            shader = Shader.Find ("Hidden/CircleBlur");
            nameIds = new [] {
                Shader.PropertyToID ("_Temp1"),
            };
        }

        public override void Render (PostProcessRenderContext context) {
            var sheet = context.propertySheets.Get (shader);
            sheet.properties.SetFloat ("_Scale", settings.scale);

            var downsample = settings.downsample;
            var width = context.width >> downsample;
            var height = context.height >> downsample;

            var cmd = context.command;
            cmd.BeginSample ("Circle Blur");
            context.GetScreenSpaceTemporaryRT (cmd, nameIds[0], 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, width, height);
            cmd.BlitFullscreenTriangle (context.source, nameIds[0], sheet, 0);
            cmd.BlitFullscreenTriangle (nameIds[0], context.destination, sheet, 1);
            cmd.ReleaseTemporaryRT (nameIds[0]);
            cmd.EndSample ("Circle Blur");
        }
    }
}