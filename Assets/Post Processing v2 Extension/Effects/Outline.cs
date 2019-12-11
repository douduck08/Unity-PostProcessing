using System;

namespace UnityEngine.Rendering.PostProcessing {
    [Serializable]
    [PostProcess (typeof (OutlineRenderer), PostProcessEvent.AfterStack, "Custom/Outline")]
    public sealed class Outline : PostProcessEffectSettings {
        public ColorParameter outlineColor = new ColorParameter () { value = Color.white };
        // [Range (0, 6)] public IntParameter downsample = new IntParameter () { value = 0 };
        [Range (1, 5)] public IntParameter iterations = new IntParameter () { value = 1 };
    }

    public sealed class OutlineRenderer : PostProcessEffectRenderer<Outline> {

        Shader outlineShader;
        Shader blurShader;
        int outlineColorId;
        int maskRT;
        int bluredMaskRT;
        int[] nameIds;

        public override void Init () {
            outlineShader = Shader.Find ("Hidden/Outline");
            blurShader = Shader.Find ("Hidden/DualBlur");
            outlineColorId = Shader.PropertyToID ("_OutlineColor");
            maskRT = Shader.PropertyToID ("_Mask1");
            bluredMaskRT = Shader.PropertyToID ("_Mask2");
            nameIds = new [] {
                Shader.PropertyToID ("_Temp1"),
                Shader.PropertyToID ("_Temp2"),
                Shader.PropertyToID ("_Temp3"),
                Shader.PropertyToID ("_Temp4"),
                Shader.PropertyToID ("_Temp5")
            };
        }

        public override DepthTextureMode GetCameraFlags () {
            return DepthTextureMode.Depth;
        }

        public override void Render (PostProcessRenderContext context) {
            var cmd = context.command;
            cmd.BeginSample ("Outline");

            var width = context.width;
            var height = context.height;
            context.GetScreenSpaceTemporaryRT (cmd, maskRT, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, FilterMode.Bilinear, width, height);
            context.GetScreenSpaceTemporaryRT (cmd, bluredMaskRT, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, FilterMode.Bilinear, width, height);

            var useResolvedDepth = RuntimeUtilities.IsResolvedDepthAvailable (context.camera);
            var depthMap = useResolvedDepth ? BuiltinRenderTextureType.ResolvedDepth : BuiltinRenderTextureType.Depth;
            var tempMask = bluredMaskRT; // borrowing bluredMask to use
            cmd.SetRenderTarget (tempMask, depthMap);
            cmd.ClearRenderTarget (false, true, Color.clear);
            foreach (var entity in OutlineEntityManager.instance.entities) {
                // TODO: check visible
                // TODO: handle submesh
                cmd.DrawRenderer (entity.renderer, entity.renderer.sharedMaterial, 0, 0);
            }

            var outlineSheet = context.propertySheets.Get (outlineShader);
            cmd.BlitFullscreenTriangle (tempMask, maskRT, outlineSheet, 0);

            var blurSheet = context.propertySheets.Get (blurShader);
            DualBlurMask (context, blurSheet, maskRT, bluredMaskRT, width, height, settings.iterations);

            outlineSheet.properties.SetColor (outlineColorId, settings.outlineColor);
            cmd.SetGlobalTexture ("_OutlineMaskTex", maskRT);
            cmd.SetGlobalTexture ("_BluredMaskTex", bluredMaskRT);
            cmd.BlitFullscreenTriangle (context.source, context.destination, outlineSheet, 1);

            cmd.ReleaseTemporaryRT (maskRT);
            cmd.ReleaseTemporaryRT (bluredMaskRT);
            cmd.EndSample ("Outline");
        }

        void DualBlurMask (PostProcessRenderContext context, PropertySheet blurSheet, int source, int destination, int sourceWidth, int sourceHeight, int iterations) {
            var cmd = context.command;
            for (int i = 0; i < iterations; i++) {
                var width = sourceWidth >> (i + 1);
                var height = sourceHeight >> (i + 1);
                context.GetScreenSpaceTemporaryRT (cmd, nameIds[i], 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, width, height);
            }

            // downsample
            cmd.BlitFullscreenTriangle (source, nameIds[0], blurSheet, 0);
            for (int i = 1; i < iterations; i++) {
                cmd.BlitFullscreenTriangle (nameIds[i - 1], nameIds[i], blurSheet, 0);
            }

            // upsample
            for (int i = iterations - 1; i >= 1; i--) {
                cmd.BlitFullscreenTriangle (nameIds[i], nameIds[i - 1], blurSheet, 1);
            }
            cmd.BlitFullscreenTriangle (nameIds[0], destination, blurSheet, 1);

            // release tempRT
            for (int i = 0; i < iterations; i++) {
                cmd.ReleaseTemporaryRT (nameIds[i]);
            }
        }
    }
}