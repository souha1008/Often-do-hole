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


/// <summary>
/// �Q�[���̏�Ԃ��Ǘ�
/// </summary>
public class GameStateManager : SingletonMonoBehaviour<GameStateManager>
{
    private const int STAGE_MAX_NUM = 15;
    private const float MAX_TIME = 300.0f;
    private string[] StageNames = new string[STAGE_MAX_NUM];


    private GAME_STATE GameState;
    private float GameTime;
    private int NowStage = 1;

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

    private void Init()
    {
        GameTime = MAX_TIME;
        SetGameState(GAME_STATE.PLAY);
        SetStageName();
    }

    private void SetStageName()
    {
        StageNames[0] = "Stage1-1";
        StageNames[1] = "Stage1-2";
        StageNames[2] = "Stage1-3";

        ///��
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
    }

    public static void LoadStage(int num)
    {
        Instance.NowStage = num;
        FadeManager.Instance.FadeStart(Instance.StageNames[Instance.NowStage], FADE_KIND.FADE_SCENECHANGE);
    }


    public static void LoadNextStage()
    {
        Instance.NowStage = Mathf.Min(GetNowStage() + 1, STAGE_MAX_NUM - 1);
        FadeManager.Instance.FadeStart(Instance.StageNames[Instance.NowStage], FADE_KIND.FADE_SCENECHANGE);
    }

    public static int GetNowStage()
    {
        return Instance.NowStage;
    }

    public static float GetGameTime()
    {
        return Instance.GameTime;
    }

    private void CountDown()
    {
        GameTime -= Time.deltaTime;
        GameTime = Mathf.Clamp(GameTime, 0, MAX_TIME);
        Debug.Log(GameTime);
    }



}
