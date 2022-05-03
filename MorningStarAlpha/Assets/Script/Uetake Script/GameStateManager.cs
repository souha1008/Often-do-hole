using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームの状態
/// </summary>
public enum GAME_STATE {
    PLAY,   //ゲームできる状態
    PAUSE,  //ポーズ中
}


/// <summary>
/// ゲームの状態を管理,時間を管理
/// </summary>
public class GameStateManager : SingletonMonoBehaviour<GameStateManager>
{
    private const float MAX_TIME = 300.0f;
    private static GAME_STATE GameState;
    private static float GameTime;

    private void Awake()
    {
        GameTime = MAX_TIME;
        SetGameState(GAME_STATE.PLAY);

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
        GameTime = 300.0f;
        SetGameState(GAME_STATE.PLAY);
    }

    private void CountDown()
    {
        GameTime = Mathf.Max(0, MAX_TIME - Time.time);
        Debug.Log(GameTime);
    }



    private void Update()
    {
        CountDown();
    }

    public static GAME_STATE GetGameState()
    {
        return GameState;
    }

    public static void SetGameState(GAME_STATE game_state)
    {
        GameState = game_state;
    }

    public static float GetGameTime()
    {
        return GameTime;
    }
}
