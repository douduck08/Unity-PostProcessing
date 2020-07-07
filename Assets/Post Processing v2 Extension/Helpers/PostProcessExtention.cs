namespace UnityEngine.Rendering.PostProcessing {
    public static class PostProcessExtention {

        static RenderTextureFormat depthTextureFormat = RenderTextureFormat.Depth;

        static Shader _depthCopyShader;
        public static Shader depthCopyShader {
            get {
                if (_depthCopyShader == null) {
                    _depthCopyShader = Shader.Find ("Hidden/DepthCopy");
                }
                return _depthCopyShader;
            }
        }


        public static RenderTargetIdentifier GetCameraDepthCopy (this PostProcessRenderContext context, CommandBuffer cmd, int destinationId, int width, int height) {
            // Limited by Unity: Cannot use this DepthCopy() result to SetRenderTarget() depth buffer
            var copySheet = context.propertySheets.Get (depthCopyShader);
            context.GetScreenSpaceTemporaryRT (cmd, destinationId, 32, depthTextureFormat, RenderTextureReadWrite.Default, FilterMode.Point, width, height);
            cmd.BlitFullscreenTriangle (BuiltinRenderTextureType.None, destinationId, copySheet, 0);
            return destinationId;
        }

        public static void CopyCameraDepth (this PostProcessRenderContext context, CommandBuffer cmd, RenderTexture destination) {
            var copySheet = context.propertySheets.Get (depthCopyShader);
            cmd.BlitFullscreenTriangle (BuiltinRenderTextureType.None, destination, copySheet, 0);
        }
    }
}