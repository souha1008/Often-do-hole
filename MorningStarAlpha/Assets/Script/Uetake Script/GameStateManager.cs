using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームの状態
/// </summary>
enum GAME_STATE {
    PLAY,   //ゲームできる状態
    PAUSE,  //ポーズ中
}


/// <summary>
/// ゲームの状態を管理
/// </summary>
public class GameStateManager : SingletonMonoBehaviour<GameSpeedManager>
{
    static GAME_STATE GameState;

    private void Awake()
    {
        GameState = GAME_STATE.PLAY;

        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject); // シーンが変わっても死なない
    }


    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
