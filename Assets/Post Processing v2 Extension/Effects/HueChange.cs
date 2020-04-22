using System;

namespace UnityEngine.Rendering.PostProcessing {
    [Serializable]
    [PostProcess (typeof (HueChangeRenderer), PostProcessEvent.AfterStack, "Toys/Hue Change")]
    public sealed class HueChange : PostProcessEffectSettings {
        public ColorParameter color = new ColorParameter () { value = new Color (1, 1, 1, 1) };
    }

    public sealed class HueChangeRenderer : PostProcessEffectRenderer<HueChange> {

        Shader shader;

        public override void Init () {
            shader = Shader.Find ("Hidden/HueChange");
        }

        public override void Render (PostProcessRenderContext context) {
            var sheet = context.propertySheets.Get (shader);
            sheet.properties.SetColor ("_Color", settings.color);

            var cmd = context.command;
            cmd.BeginSample ("HueChange");
            cmd.BlitFullscreenTriangle (context.source, context.destination, sheet, 0);
            cmd.EndSample ("HueChange");
        }
    }
}