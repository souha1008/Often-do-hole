using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// フェードの状態
public enum FADE_STATE
{
    FADE_NONE = 0,      // フェード処理をしていない
    FADE_IN,            // フェードイン処理中
    FADE_OUT,           // フェードアウト処理中
    //FADE_MAX            // フェード状態最大数
}

// フェードの種類
public enum FADE_KIND
{
    FADE_GAMOVER = 0,      // ゲームオーバーフェード
    FADE_SCENECHANGE,      // シーン変更フェード
    FADE_STAGECHANGE,      // ステージ変更フェード
    //FADE_MAX               // フェードの種類最大数
}


public class FadeManager : SingletonMonoBehaviour<FadeManager>
{
    // 変数

    // フェード秒数(ゲームオーバー)
    [Label("フェード秒数(ゲームオーバー)")]
    [SerializeField] private float FadeTime_GameOver = 1;

    // フェード秒数(シーン変更)
    [Label("フェード秒数(シーン変更)")]
    [SerializeField] private float FadeTime_SceneChange = 1;

    // フェード秒数(ステージ変更)
    [Label("フェード秒数(ステージ変更)")]
    [SerializeField] private float FadeTime_StageChange = 1;


    private Texture2D FadeTexture;                    // フェードのテクスチャ
    static private FADE_STATE NowFadeState;           // 現在のフェードの状態
    private FADE_STATE OldFadeState;                  // ひとつ前のフェードの状態
    private FADE_KIND NowFadeKind;                    // 現在のフェードの種類
    private Color FadeColor;                          // フェードのカラー

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

        DontDestroyOnLoad(this.gameObject); // シーンが変わっても死なない

        // 初期化
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

        //テクスチャ作成
        //Debug.Log("FadeManager作成");

        FadeTexture = new Texture2D(32, 32, TextureFormat.RGB24, false);
        FadeTexture.ReadPixels(new Rect(0, 0, 32, 32), 0, 0, false);
        FadeTexture.SetPixel(0, 0, Color.white);
        FadeTexture.Apply();
    }
    private void OnGUI()
    {
        if (NowFadeState == FADE_STATE.FADE_NONE)
            return;

        //透明度を更新してテクスチャを描画
        GUI.color = FadeColor;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), FadeTexture);
    }

    private void Start() 
    {
        SoundManager.Instance.PlaySound("MorningBGM", 0.6f, AudioReverbPreset.City);
    }

    private void Update()
    {
        // フェード処理中
        if (NowFadeState != FADE_STATE.FADE_NONE)
        {
            if (NowFadeState == FADE_STATE.FADE_OUT)
            {// フェードアウト処理
                switch (NowFadeKind)// α値を加算して画面を消していく
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
                NowTime += Time.unscaledDeltaTime; // 時間加算

                if (FadeColor.a >= 1.0f)
                {
                    // フェードイン処理に切り替え
                    FadeColor.a = 1.0f;
                    NowTime = 0.0f;
                    NowFadeState = FADE_STATE.FADE_IN;
                }
            }
            else if (NowFadeState == FADE_STATE.FADE_IN)
            {// フェードイン処理
                switch (NowFadeKind)// α値を減算して画面を浮き上がらせる
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
                NowTime += Time.unscaledDeltaTime; // 時間加算

                if (FadeColor.a <= 0.0f)
                {
                    // フェード処理終了
                    FadeColor.a = 0.0f;
                    NowTime = 0.0f;
                    NowFadeState = FADE_STATE.FADE_NONE;
                }
            }
        }

        // フェードイン時の処理
        if (OldFadeState == FADE_STATE.FADE_OUT && NowFadeState == FADE_STATE.FADE_IN)
        {
            //シーン終了なので時間をもとに戻す
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

        OldFadeState = NowFadeState; // ひとつ前のフェードに現在のフェード入力
    }


    // フェードインしたときの処理(ゲームオーバー)(ゲームリスタート)
    private void FadeIn_GameOver()
    {
        // ゲームシーンのリセット
        SoundManager.Instance.StopSound("BGM_01");
        SceneManager.LoadScene(NextSceneName);
        SoundManager.Instance.PlaySound("BGM_01", 0.6f, AudioReverbPreset.City);
        GameStateManager.SetGameState(GAME_STATE.PLAY);
        Time.timeScale = 1.0f;
    }
    // フェードインしたときの処理(シーン変更)
    public void FadeIn_SceneChange()
    {
        SceneManager.LoadScene(NextSceneName);
    }

    // フェードインしたときの処理(ステージ変更)
    public void FadeIn_StageChange()
    {
        // ※後で別の処理に任せる予定
    }

    // フェード変更
    //
    // 引数１：フェード状態の種類(今のところフェードアウトのみ)
    // 引数２：何のフェードか(ゲームオーバー,シーンチェンジetc...)
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
