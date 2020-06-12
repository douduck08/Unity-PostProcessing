using System;

namespace UnityEngine.Rendering.PostProcessing {
    [Serializable]
    [PostProcess (typeof (NoiseRenderer), PostProcessEvent.AfterStack, "Toys/Noise")]
    public sealed class Noise : PostProcessEffectSettings {
        public FloatParameter scale = new FloatParameter () { value = 1f };
    }

    public sealed class NoiseRenderer : PostProcessEffectRenderer<Noise> {

        Shader shader;

        public override void Init () {
            shader = Shader.Find ("Hidden/Noise");
        }

        public override void Render (PostProcessRenderContext context) {
            var sheet = context.propertySheets.Get (shader);
            sheet.properties.SetFloat ("_Scale", settings.scale);

            var cmd = context.command;
            cmd.BeginSample ("Noise");
            cmd.BlitFullscreenTriangle (context.source, context.destination, sheet, 0);
            cmd.EndSample ("Noise");
        }
    }
}