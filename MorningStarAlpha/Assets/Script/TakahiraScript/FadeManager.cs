using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [Header("フェードにかかる秒数(ゲームオーバー)")]
    [SerializeField] private float FadeTime_GameOver = 1;

    // フェード秒数(シーン変更)
    [Header("フェードにかかる秒数(シーン変更)")]
    [SerializeField] private float FadeTime_SceneChange = 1;

    // フェード秒数(ステージ変更)
    [Header("フェードにかかる秒数(ステージ変更)")]
    [SerializeField] private float FadeTime_StageChange = 1;


    private Texture2D FadeTexture;                    // フェードのテクスチャ
    private float FadeRate;                           // フェード係数
    private FADE_STATE NowFadeState;                  // 現在のフェードの状態
    private FADE_STATE OldFadeState;                  // ひとつ前のフェードの状態
    private FADE_KIND NowFadeKind;                    // 現在のフェードの種類
    private Color FadeColor;                          // フェードのカラー

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject); // シーンが変わっても死なない

        // 初期化
        NowFadeState = OldFadeState = FADE_STATE.FADE_NONE;
        NowFadeKind = FADE_KIND.FADE_GAMOVER;
        FadeRate = Time.fixedDeltaTime / FadeTime_GameOver;
        FadeColor = new Color(0, 0, 0, 0);

        //テクスチャ作成
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

    private void Start() { }

    private void FixedUpdate()
    {
        // フェード処理中
        if (NowFadeState != FADE_STATE.FADE_NONE)
        {
            if (NowFadeState == FADE_STATE.FADE_OUT)
            {// フェードアウト処理
                FadeColor.a += FadeRate;     // α値を加算して画面を消していく

                if (FadeColor.a >= 1.0f)
                {
                    // フェードイン処理に切り替え
                    FadeColor.a = 1.0f;
                    NowFadeState = FADE_STATE.FADE_IN;
                }
            }
            else if (NowFadeState == FADE_STATE.FADE_IN)
            {// フェードイン処理
                FadeColor.a -= FadeRate;     // α値を減算して画面を浮き上がらせる
                if (FadeColor.a <= 0.0f)
                {
                    // フェード処理終了
                    FadeColor.a = 0.0f;
                    NowFadeState = FADE_STATE.FADE_NONE;
                }
            }
        }

        // フェードイン時の処理
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

        OldFadeState = NowFadeState; // ひとつ前のフェードに現在のフェード入力
    }


    // フェードインしたときの処理(ゲームオーバー)
    private void FadeIn_GameOver()
    {
        // チェックポイントに復帰処理
        CheckPointManager.Instance.CheckPointAction();
    }
    // フェードインしたときの処理(シーン変更)
    private void FadeIn_SceneChange()
    {
        // ※後で別の処理に任せる予定
    }
    // フェードインしたときの処理(ステージ変更)
    private void FadeIn_StageChange()
    {
        // ※後で別の処理に任せる予定
    }

    // フェード変更
    //
    // 引数１：フェード状態の種類(今のところフェードアウトのみ)
    // 引数２：何のフェードか(ゲームオーバー,シーンチェンジetc...)
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
