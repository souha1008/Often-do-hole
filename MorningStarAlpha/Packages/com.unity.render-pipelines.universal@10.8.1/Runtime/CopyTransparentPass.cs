using System;

namespace UnityEngine.Rendering.Universal.Internal
{
    public class CopyTransparentPass : ScriptableRenderPass
    {
        private RenderTargetIdentifier source { get; set; }
        private RenderTargetHandle destination { get; set; }
        const string m_ProfilerTag = "CopyColorTransparentPass";//FrameDebugger�ŕ\������閼�O

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
            //�𑜓x�����������Ȃ牺�L�R�����g��L���ɂ��Ă�������
            //descriptor.width /= 2;
            //descriptor.height /= 2;
            cmd.GetTemporaryRT(destination.id, descriptor, FilterMode.Point);
        }
        //�`�揈��
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            RenderTargetIdentifier opaqueColorRT = destination.Identifier();

            Blit(cmd, source, opaqueColorRT);
            Blit(cmd, opaqueColorRT, source);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        //�������
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