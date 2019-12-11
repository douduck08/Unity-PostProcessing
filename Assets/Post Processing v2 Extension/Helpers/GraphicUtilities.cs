namespace UnityEngine.Rendering.PostProcessing {
    public static class GraphicUtilities {

        static Shader _depthCopyShader;
        public static Shader depthCopyShader {
            get {
                if (_depthCopyShader == null) {
                    _depthCopyShader = Shader.Find ("Hidden/DepthCopy");
                }
                return _depthCopyShader;
            }
        }

        public static RenderTargetIdentifier DepthCopy (PostProcessRenderContext context, PropertySheet copySheet, int nameId, int width, int height) {
            // FIXME: cannot use this DepthCopy() result to SetRenderTarget() depth buffer
            var cmd = context.command;
            var depthTextureFormat = RenderTextureFormat.Depth;
            context.GetScreenSpaceTemporaryRT (cmd, nameId, 0, depthTextureFormat, RenderTextureReadWrite.Default, FilterMode.Point, width, height);
            cmd.BlitFullscreenTriangle (BuiltinRenderTextureType.None, nameId, copySheet, 0);
            return nameId;
        }
    }
}