using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Q�[���̏��
/// </summary>
public enum GAME_STATE {
    PLAY,   //�Q�[���ł�����
    PAUSE,  //�|�[�Y��
}


/// <summary>
/// �Q�[���̏�Ԃ��Ǘ�
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
        DontDestroyOnLoad(this.gameObject); // �V�[�����ς���Ă����ȂȂ�
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
