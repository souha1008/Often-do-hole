using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �A���t�@�l��ς��đO�ʂ̃I�u�W�F�N�g�𓧉߂�����X�N���v�g(�g�D�[���V�F�[�_�[�̂�)
public class FrontObjectAlphaChange : MonoBehaviour
{
    private List <Material> Materials = new List<Material>();   // �}�e���A���i�[�p���X�g

    //[Label("�����n�߂鋗��")] public float StartDisAppearDistance = 23;   // �����n�߂鋗��(�������̕����傫��)
    //[Label("���S�ɏ����鋗��")] public float EndDisAppearDistance = 18;     // ���S�ɏ����鋗��(0�ȉ��ɂ��Ȃ�����)
    [Label("�ŏ��̓��ߗ�(0�`1)")] public float StartAlpha = 1.0f;     // �ŏ��̓��ߗ�(0.0f�`1.0f)

    //private bool OnceFlag = false;

    //[ReadOnly] public float NowAlpha = 0;

    void Start()
    {
        //if (StartDisAppearDistance < EndDisAppearDistance)
        //{
        //    StartDisAppearDistance = EndDisAppearDistance;
        //}

        if (StartAlpha < 0.0f)
        {
            StartAlpha = 0.0f;
        }
        else if (StartAlpha > 1.0f)
        {
            StartAlpha = 1.0f;
        }

        Renderer[] renderers = this.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] materials = renderers[i].materials;

            for (int j = 0; j < materials.Length; j++)
            {
                Materials.Add(materials[j]);
            }
        }

        SetAlpha(StartAlpha - 1.0f); // �A���t�@�l�Z�b�g
        Materials.Clear();  // �I������
    }

    void Update()
    {
//#if UNITY_EDITOR
//        if (StartAlpha < 0.0f)
//        {
//            StartAlpha = 0.0f;
//        }
//        else if (StartAlpha > 1.0f)
//        {
//            StartAlpha = 1.0f;
//        }
//#endif


//        // �v���C���[�̍��W
//        Vector3 PlayerPos = PlayerMain.instance.transform.position;
//        Vector3 ThisPos = this.gameObject.transform.position;
//        float Dis = Vector2.Distance(PlayerPos, ThisPos);

//        // �v���C���[�Ƃ̋������m�F
//        if (Dis <= StartDisAppearDistance)
//        {
//            if (Dis <= EndDisAppearDistance)
//            {
//                NowAlpha = -1.0f;
//                SetAlpha(NowAlpha);
//            }
//            else
//            {
//                NowAlpha = ((Dis - EndDisAppearDistance) / (StartDisAppearDistance - EndDisAppearDistance)) * StartAlpha - 1.0f;
//                SetAlpha(NowAlpha);
//            }
//            OnceFlag = false;
//        }
//        else if (!OnceFlag)
//        {
//            OnceFlag = true;
//            NowAlpha = StartAlpha - 1.0f;
//            SetAlpha(NowAlpha);
//        }
    }

    // �A���t�@�l�ύX����
    private void SetAlpha(float Alpha)
    {
        for (int i = 0; i < Materials.Count; i++)
        {
            Materials[i].SetFloat("_Tweak_transparency", Alpha);
        }
    }

    private void OnDestroy()
    {
        Materials.Clear();
    }
}
