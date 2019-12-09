using System;

namespace UnityEngine.Rendering.PostProcessing {
    [Serializable]
    [PostProcess (typeof (BlackholeRenderer), PostProcessEvent.AfterStack, "Toys/Blackhole")]
    public sealed class Blackhole : PostProcessEffectSettings {
        public Vector2Parameter center = new Vector2Parameter () { value = new Vector2 () };
        public FloatParameter radius = new FloatParameter () { value = 100f };
        public FloatParameter distortionRange = new FloatParameter () { value = 1f };
        public FloatParameter distortionStrength = new FloatParameter () { value = 1f };
    }

    public sealed class BlackholeRenderer : PostProcessEffectRenderer<Blackhole> {

        Shader shader;

        public override void Init () {
            shader = Shader.Find ("Hidden/Blackhole");
        }

        public override void Render (PostProcessRenderContext context) {
            var sheet = context.propertySheets.Get (shader);
            sheet.properties.SetVector ("_Center", settings.center);
            sheet.properties.SetFloat ("_Radias", Mathf.Max (0.01f, settings.radius));
            sheet.properties.SetFloat ("_Range", Mathf.Max (0.01f, settings.distortionRange));
            sheet.properties.SetFloat ("_Strength", settings.distortionStrength);

            var cmd = context.command;
            cmd.BeginSample ("Blackhole");
            cmd.BlitFullscreenTriangle (context.source, context.destination, sheet, 0);
            cmd.EndSample ("Blackhole");
        }
    }
}