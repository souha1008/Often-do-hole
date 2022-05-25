using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Title_p2 : MonoBehaviour
{
    [Header("ヒエラルキーよりアタッチ")]
    [Tooltip("PressAnyアニメーターを入れる")][SerializeField] Animator PressAny_animator;
    [Tooltip("PressAnyアニメーション終了後のデュレーション")][SerializeField] int SceneChange_Frame = 200;
    [Tooltip("メニューマネージャーを入れる")][SerializeField] GameObject MenuManager;
    [Tooltip("PressAnyUIを入れる")][SerializeField] GameObject PressAny_GO;
    [Tooltip("SelectUIを入れる")][SerializeField] GameObject Select_GO;


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

        StartCoroutine("WaitAwake");
    }

    IEnumerator WaitAwake()
    {
        yield return new WaitForEndOfFrame();
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

        // Menu関係非アクティブ
        MenuManager.SetActive(false);
        Select_GO.SetActive(false);

        // サウンド再生
        SoundManager.Instance.PlaySound("Title_BGM", 0.5f, 0.5f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Anim_CurrentState_Info = PressAny_animator.GetCurrentAnimatorStateInfo(0);

        // PressAnyButton
        if (Input.GetButton("ButtonA") || Input.GetButton("ButtonB") || Input.GetButton("Button_Select") && once_press == false)
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
                //SceneManager.LoadScene("Menu");
                PressAny_GO.SetActive(false);
                MenuManager.SetActive(true);
                Select_GO.SetActive(true);
                this.gameObject.SetActive(false);
            }
        }
    }

    bool CheckCurrent_PressAnyIdle()
    {
        bool isState = Anim_CurrentState_Info.shortNameHash == Anim_StateIdle2;

        return isState;
    }
}
