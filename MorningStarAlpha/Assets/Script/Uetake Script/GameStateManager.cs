using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Q�[���̏��
/// </summary>
public enum GAME_STATE {
    PLAY,   //�Q�[���ł�����
    PAUSE,  //�|�[�Y��
    RESULT,//���U���g��
}

public class CLEAR_RANK_TIME {
    public CLEAR_RANK_TIME(float s , float a , float b){ S = s; A = a; B = b;}

    public float S;
    public float A;
    public float B;

}



/// <summary>
/// �Q�[���̏�Ԃ��Ǘ�
/// </summary>
public class GameStateManager : SingletonMonoBehaviour<GameStateManager>
{
    private const int STAGE_MAX_NUM = 8;
    private const float MAX_TIME = 999.9999f;

#if true
    private string[] StageNames = { "Stage1-1_fix" ,"Stage1-2_fix" , "Stage2-1_fix" , "Stage2-2_fix" , "Stage2-3_fix" , "Stage3-1_fix",
    "Stage3-3_fix", "StageBoss_fix"};

      public CLEAR_RANK_TIME[] ClearRankTime = {
        new CLEAR_RANK_TIME(50 ,150,999) ,
        new CLEAR_RANK_TIME(120,220,999) ,
        new CLEAR_RANK_TIME(100,200,999) ,
        new CLEAR_RANK_TIME(110,210,999) ,
        new CLEAR_RANK_TIME(120,220,999) ,
        new CLEAR_RANK_TIME(120,220,999) ,
        new CLEAR_RANK_TIME(120,220,999) ,
        new CLEAR_RANK_TIME(100,200,999) 
    };
#else
    private string[] StageNames = { "coinTestScene" ,"coinTestScene" , "coinTestScene" , "coinTestScene" , "coinTestScene" , "coinTestScene",
    "coinTestScene", "coinTestScene"};

    public CLEAR_RANK_TIME[] ClearRankTime = {
        new CLEAR_RANK_TIME(10,15,999) ,
        new CLEAR_RANK_TIME(150,240,999) ,
        new CLEAR_RANK_TIME(120,250,999) ,
        new CLEAR_RANK_TIME(130,300,999) ,
        new CLEAR_RANK_TIME(150,400,999) ,
        new CLEAR_RANK_TIME(150,400,999) ,
        new CLEAR_RANK_TIME(200,450,999) ,
        new CLEAR_RANK_TIME(250,450,999)
    };
#endif

    private GAME_STATE GameState;
    private float GameTime;
    private GAME_RANK GameRank;
    private int NowStage = 0;
    private bool StageSoundFlag = false;

    [SerializeField] public bool ColliderVisible = true;

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            SetGameState(GAME_STATE.PLAY);
            return;
        }
        DontDestroyOnLoad(this.gameObject); // �V�[�����ς���Ă����ȂȂ�

        Init();
    }

    private void Start()
    {

    }

    private void Update()
    {
        CountDown();
    }

    private void Init()
    {
        GameTime = 0.0f;
        GameRank = GAME_RANK.S;
        SetGameState(GAME_STATE.PLAY);
    }

    //�v���C���[�ŌĂ�ł���
    public void PlayGameStageBGM()
    {
        if (StageSoundFlag)
        {
            SoundManager.Instance.StopSound();

            switch (NowStage) {
                case 0:
                case 1:
                case 2:
                    SoundManager.Instance.PlaySound("MorningBGM", 0.53f, AudioReverbPreset.City);
                    break;

                case 3:
                case 4:
                    SoundManager.Instance.PlaySound("EveningBGM", 0.53f, AudioReverbPreset.City);
                    break;

                case 5:
                case 6:               
                    SoundManager.Instance.PlaySound("night_theme_0516", 0.53f, AudioReverbPreset.City);
                    break;

                case 7:
                    SoundManager.Instance.PlaySound("lastbattle_0516", 0.53f, AudioReverbPreset.City);
                    break;

                default:
                    break;
            }


         
            StageSoundFlag = false;
        }
    }

    public bool PressAny()
    {
        bool returnBool = false;

        if (Input.GetKeyDown("joystick button 0"))
        {
            returnBool = true;
        }
        if (Input.GetKeyDown("joystick button 1"))
        {
            returnBool = true;
        }
        if (Input.GetKeyDown("joystick button 2"))
        {
            returnBool = true;
        }
        if (Input.GetKeyDown("joystick button 3"))
        {
            returnBool = true;
        }
        if (Input.GetKeyDown("joystick button 4"))
        {
            returnBool = true;
        }
        if (Input.GetKeyDown("joystick button 5"))
        {
            returnBool = true;
        }
        if (Input.GetKeyDown("joystick button 6"))
        {
            returnBool = true;
        }
        if (Input.GetKeyDown("joystick button 7"))
        {
            returnBool = true;
        }
        if (Input.GetKeyDown("joystick button 8"))
        {
            returnBool = true;
        }
        if (Input.GetKeyDown("joystick button 9"))
        {
            returnBool = true;
        }

        return returnBool;
    }

    public static GAME_STATE GetGameState()
    {
        return Instance.GameState;
    }

    public static void SetGameState(GAME_STATE game_state)
    {
        Instance.GameState = game_state;

        // �Q�[���N���A���̏���
        if(Instance.GameState == GAME_STATE.RESULT)
        {
            // �R�C��
            CoinManager.Instance.SetCheckPointCoinData();   // �R�C���f�[�^�X�V
            CoinManager.Instance.SetCoinSaveData();         // �R�C���f�[�^�Z�[�u

            // �����N
            Instance.GameRank = Instance.CalicurateRank(GetNowStage());

            // �`�F�b�N�|�C���g
            CheckPointManager.Instance.ResetCheckPoint();   // �`�F�b�N�|�C���g�̃��Z�b�g



            // ���Ԃ��Z�[�u�f�[�^�ɃZ�[�u����
            if (GetGameTime() < SaveDataManager.Instance.MainData.Stage[GetNowStage()].Time)
            {
                SaveDataManager.Instance.MainData.Stage[GetNowStage()].Time = GetGameTime();
            }
            // �����N���Z�[�u�f�[�^�ɃZ�[�u����
            if (((int)GetGameRank()) < ((int)SaveDataManager.Instance.MainData.Stage[GetNowStage()].Rank))
            {
                SaveDataManager.Instance.MainData.Stage[GetNowStage()].Rank = GetGameRank();
            }

            // �N���A���
            SaveDataManager.Instance.MainData.Stage[GetNowStage()].Clear = true;    // �X�e�[�W�N���A���Z�[�u


            // ���͂����f�[�^�����S�ɃZ�[�u����
            SaveDataManager.Instance.SaveData();

            //Debug.LogWarning("�Z�[�u�f�[�^�ɃZ�[�u�F" + SaveDataManager.Instance.GetStageData(GameStateManager.GetNowStage()).Time);
            //Debug.LogWarning("�����N�F" + GetGameRank());
            //Debug.LogWarning("�^�C���F" + GetGameTime());
        }
    }


    //�����ɏ�������������񏑂���
    //�X�e�[�W�Z���N�g�̊J�n�ƃ��g���C���ɏ���������
    //�X�e�[�W�̏��߂���J�n����ꍇ�ɌĂ΂��i�j���̃`�F�b�N�|�C���g�ł͌Ă΂�Ȃ�
    private void InitializeStage()
    {
        Init();
        StageSoundFlag = true;
        CoinManager.Instance.ResetCoin();               // �R�C���̃��Z�b�g
        CheckPointManager.Instance.ResetCheckPoint();   // �`�F�b�N�|�C���g�̃��Z�b�g
    }

    //�X�e�[�W�Z���N�g�������Ƃ��̂�
    public static void LoadStage(int num)
    {
        Debug.Log("���[�h�X�e�[�W");
        Instance.NowStage = num;
        Instance.InitializeStage();
        FadeManager.Instance.FadeStart(Instance.StageNames[Instance.NowStage], FADE_KIND.FADE_SCENECHANGE);
    }

    //�Q�[���I�[�o�[���g���C�y�у��j���[���烊�g���C���Ɏg�p����
    public static void LoadNowStage()
    {
        LoadStage(Instance.NowStage);
    }

    //���U���g���玟�̃X�e�[�W�ɍs���Ƃ��݂̂Ɏg�p����
    public static void LoadNextStage()
    {
        Instance.NowStage = Mathf.Min(GetNowStage() + 1, STAGE_MAX_NUM - 1);
        LoadStage(Instance.NowStage);
    }

    // �X�e�[�W�Z���N�g�Ɉڍs
    public static void LoadStageSelect(bool SoundStop)
    {
        FadeManager.Instance.FadeStageSelect(SoundStop);
    }

    public static void GameOverReloadScene()
    {
        FadeManager.Instance.FadeGameOver();
    }

    public static int GetNowStage()
    {
        return Instance.NowStage;
    }

    public static float GetGameTime()
    {
        return Instance.GameTime;
    }


    public static GAME_RANK GetGameRank()
    {
        return Instance.GameRank;
    }

    private GAME_RANK CalicurateRank(int stage_num)
    {
        int time = (int)GetGameTime();
        GAME_RANK returnRank = GAME_RANK.S;

        if(time <= (int)ClearRankTime[stage_num].S)
        {
            returnRank = GAME_RANK.S;
        }
        else if(time <= (int)ClearRankTime[stage_num].A)
        {
            returnRank = GAME_RANK.A;
        }
        else
        {
            returnRank = GAME_RANK.B;
        }

        return returnRank;
    }

    private void CountDown()
    {
        if(FadeManager.GetNowState() == FADE_STATE.FADE_NONE) {
            if (GameState == GAME_STATE.PLAY)
            {
                GameTime += Time.deltaTime;
                GameTime = Mathf.Clamp(GameTime, 0, MAX_TIME);
            }
        }
    }
}
