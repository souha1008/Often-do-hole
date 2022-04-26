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
/// ゲームの状態を管理
/// </summary>
public class GameStateManager : SingletonMonoBehaviour<GameSpeedManager>
{
    public static GAME_STATE GameState;

    private void Awake()
    {
        SetGameState(GAME_STATE.PLAY);

        if (this != Instance)
        {
            Destroy(this);
            SetGameState(GAME_STATE.PLAY);
            return;
        }
        DontDestroyOnLoad(this.gameObject); // シーンが変わっても死なない
    }


    private void Update()
    {
        Debug.Log(GetGameState());
    }

    public static GAME_STATE GetGameState()
    {
        return GameState;
    }

    public static void SetGameState(GAME_STATE game_state)
    {
        GameState = game_state;
    }

}
