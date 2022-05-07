using System;

namespace UnityEngine.Rendering.Universal.Internal
{
    public class CopyTransparentPass : ScriptableRenderPass
    {
        private RenderTargetIdentifier source { get; set; }
        private RenderTargetHandle destination { get; set; }
        const string m_ProfilerTag = "CopyColorTransparentPass";//FrameDebuggerで表示される名前

        public CopyTransparentPass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
        }

        public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination)
        {
            this.source = source;
            this.destination = destination;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescripor)
        {
            RenderTextureDescriptor descriptor = cameraTextureDescripor;
            descriptor.msaaSamples = 1;
            descriptor.depthBufferBits = 0;
            //解像度を下げたいなら下記コメントを有効にしてください
            //descriptor.width /= 2;
            //descriptor.height /= 2;
            cmd.GetTemporaryRT(destination.id, descriptor, FilterMode.Point);
        }
        //描画処理
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            RenderTargetIdentifier opaqueColorRT = destination.Identifier();

            Blit(cmd, source, opaqueColorRT);
            Blit(cmd, opaqueColorRT, source);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        //解放処理
        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");

            if (destination != RenderTargetHandle.CameraTarget)
            {
                cmd.ReleaseTemporaryRT(destination.id);
                destination = RenderTargetHandle.CameraTarget;
            }
        }
    }
}