using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �A���t�@�l��ς��đO�ʂ̃I�u�W�F�N�g�𓧉߂�����X�N���v�g(�g�D�[���V�F�[�_�[�̂�)
public class FrontObjectAlphaChange : MonoBehaviour
{
    private List <Material> Materials = new List<Material>();   // �}�e���A���i�[�p���X�g

    private static float StartDisAppearDistance = 23;   // �����n�߂鋗��(�������̕����傫��)
    private static float EndDisAppearDistance = 18;     // ���S�ɏ����鋗��(0�ȉ��ɂ��Ȃ�����)

    private bool OnceFlag = false;

    public float wa = 0;

    void Start()
    {
        Renderer[] renderers = this.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] materials = renderers[i].materials;

            for (int j = 0; j < materials.Length; j++)
            {
                Materials.Add(materials[j]);
            }
        }
    }

    void Update()
    {
        // �v���C���[�̍��W
        Vector3 PlayerPos = PlayerMain.instance.transform.position;
        Vector3 ThisPos = this.gameObject.transform.position;
        float Dis = Vector2.Distance(PlayerPos, ThisPos);

        // �v���C���[�Ƃ̋������m�F
        if (Dis <= StartDisAppearDistance)
        {
            if (Dis <= EndDisAppearDistance)
            {
                SetAlpha(-1.0f);
            }
            else
            {
                wa = (Dis - EndDisAppearDistance) / (StartDisAppearDistance - EndDisAppearDistance) - 1.0f;
                SetAlpha(wa);
            }
            OnceFlag = false;
        }
        else if (!OnceFlag)
        {
            OnceFlag = true;
            SetAlpha(0.0f);
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
}
