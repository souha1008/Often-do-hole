using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


public class FadeManager : SingletonMonoBehaviour<FadeManager>
{
    // �ϐ�

    // �t�F�[�h�b��(�Q�[���I�[�o�[)
    [Header("�t�F�[�h�ɂ�����b��(�Q�[���I�[�o�[)")]
    [SerializeField] private float FadeTime_GameOver = 1;

    // �t�F�[�h�b��(�V�[���ύX)
    [Header("�t�F�[�h�ɂ�����b��(�V�[���ύX)")]
    [SerializeField] private float FadeTime_SceneChange = 1;

    // �t�F�[�h�b��(�X�e�[�W�ύX)
    [Header("�t�F�[�h�ɂ�����b��(�X�e�[�W�ύX)")]
    [SerializeField] private float FadeTime_StageChange = 1;


    private Texture2D FadeTexture;                    // �t�F�[�h�̃e�N�X�`��
    private float FadeRate;                           // �t�F�[�h�W��
    private FADE_STATE NowFadeState;                  // ���݂̃t�F�[�h�̏��
    private FADE_STATE OldFadeState;                  // �ЂƂO�̃t�F�[�h�̏��
    private FADE_KIND NowFadeKind;                    // ���݂̃t�F�[�h�̎��
    private Color FadeColor;                          // �t�F�[�h�̃J���[

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject); // �V�[�����ς���Ă����ȂȂ�

        // ������
        NowFadeState = OldFadeState = FADE_STATE.FADE_NONE;
        NowFadeKind = FADE_KIND.FADE_GAMOVER;
        FadeRate = Time.fixedDeltaTime / FadeTime_GameOver;
        FadeColor = new Color(0, 0, 0, 0);

        //�e�N�X�`���쐬
        FadeTexture = new Texture2D(32, 32, TextureFormat.RGB24, false);
        FadeTexture.ReadPixels(new Rect(0, 0, 32, 32), 0, 0, false);
        FadeTexture.SetPixel(0, 0, Color.white);
        FadeTexture.Apply();
    }
    private void OnGUI()
    {
        if (NowFadeState == FADE_STATE.FADE_NONE)
            return;

        //�����x���X�V���ăe�N�X�`����`��
        GUI.color = FadeColor;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), FadeTexture);
    }

    private void Start() { }

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
        // �`�F�b�N�|�C���g�ɕ��A����
        CheckPointManager.Instance.CheckPointAction();
    }
    // �t�F�[�h�C�������Ƃ��̏���(�V�[���ύX)
    private void FadeIn_SceneChange()
    {
        // ����ŕʂ̏����ɔC����\��
    }
    // �t�F�[�h�C�������Ƃ��̏���(�X�e�[�W�ύX)
    private void FadeIn_StageChange()
    {
        // ����ŕʂ̏����ɔC����\��
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
                    FadeRate = Time.fixedDeltaTime / FadeTime_GameOver;
                    break;
                case FADE_KIND.FADE_SCENECHANGE:
                    FadeRate = Time.fixedDeltaTime / FadeTime_SceneChange;
                    break;
                case FADE_KIND.FADE_STAGECHANGE:
                    FadeRate = Time.fixedDeltaTime / FadeTime_StageChange;
                    break;
                default:
                    break;
            }
        }
    }
    public FADE_STATE GetNowState()
    {
        return NowFadeState;
    }
}
