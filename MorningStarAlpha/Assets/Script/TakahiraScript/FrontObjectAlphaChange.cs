using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �A���t�@�l��ς��đO�ʂ̃I�u�W�F�N�g�𓧉߂�����X�N���v�g(�g�D�[���V�F�[�_�[�̂�)
public class FrontObjectAlphaChange : MonoBehaviour
{
    private List <Material> Materials = new List<Material>();   // �}�e���A���i�[�p���X�g

    [Label("�����ɂ��ϓ��Ȃ��Ȃ�`�F�b�N")] public bool NoUseDistanceFlag = false;     // �ŏ��̓��ߗ�(0.0f�`1.0f)
    [Label("�����n�߂鋗��")] public float StartDisAppearDistance = 23;   // �����n�߂鋗��(�������̕����傫��)
    [Label("���S�ɏ����鋗��")] public float EndDisAppearDistance = 18;     // ���S�ɏ����鋗��(0�ȉ��ɂ��Ȃ�����)
    [Label("�ŏ��̓��ߗ�(0�`1)")] public float StartAlpha = 1.0f;     // �ŏ��̓��ߗ�(0.0f�`1.0f)
    [Label("�Ō�̓��ߗ�(0�`1)")] public float EndAlpha = 0.0f;     // �Ō�̓��ߗ�(0.0f�`1.0f)

    private bool OnceFlag = false;

    [ReadOnly] public float NowAlpha = 0;

    void Start()
    {
        CheckLength(); // ����,���ߓx�̖����`�F�b�N

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
        if (NoUseDistanceFlag) 
            Materials.Clear();  // �I������
    }

    void Update()
    {
        if (!NoUseDistanceFlag)
        {

#if UNITY_EDITOR
            CheckLength(); // ����,���ߓx�̖����`�F�b�N
#endif

            // �v���C���[�̍��W
            Vector3 PlayerPos;
            if (PlayerMain.instance != null)
            {
                PlayerPos = PlayerMain.instance.transform.position;
            }
            else
            {
                NoUseDistanceFlag = true;
                return;
            }
            Vector3 ThisPos = this.gameObject.transform.position;
            float Dis = Vector2.Distance(PlayerPos, ThisPos);

            // �v���C���[�Ƃ̋������m�F
            if (Dis <= StartDisAppearDistance)
            {
                if (Dis <= EndDisAppearDistance)
                {
                    NowAlpha = EndAlpha - 1.0f;
                    SetAlpha(NowAlpha);
                }
                else
                {
                    NowAlpha = ((Dis - EndDisAppearDistance) / (StartDisAppearDistance - EndDisAppearDistance)) * (StartAlpha - EndAlpha) + EndAlpha - 1.0f;
                    SetAlpha(NowAlpha);
                }
                OnceFlag = false;
            }
            else if (!OnceFlag)
            {
                OnceFlag = true;
                NowAlpha = StartAlpha - 1.0f;
                SetAlpha(NowAlpha);
            }
        }
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

    // ����,���ߓx�̖����`�F�b�N
    private void CheckLength()
    {
        if (StartDisAppearDistance < EndDisAppearDistance)
        {
            StartDisAppearDistance = EndDisAppearDistance;
        }

        Mathf.Clamp(StartAlpha, 0.0f, 1.0f);
        Mathf.Clamp(EndAlpha, 0.0f, 1.0f);

        if (StartAlpha < EndAlpha)
        {
            StartAlpha = EndAlpha;
        }
    }
}
