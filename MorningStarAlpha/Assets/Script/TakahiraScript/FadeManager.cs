using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [Label("�t�F�[�h�b��(�Q�[���I�[�o�[)")]
    [SerializeField] private float FadeTime_GameOver = 1;

    // �t�F�[�h�b��(�V�[���ύX)
    [Label("�t�F�[�h�b��(�V�[���ύX)")]
    [SerializeField] private float FadeTime_SceneChange = 1;

    // �t�F�[�h�b��(�X�e�[�W�ύX)
    [Label("�t�F�[�h�b��(�X�e�[�W�ύX)")]
    [SerializeField] private float FadeTime_StageChange = 1;


    private Texture2D FadeTexture;                    // �t�F�[�h�̃e�N�X�`��
    static private FADE_STATE NowFadeState;           // ���݂̃t�F�[�h�̏��
    private FADE_STATE OldFadeState;                  // �ЂƂO�̃t�F�[�h�̏��
    private FADE_KIND NowFadeKind;                    // ���݂̃t�F�[�h�̎��
    private Color FadeColor;                          // �t�F�[�h�̃J���[

    private float NowTime;

    private float TimeGameOver;
    private float TimeSceneChange;
    private float TimeStageChange;

    private string NextSceneName;

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject); // �V�[�����ς���Ă����ȂȂ�

        // ������
        FadeTime_GameOver = Mathf.Max(FadeTime_GameOver, 0.01f);
        FadeTime_SceneChange = Mathf.Max(FadeTime_SceneChange, 0.01f);
        FadeTime_StageChange = Mathf.Max(FadeTime_StageChange, 0.01f);

        TimeGameOver = FadeTime_GameOver;
        TimeSceneChange = FadeTime_SceneChange;
        TimeStageChange = FadeTime_StageChange;

        NowTime = 0.0f;

        NowFadeState = OldFadeState = FADE_STATE.FADE_NONE;
        NowFadeKind = FADE_KIND.FADE_GAMOVER;
        FadeColor = new Color(0, 0, 0, 0);

        //�e�N�X�`���쐬
        //Debug.Log("FadeManager�쐬");

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

    private void Start() 
    {
        SoundManager.Instance.PlaySound("MorningBGM", 0.6f, AudioReverbPreset.City);
    }

    private void Update()
    {
        // �t�F�[�h������
        if (NowFadeState != FADE_STATE.FADE_NONE)
        {
            if (NowFadeState == FADE_STATE.FADE_OUT)
            {// �t�F�[�h�A�E�g����
                switch (NowFadeKind)// ���l�����Z���ĉ�ʂ������Ă���
                {
                    case FADE_KIND.FADE_GAMOVER:
                        FadeColor.a = NowTime / TimeGameOver;
                        break;
                    case FADE_KIND.FADE_SCENECHANGE:
                        FadeColor.a = NowTime / TimeSceneChange;
                        break;
                    case FADE_KIND.FADE_STAGECHANGE:
                        FadeColor.a = NowTime / TimeStageChange;
                        break;
                    default:
                        break;
                }
                NowTime += Time.unscaledDeltaTime; // ���ԉ��Z

                if (FadeColor.a >= 1.0f)
                {
                    // �t�F�[�h�C�������ɐ؂�ւ�
                    FadeColor.a = 1.0f;
                    NowTime = 0.0f;
                    NowFadeState = FADE_STATE.FADE_IN;
                }
            }
            else if (NowFadeState == FADE_STATE.FADE_IN)
            {// �t�F�[�h�C������
                switch (NowFadeKind)// ���l�����Z���ĉ�ʂ𕂂��オ�点��
                {
                    case FADE_KIND.FADE_GAMOVER:
                        FadeColor.a = 1.0f - NowTime / TimeGameOver;
                        break;
                    case FADE_KIND.FADE_SCENECHANGE:
                        FadeColor.a = 1.0f - NowTime / TimeSceneChange;
                        break;
                    case FADE_KIND.FADE_STAGECHANGE:
                        FadeColor.a = 1.0f - NowTime / TimeStageChange;
                        break;
                    default:
                        break;
                }
                NowTime += Time.unscaledDeltaTime; // ���ԉ��Z

                if (FadeColor.a <= 0.0f)
                {
                    // �t�F�[�h�����I��
                    FadeColor.a = 0.0f;
                    NowTime = 0.0f;
                    NowFadeState = FADE_STATE.FADE_NONE;
                }
            }
        }

        // �t�F�[�h�C�����̏���
        if (OldFadeState == FADE_STATE.FADE_OUT && NowFadeState == FADE_STATE.FADE_IN)
        {
            //�V�[���I���Ȃ̂Ŏ��Ԃ����Ƃɖ߂�
            Time.timeScale = 1.0f;
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


    // �t�F�[�h�C�������Ƃ��̏���(�Q�[���I�[�o�[)(�Q�[�����X�^�[�g)
    private void FadeIn_GameOver()
    {
        // �Q�[���V�[���̃��Z�b�g
        SoundManager.Instance.StopSound("BGM_01");
        SceneManager.LoadScene(NextSceneName);
        SoundManager.Instance.PlaySound("BGM_01", 0.6f, AudioReverbPreset.City);
        GameStateManager.SetGameState(GAME_STATE.PLAY);
        Time.timeScale = 1.0f;
    }
    // �t�F�[�h�C�������Ƃ��̏���(�V�[���ύX)
    public void FadeIn_SceneChange()
    {
        SceneManager.LoadScene(NextSceneName);
    }

    // �t�F�[�h�C�������Ƃ��̏���(�X�e�[�W�ύX)
    public void FadeIn_StageChange()
    {
        // ����ŕʂ̏����ɔC����\��
    }

    // �t�F�[�h�ύX
    //
    // �����P�F�t�F�[�h��Ԃ̎��(���̂Ƃ���t�F�[�h�A�E�g�̂�)
    // �����Q�F���̃t�F�[�h��(�Q�[���I�[�o�[,�V�[���`�F���Wetc...)
    public void FadeStart(string nextFadeName, FADE_KIND FadeKind)
    {
        if (NowFadeState == FADE_STATE.FADE_NONE)
        {
            NextSceneName = nextFadeName;

            NowFadeState = FADE_STATE.FADE_OUT;
            NowFadeKind = FadeKind;

            NowTime = 0.0f;

            TimeGameOver = FadeTime_GameOver = Mathf.Max(FadeTime_GameOver, 0.01f);
            TimeSceneChange = FadeTime_SceneChange = Mathf.Max(FadeTime_SceneChange, 0.01f);
            TimeStageChange = FadeTime_StageChange = Mathf.Max(FadeTime_StageChange, 0.01f);
        }
    }

    public void FadeGameOver()
    {
        if (NowFadeState == FADE_STATE.FADE_NONE)
        {
            NextSceneName = SceneManager.GetActiveScene().name;
            NowFadeState = FADE_STATE.FADE_OUT;
            NowFadeKind = FADE_KIND.FADE_GAMOVER;

            NowTime = 0.0f;

            TimeGameOver = FadeTime_GameOver = Mathf.Max(FadeTime_GameOver, 0.01f);
            TimeSceneChange = FadeTime_SceneChange = Mathf.Max(FadeTime_SceneChange, 0.01f);
            TimeStageChange = FadeTime_StageChange = Mathf.Max(FadeTime_StageChange, 0.01f);
        }
    }

    static public FADE_STATE GetNowState()
    {
        return NowFadeState;
    }
}
