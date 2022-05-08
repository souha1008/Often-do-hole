//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Rendering;
//using UnityEngine.Rendering.Universal;

//// ステンシルバッファ描画Pass
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

//            // ステンシルバッファを描画対象にする
//            var renderMgr = RenderManager.Instance;
//            if (renderMgr.StencilBuffer == null)    // 無ければ生成
//            {
//                renderMgr.CreateStencilBuffer(new Vector2Int(camera.pixelWidth, camera.pixelHeight));
//            }
//            ConfigureTarget(renderMgr.StencilBuffer);
//            ConfigureClear(ClearFlag.All, Color.clear);

//            // ステンシルバッファへ描画を行う
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