using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal.Internal
{
    public class TransparentRendererFeature : ScriptableRendererFeature
    {
        private CopyTransparentPass copyTransparentPass = null;//パス

        private RenderTargetHandle m_CameraColorAttachment;//コピー元テクスチャ
        private RenderTargetHandle m_CamerTransparentTexture; //コピー先テクスチャ

        [SerializeField]
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;//レンダリングタイミング


        public override void Create()
        {
            copyTransparentPass = new CopyTransparentPass(renderPassEvent);
            //コピー元とコピー先のテクスチャ定義
            m_CameraColorAttachment.Init("_CameraColorTexture"); //コピー元　_CameraColorTextureにすることでレンダリングされてきた物が設定される
                                                                 //_CameraColorTextureはUnityのバージョンによって名前が異なる可能性がある
            m_CamerTransparentTexture.Init("_CameraTransparentTexture"); //コピー先　shadergraphに設定するテクスチャ名はここで設定　名前は適当でいい
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            copyTransparentPass.Setup(m_CameraColorAttachment.Identifier(), m_CamerTransparentTexture);
            renderer.EnqueuePass(copyTransparentPass);
        }
    }
}
