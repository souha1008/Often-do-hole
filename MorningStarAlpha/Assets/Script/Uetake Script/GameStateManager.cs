using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Q�[���̏��
/// </summary>
enum GAME_STATE {
    PLAY,   //�Q�[���ł�����
    PAUSE,  //�|�[�Y��
}


/// <summary>
/// �Q�[���̏�Ԃ��Ǘ�
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

        DontDestroyOnLoad(this.gameObject); // �V�[�����ς���Ă����ȂȂ�
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
