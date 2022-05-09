using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームの状態
/// </summary>
public enum GAME_STATE {
    PLAY,   //ゲームできる状態
    PAUSE,  //ポーズ中
    RESULT,//リザルト中
}

public enum GAME_RANK
{
    S,
    A,
    B,
}


/// <summary>
/// ゲームの状態を管理
/// </summary>
public class GameStateManager : SingletonMonoBehaviour<GameStateManager>
{
    private const int STAGE_MAX_NUM = 16;
    private const float MAX_TIME = 999.0f;
    private string[] StageNames = { "Stage1-1" ,"Stage1-2" , "Stage1-3" , "Stage1-4" , "Stage1-5" , "Stage1-6",
    "Stage2-1","Stage2-2","Stage2-3","Stage2-4","Stage2-5",
    "Stage3-1","Stage3-2","Stage3-3","Stage3-4","Stage3-5"};


    private GAME_STATE GameState;
    private float GameTime;
    private GAME_RANK GameRank;
    private int NowStage = 0;

    private void Awake()
    {
        Init();

        if (this != Instance)
        {
            Destroy(this);
            SetGameState(GAME_STATE.PLAY);
            return;
        }
        DontDestroyOnLoad(this.gameObject); // シーンが変わっても死なない
    }

    private void Init()
    {
        GameTime = MAX_TIME;
        GameRank = GAME_RANK.S;
        SetGameState(GAME_STATE.PLAY);
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

        if(Instance.GameState == GAME_STATE.RESULT)
        {
            Instance.CalicurateRank();
        }
    }

    public static void LoadStage(int num)
    {
        Instance.NowStage = num;
        FadeManager.Instance.FadeStart(Instance.StageNames[Instance.NowStage], FADE_KIND.FADE_SCENECHANGE);
    }

    //リトライ時に使用
    public static void LoadNowStage()
    {
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

    public GAME_RANK GetGameRank()
    {
        return GameRank;
    }

    public void CalicurateRank()
    {
        //仮

        if(GameTime > 990)
        {
            GameRank = GAME_RANK.S;
        }
        else if(GameTime > 980)
        {
            GameRank = GAME_RANK.A;
        }
        if (GameTime > 970)
        {
            GameRank = GAME_RANK.B;
        }
    }

    private void CountDown()
    {
        if (GameState == GAME_STATE.PLAY)
        {
            GameTime -= Time.deltaTime;
            GameTime = Mathf.Clamp(GameTime, 0, MAX_TIME);
        }

        //Debug.Log(GameTime);
    }
}
