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
    private const int STAGE_MAX_NUM = 16;
    private const float MAX_TIME = 999.9f;
    private string[] StageNames = { "Stage1-1_Prod" ,"Stage2-2" , "Stage1-3" , "Stage1-4" , "Stage1-5" , "Stage1-6",
    "Stage2-1","Stage2-2","Stage2-3","Stage2-4","Stage2-5",
    "Stage3-1","Stage3-2","Stage3-3","Stage3-4","Stage3-5"};
    public CLEAR_RANK_TIME[] ClearRank = {
        new CLEAR_RANK_TIME(900,750,300) ,
        new CLEAR_RANK_TIME(900,750,300) ,
        new CLEAR_RANK_TIME(900,750,300) ,
        new CLEAR_RANK_TIME(900,750,300) ,
        new CLEAR_RANK_TIME(900,750,300) ,
        new CLEAR_RANK_TIME(900,750,300) ,
        new CLEAR_RANK_TIME(900,750,300) ,
        new CLEAR_RANK_TIME(900,750,300) ,
        new CLEAR_RANK_TIME(900,750,300) ,
        new CLEAR_RANK_TIME(900,750,300) ,
        new CLEAR_RANK_TIME(900,750,300)
    };

    private GAME_STATE GameState;
    private float GameTime;
    private GAME_RANK GameRank;
    private int NowStage = 0;
    private bool StageSoundFlag = false;
    private bool firstEnter = true; //stage�������Ƃ�true,�����ǎ��ʂ�false

    private void Awake()
    {
        Init();

        if (this != Instance)
        {
            Destroy(this);
            SetGameState(GAME_STATE.PLAY);
            return;
        }
        DontDestroyOnLoad(this.gameObject); // �V�[�����ς���Ă����ȂȂ�
    }

    private void Start()
    {
       
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

            //��ɃX�e�[�W�A�Ԃɂ���ĕ���
            SoundManager.Instance.PlaySound("MorningBGM", 0.6f, AudioReverbPreset.City);
            StageSoundFlag = false;
        }
    }


    private void Update()
    {
        if (GetGameState() == GAME_STATE.PLAY)
        {
            CountDown();
        }
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
            //CoinManager.Instance.ResetCoin();               // �R�C����񃊃Z�b�g

            // �`�F�b�N�|�C���g
            CheckPointManager.Instance.ResetCheckPoint();   // �`�F�b�N�|�C���g�̃��Z�b�g


            // �����N
            Instance.CalicurateRank(GetNowStage());

            // ���@���Ԃ��Z�[�u�f�[�^�ɃZ�[�u����
            SaveDataManager.Instance.MainData.Stage[GetNowStage()].Time = GetGameTime();
            // ���@�����N���Z�[�u�f�[�^�ɃZ�[�u����
            SaveDataManager.Instance.MainData.Stage[GetNowStage()].Rank = Instance.GameRank;

            // �N���A���
            SaveDataManager.Instance.MainData.Stage[GetNowStage()].Clear = true;    // �X�e�[�W�N���A���Z�[�u


            // ���͂����f�[�^�����S�ɃZ�[�u����
            SaveDataManager.Instance.SaveData();
        }
    }


    //�����ɏ�������������񏑂���
    //�X�e�[�W�Z���N�g�̊J�n�ƃ��g���C���ɏ���������
    //�X�e�[�W�̏��߂���J�n����ꍇ�ɌĂ΂��i�j���̃`�F�b�N�|�C���g�ł͌Ă΂�Ȃ�
    private void InitializeStage()
    {
        Init();
        StageSoundFlag = true;
        firstEnter = true;
        CoinManager.Instance.ResetCoin();               // �R�C���̃��Z�b�g
        CheckPointManager.Instance.ResetCheckPoint();   // �`�F�b�N�|�C���g�̃��Z�b�g
    }

    //�X�e�[�W�Z���N�g�������Ƃ��̂�
    public static void LoadStage(int num)
    {
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
    public static void LoadStageSelect()
    {
        FadeManager.Instance.FadeStageSelect();
    }

    public static void GameOverReloadScene()
    {
        Instance.firstEnter = false;
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

    private GAME_RANK CalicurateRank(int stage_num)
    {
        float time = GetGameTime();
        GAME_RANK returnRank = GAME_RANK.S;

        if(time >= ClearRank[stage_num].S)
        {
            returnRank = GAME_RANK.S;
        }
        else if(time >= ClearRank[stage_num].A)
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
        if (GameState == GAME_STATE.PLAY)
        {
            GameTime += Time.deltaTime;
            GameTime = Mathf.Clamp(GameTime, 0, MAX_TIME);
        }
    }
}
