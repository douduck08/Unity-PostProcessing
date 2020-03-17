using System;

namespace UnityEngine.Rendering.PostProcessing {
    [Serializable]
    [PostProcess (typeof (VisualizeWorldPositionRenderer), PostProcessEvent.AfterStack, "Custom/Visualize World Position")]
    public sealed class VisualizeWorldPosition : PostProcessEffectSettings {
        public FloatParameter grid = new FloatParameter () { value = 1f };
        public FloatParameter width = new FloatParameter () { value = 1f };
    }

    public sealed class VisualizeWorldPositionRenderer : PostProcessEffectRenderer<VisualizeWorldPosition> {

        Shader shader;

        public override void Init () {
            shader = Shader.Find ("Hidden/VisualizeWorldPosition");
        }

        public override void Render (PostProcessRenderContext context) {
            var sheet = context.propertySheets.Get (shader);
            sheet.properties.SetFloat ("_Grid", Mathf.Max (0f, settings.grid));
            sheet.properties.SetFloat ("_Width", Mathf.Max (0f, settings.width));
            sheet.properties.SetMatrix ("_InverseView", context.camera.cameraToWorldMatrix);

            var cmd = context.command;
            cmd.BeginSample ("Visualize World Position");
            cmd.BlitFullscreenTriangle (context.source, context.destination, sheet, 0);
            cmd.EndSample ("Visualize World Position");
        }
    }
}