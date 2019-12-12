namespace UnityEngine.Rendering.PostProcessing {
    public static class PostProcessExtention {

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
            // FIXME: cannot use this DepthCopy() result to SetRenderTarget() depth buffer
            var copySheet = context.propertySheets.Get (depthCopyShader);
            var depthTextureFormat = RenderTextureFormat.Depth;
            context.GetScreenSpaceTemporaryRT (cmd, destinationId, 32, depthTextureFormat, RenderTextureReadWrite.Default, FilterMode.Point, width, height);
            cmd.BlitFullscreenTriangle (BuiltinRenderTextureType.None, destinationId, copySheet, 0);
            return destinationId;
        }
    }
}