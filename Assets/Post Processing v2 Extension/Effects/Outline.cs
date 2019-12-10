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
        Shader depthCopyShader;
        int outlineColorId;
        int depthCopyId;
        int maskRT;
        int bluredMaskRT;
        int[] nameIds;

        public override void Init () {
            outlineShader = Shader.Find ("Hidden/Outline");
            blurShader = Shader.Find ("Hidden/DualBlur");
            depthCopyShader = Shader.Find ("Hidden/PostProcessing/MultiScaleVO");
            depthCopyId = Shader.PropertyToID ("CopyDepth");
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
            var outlineSheet = context.propertySheets.Get (outlineShader);
            outlineSheet.properties.SetColor (outlineColorId, settings.outlineColor);

            var blurSheet = context.propertySheets.Get (blurShader);
            var copySheet = context.propertySheets.Get (depthCopyShader);

            var cmd = context.command;
            cmd.BeginSample ("Outline");

            var width = context.width;
            var height = context.height;
            context.GetScreenSpaceTemporaryRT (cmd, maskRT, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, FilterMode.Bilinear, width, height);
            context.GetScreenSpaceTemporaryRT (cmd, bluredMaskRT, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, FilterMode.Bilinear, width, height);

            var needCopyDepth = !RuntimeUtilities.IsResolvedDepthAvailable (context.camera);
            var depthMap = needCopyDepth ? DepthCopy (context, copySheet, depthCopyId, width, height) : BuiltinRenderTextureType.ResolvedDepth;

            var tempMask = bluredMaskRT; // temp using
            cmd.SetRenderTarget (tempMask, depthMap);
            cmd.ClearRenderTarget (false, true, Color.clear);
            foreach (var entity in OutlineEntityManager.instance.entities) {
                // TODO: check visible
                // TODO: handle submesh
                cmd.DrawRenderer (entity.renderer, entity.renderer.sharedMaterial, 0, 0);
            }
            cmd.BlitFullscreenTriangle (tempMask, maskRT, outlineSheet, 0);
            DualBlurMask (context, blurSheet, maskRT, bluredMaskRT, width, height, settings.iterations);

            cmd.SetGlobalTexture ("_OutlineMaskTex", maskRT);
            cmd.SetGlobalTexture ("_BluredMaskTex", bluredMaskRT);
            cmd.BlitFullscreenTriangle (context.source, context.destination, outlineSheet, 1);

            cmd.ReleaseTemporaryRT (maskRT);
            cmd.ReleaseTemporaryRT (bluredMaskRT);
            if (needCopyDepth) {
                cmd.ReleaseTemporaryRT (depthCopyId);
            }
            cmd.EndSample ("Outline");
        }

        RenderTargetIdentifier DepthCopy (PostProcessRenderContext context, PropertySheet copySheet, int nameId, int width, int height) {
            // has bug on forward path
            var cmd = context.command;
            var depthTextureFormat = RenderTextureFormat.RFloat;
            context.GetScreenSpaceTemporaryRT (cmd, nameId, 0, depthTextureFormat, RenderTextureReadWrite.Default, FilterMode.Point, width, height);
            cmd.BlitFullscreenTriangle (BuiltinRenderTextureType.None, nameId, copySheet, 0);
            return nameId;
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