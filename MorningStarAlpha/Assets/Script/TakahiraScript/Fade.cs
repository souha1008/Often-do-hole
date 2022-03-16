using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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


public class Fade : MonoBehaviour
{
    // 変数
    public GameObject Player;                         // プレイヤーオブジェクト
    public Image FadeImage;                           // フェードのイメージ
    public const float FadeRate_GameOver = 0.04f;     // フェード係数(ゲームオーバー)
    public const float FadeRate_SceneChange = 0.02f;  // フェード係数(シーン変更)
    public const float FadeRate_StageChange = 0.02f;  // フェード係数(ステージ変更)
    private float FadeRate;                           // フェード係数
    private FADE_STATE NowFadeState;                  // 現在のフェードの状態
    private FADE_STATE OldFadeState;                  // ひとつ前のフェードの状態
    private FADE_KIND NowFadeKind;                    // 現在のフェードの種類
    private Color FadeColor;                          // フェードのカラー

    private void Start()
    {
        // 初期化
        NowFadeState = OldFadeState = FADE_STATE.FADE_NONE;
        NowFadeKind = FADE_KIND.FADE_GAMOVER;
        FadeRate = FadeRate_GameOver;
        FadeColor = new Color(0, 0, 0, 0);

        DontDestroyOnLoad(this.gameObject);
    }

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

            FadeImage.color = FadeColor; // カラー変更
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
        // 後でチェックポイント処理に任せる予定
        Player.GetComponent<Transform>().position = new Vector3(0, 10, 0);
        Player.GetComponent<PlayerMain>().vel = new Vector3(0, 0, 0);
    }
    // フェードインしたときの処理(シーン変更)
    private void FadeIn_SceneChange()
    {
        // 後で別の処理に任せる予定
    }
    // フェードインしたときの処理(ステージ変更)
    private void FadeIn_StageChange()
    {
        // 後で別の処理に任せる予定
    }


    public FADE_STATE GetNowState()
    {
        return NowFadeState;
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
