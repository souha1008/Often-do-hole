//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Rendering;
//using UnityEngine.Rendering.Universal;

//// �X�e���V���o�b�t�@�`��Pass
//public class DrawStencilRendererPass : ScriptableRenderPass
//{
//    private const string ProfilerTag = nameof(DrawStencilRendererPass);
//    private new readonly ProfilingSampler profilingSampler = new ProfilingSampler(ProfilerTag);

//    public DrawStencilRendererPass()
//    {
//        renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
//    }

//    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
//    {
//        var cmd = CommandBufferPool.Get(ProfilerTag);
//        using (new ProfilingScope(cmd, profilingSampler))
//        {
//            context.ExecuteCommandBuffer(cmd);
//            cmd.Clear();

//            var camera = renderingData.cameraData.camera;

//            // �X�e���V���o�b�t�@��`��Ώۂɂ���
//            var renderMgr = RenderManager.Instance;
//            if (renderMgr.StencilBuffer == null)    // ������ΐ���
//            {
//                renderMgr.CreateStencilBuffer(new Vector2Int(camera.pixelWidth, camera.pixelHeight));
//            }
//            ConfigureTarget(renderMgr.StencilBuffer);
//            ConfigureClear(ClearFlag.All, Color.clear);

//            // �X�e���V���o�b�t�@�֕`����s��
//            SortingSettings sortingSettings = new SortingSettings(camera) { criteria = SortingCriteria.CommonOpaque };
//            FilteringSettings filteringSettings = new FilteringSettings(
//                                                    RenderQueueRange.all,
//                                                    camera.cullingMask
//                                                    );
//            List<ShaderTagId> shaderTagIds = new List<ShaderTagId>
//            {
//                new ShaderTagId( "DrawStencil" )
//            };
//            var drawingSettings = CreateDrawingSettings(shaderTagIds, ref renderingData, SortingCriteria.CommonTransparent);
//            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);

//            context.ExecuteCommandBuffer(cmd);
//            CommandBufferPool.Release(cmd);
//        }
//    }
//}