using System;

namespace UnityEngine.Rendering.PostProcessing {
    [Serializable]
    [PostProcess (typeof (HueShiftRenderer), PostProcessEvent.AfterStack, "Custom/Hue Shift")]
    public sealed class HueShift : PostProcessEffectSettings {
        [Range (0f, 1f)] public FloatParameter percentage = new FloatParameter () { value = 0f };
    }

    public sealed class HueShiftRenderer : PostProcessEffectRenderer<HueShift> {

        Shader shader;

        public override void Init () {
            shader = Shader.Find ("Hidden/HueShift");
        }

        public override void Render (PostProcessRenderContext context) {
            var sheet = context.propertySheets.Get (shader);
            sheet.properties.SetFloat ("_Scalar", settings.percentage);

            var cmd = context.command;
            cmd.BeginSample ("HueShift");
            cmd.BlitFullscreenTriangle (context.source, context.destination, sheet, 0);
            cmd.EndSample ("HueShift");
        }
    }
}