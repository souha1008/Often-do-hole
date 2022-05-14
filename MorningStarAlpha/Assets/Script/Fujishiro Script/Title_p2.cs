using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Title_p2 : MonoBehaviour
{
    [Header("ヒエラルキーよりアタッチ")]
    [SerializeField] Animator PressAny_animator;
    [SerializeField] int SceneChange_Frame = 200;

    [System.NonSerialized] public static Title_p2 instance;

    private bool once_press;

    public bool changeScene;

    // アニメーション関係
    int PushButton;
    int Anim_StateIdle2;
    AnimatorStateInfo Anim_CurrentState_Info;

    // フレームカウント
    int frame_Count;


    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        once_press = false;
        PlayerMain.instance.mode = new PlayerState_Title();

        frame_Count = 0;

        // 変数初期化
        changeScene = false;

        // アニメーションパラメータ取得
        PushButton = Animator.StringToHash("PushButton");
        Anim_StateIdle2 = Animator.StringToHash("PressAny_Idle2anim");

        SoundManager.Instance.PlaySound("Title_BGM", 100, 1.8f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Anim_CurrentState_Info = PressAny_animator.GetCurrentAnimatorStateInfo(0);

        // PressAnyButton
        if (Input.GetButton("Fire1") || Input.GetButton("Fire2")
            || Input.GetButton("Fire3") || Input.GetButton("Jump") && once_press == false)
        { 
            once_press = true;
            PressAny_animator.SetBool(PushButton, true);
        }

        if (once_press == true)
        {
            if(CheckCurrent_PressAnyIdle() == true)
            {
                frame_Count++;
            }
            if (frame_Count >= SceneChange_Frame)
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }

    bool CheckCurrent_PressAnyIdle()
    {
        bool isState = Anim_CurrentState_Info.shortNameHash == Anim_StateIdle2;

        return isState;
    }
}
