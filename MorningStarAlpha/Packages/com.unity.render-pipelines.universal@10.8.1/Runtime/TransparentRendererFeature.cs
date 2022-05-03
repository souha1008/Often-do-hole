using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal.Internal
{
    public class TransparentRendererFeature : ScriptableRendererFeature
    {
        private CopyTransparentPass copyTransparentPass = null;//�p�X

        private RenderTargetHandle m_CameraColorAttachment;//�R�s�[���e�N�X�`��
        private RenderTargetHandle m_CamerTransparentTexture; //�R�s�[��e�N�X�`��

        [SerializeField]
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;//�����_�����O�^�C�~���O


        public override void Create()
        {
            copyTransparentPass = new CopyTransparentPass(renderPassEvent);
            //�R�s�[���ƃR�s�[��̃e�N�X�`����`
            m_CameraColorAttachment.Init("_CameraColorTexture"); //�R�s�[���@_CameraColorTexture�ɂ��邱�ƂŃ����_�����O����Ă��������ݒ肳���
                                                                 //_CameraColorTexture��Unity�̃o�[�W�����ɂ���Ė��O���قȂ�\��������
            m_CamerTransparentTexture.Init("_CameraTransparentTexture"); //�R�s�[��@shadergraph�ɐݒ肷��e�N�X�`�����͂����Őݒ�@���O�͓K���ł���
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            copyTransparentPass.Setup(m_CameraColorAttachment.Identifier(), m_CamerTransparentTexture);
            renderer.EnqueuePass(copyTransparentPass);
        }
    }
}
