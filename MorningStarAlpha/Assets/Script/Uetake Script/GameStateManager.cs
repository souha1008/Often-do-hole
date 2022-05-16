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

public class CLEAR_RANK_TIME {
    public CLEAR_RANK_TIME(float s , float a , float b){ S = s; A = a; B = b;}

    public float S;
    public float A;
    public float B;

}



/// <summary>
/// ゲームの状態を管理
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
    private bool firstEnter = true; //stage入ったときtrue,いちど死ぬとfalse

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

    private void Start()
    {
       
    }

    private void Init()
    {
        GameTime = 0.0f;
        GameRank = GAME_RANK.S;
        SetGameState(GAME_STATE.PLAY);
    }

    //プレイヤーで呼んでいる
    public void PlayGameStageBGM()
    {
        if (StageSoundFlag)
        {
            SoundManager.Instance.StopSound();

            //後にステージ連番によって分岐
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

        // ゲームクリア時の処理
        if(Instance.GameState == GAME_STATE.RESULT)
        {
            // コイン
            CoinManager.Instance.SetCheckPointCoinData();   // コインデータ更新
            CoinManager.Instance.SetCoinSaveData();         // コインデータセーブ
            //CoinManager.Instance.ResetCoin();               // コイン情報リセット

            // チェックポイント
            CheckPointManager.Instance.ResetCheckPoint();   // チェックポイントのリセット


            // ランク
            Instance.CalicurateRank(GetNowStage());

            // ※　時間をセーブデータにセーブする
            SaveDataManager.Instance.MainData.Stage[GetNowStage()].Time = GetGameTime();
            // ※　ランクをセーブデータにセーブする
            SaveDataManager.Instance.MainData.Stage[GetNowStage()].Rank = Instance.GameRank;

            // クリア情報
            SaveDataManager.Instance.MainData.Stage[GetNowStage()].Clear = true;    // ステージクリア情報セーブ


            // 入力したデータを完全にセーブする
            SaveDataManager.Instance.SaveData();
        }
    }


    //ここに初期化したい情報書いて
    //ステージセレクトの開始とリトライ時に情報を初期化
    //ステージの初めから開始する場合に呼ばれる（針等のチェックポイントでは呼ばれない
    private void InitializeStage()
    {
        Init();
        StageSoundFlag = true;
        firstEnter = true;
        CoinManager.Instance.ResetCoin();               // コインのリセット
        CheckPointManager.Instance.ResetCheckPoint();   // チェックポイントのリセット
    }

    //ステージセレクトから入るときのみ
    public static void LoadStage(int num)
    {
        Instance.NowStage = num;
        Instance.InitializeStage();
        FadeManager.Instance.FadeStart(Instance.StageNames[Instance.NowStage], FADE_KIND.FADE_SCENECHANGE);
    }

    //ゲームオーバーリトライ及びメニューからリトライ時に使用する
    public static void LoadNowStage()
    {
        LoadStage(Instance.NowStage);
    }

    //リザルトから次のステージに行くときのみに使用する
    public static void LoadNextStage()
    {
        Instance.NowStage = Mathf.Min(GetNowStage() + 1, STAGE_MAX_NUM - 1);
        LoadStage(Instance.NowStage);
    }

    // ステージセレクトに移行
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
