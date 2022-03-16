using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// �t�F�[�h�̏��
public enum FADE_STATE
{
    FADE_NONE = 0,      // �t�F�[�h���������Ă��Ȃ�
    FADE_IN,            // �t�F�[�h�C��������
    FADE_OUT,           // �t�F�[�h�A�E�g������
    //FADE_MAX            // �t�F�[�h��ԍő吔
}

// �t�F�[�h�̎��
public enum FADE_KIND
{
    FADE_GAMOVER = 0,      // �Q�[���I�[�o�[�t�F�[�h
    FADE_SCENECHANGE,      // �V�[���ύX�t�F�[�h
    FADE_STAGECHANGE,      // �X�e�[�W�ύX�t�F�[�h
    //FADE_MAX               // �t�F�[�h�̎�ލő吔
}


public class Fade : MonoBehaviour
{
    // �ϐ�
    public GameObject Player;                         // �v���C���[�I�u�W�F�N�g
    public Image FadeImage;                           // �t�F�[�h�̃C���[�W
    public const float FadeRate_GameOver = 0.04f;     // �t�F�[�h�W��(�Q�[���I�[�o�[)
    public const float FadeRate_SceneChange = 0.02f;  // �t�F�[�h�W��(�V�[���ύX)
    public const float FadeRate_StageChange = 0.02f;  // �t�F�[�h�W��(�X�e�[�W�ύX)
    private float FadeRate;                           // �t�F�[�h�W��
    private FADE_STATE NowFadeState;                  // ���݂̃t�F�[�h�̏��
    private FADE_STATE OldFadeState;                  // �ЂƂO�̃t�F�[�h�̏��
    private FADE_KIND NowFadeKind;                    // ���݂̃t�F�[�h�̎��
    private Color FadeColor;                          // �t�F�[�h�̃J���[

    private void Start()
    {
        // ������
        NowFadeState = OldFadeState = FADE_STATE.FADE_NONE;
        NowFadeKind = FADE_KIND.FADE_GAMOVER;
        FadeRate = FadeRate_GameOver;
        FadeColor = new Color(0, 0, 0, 0);

        DontDestroyOnLoad(this.gameObject);
    }

    private void FixedUpdate()
    {
        // �t�F�[�h������
        if (NowFadeState != FADE_STATE.FADE_NONE)
        {
            if (NowFadeState == FADE_STATE.FADE_OUT)
            {// �t�F�[�h�A�E�g����
                FadeColor.a += FadeRate;     // ���l�����Z���ĉ�ʂ������Ă���

                if (FadeColor.a >= 1.0f)
                {
                    // �t�F�[�h�C�������ɐ؂�ւ�
                    FadeColor.a = 1.0f;
                    NowFadeState = FADE_STATE.FADE_IN;
                }
            }
            else if (NowFadeState == FADE_STATE.FADE_IN)
            {// �t�F�[�h�C������
                FadeColor.a -= FadeRate;     // ���l�����Z���ĉ�ʂ𕂂��オ�点��
                if (FadeColor.a <= 0.0f)
                {
                    // �t�F�[�h�����I��
                    FadeColor.a = 0.0f;
                    NowFadeState = FADE_STATE.FADE_NONE;
                }
            }

            FadeImage.color = FadeColor; // �J���[�ύX
        }

        // �t�F�[�h�C�����̏���
        if (OldFadeState == FADE_STATE.FADE_OUT && NowFadeState == FADE_STATE.FADE_IN)
        {
            switch (NowFadeKind)
            {
                case FADE_KIND.FADE_GAMOVER:
                    FadeIn_GameOver();
                    break;
                case FADE_KIND.FADE_SCENECHANGE:
                    FadeIn_SceneChange();
                    break;
                case FADE_KIND.FADE_STAGECHANGE:
                    FadeIn_StageChange();
                    break;
                default:
                    break;
            }
        }

        OldFadeState = NowFadeState; // �ЂƂO�̃t�F�[�h�Ɍ��݂̃t�F�[�h����
    }


    // �t�F�[�h�C�������Ƃ��̏���(�Q�[���I�[�o�[)
    private void FadeIn_GameOver()
    {
        // ��Ń`�F�b�N�|�C���g�����ɔC����\��
        Player.GetComponent<Transform>().position = new Vector3(0, 10, 0);
        Player.GetComponent<PlayerMain>().vel = new Vector3(0, 0, 0);
    }
    // �t�F�[�h�C�������Ƃ��̏���(�V�[���ύX)
    private void FadeIn_SceneChange()
    {
        // ��ŕʂ̏����ɔC����\��
    }
    // �t�F�[�h�C�������Ƃ��̏���(�X�e�[�W�ύX)
    private void FadeIn_StageChange()
    {
        // ��ŕʂ̏����ɔC����\��
    }


    public FADE_STATE GetNowState()
    {
        return NowFadeState;
    }
    // �t�F�[�h�ύX
    //
    // �����P�F�t�F�[�h��Ԃ̎��(���̂Ƃ���t�F�[�h�A�E�g�̂�)
    // �����Q�F���̃t�F�[�h��(�Q�[���I�[�o�[,�V�[���`�F���Wetc...)
    public void SetNextFade(FADE_STATE NextFadeState, FADE_KIND FadeKind)
    {
        if (NowFadeState == FADE_STATE.FADE_NONE)
        {
            NowFadeState = NextFadeState;
            NowFadeKind = FadeKind;

            switch(NowFadeKind)
            {
                case FADE_KIND.FADE_GAMOVER:
                    FadeRate = FadeRate_GameOver;
                    break;
                case FADE_KIND.FADE_SCENECHANGE:
                    FadeRate = FadeRate_SceneChange;
                    break;
                case FADE_KIND.FADE_STAGECHANGE:
                    FadeRate = FadeRate_StageChange;
                    break;
                default:
                    break;
            }
        }
    }
}
