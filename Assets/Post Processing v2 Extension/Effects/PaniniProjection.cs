using System;

namespace UnityEngine.Rendering.PostProcessing {
    [Serializable]
    [PostProcess (typeof (PaniniProjectionRenderer), PostProcessEvent.AfterStack, "Custom/Panini Projection")]
    public sealed class PaniniProjection : PostProcessEffectSettings {
        [Range (0.0f, 2.0f)] public FloatParameter projectionDistance = new FloatParameter () { value = 1f };
        [Range (1.0f, 2.0f)] public FloatParameter scale = new FloatParameter () { value = 1f };
    }

    public sealed class PaniniProjectionRenderer : PostProcessEffectRenderer<PaniniProjection> {

        Shader shader;

        public override void Init () {
            shader = Shader.Find ("Hidden/PaniniProjection");
        }

        public override void Render (PostProcessRenderContext context) {
            var sheet = context.propertySheets.Get (shader);

            // var projectionMatrix = GL.GetGPUProjectionMatrix (context.camera.projectionMatrix, false);
            // sheet.properties.SetMatrix ("_InverseProjectionMatrix", Matrix4x4.Inverse (projectionMatrix)); // use to calculate viewDir
            sheet.properties.SetFloat ("_Distance", settings.projectionDistance);
            sheet.properties.SetFloat ("_Scale", settings.scale);

            var cmd = context.command;
            cmd.BeginSample ("Panini Projection");
            cmd.BlitFullscreenTriangle (context.source, context.destination, sheet, 0);
            cmd.EndSample ("Panini Projection");
        }
    }
}