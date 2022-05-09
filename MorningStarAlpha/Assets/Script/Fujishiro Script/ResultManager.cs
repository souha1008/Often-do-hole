using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ResultManager : MonoBehaviour
{
    [System.NonSerialized] public bool anim_end;
    [System.NonSerialized] public static ResultManager instance;

    // カメラ
    [Header("手配書揺れ関係")]
    [SerializeField] Image Wanted_Sprite;
    [SerializeField][Range(0.0f, 1.0f)] float duration;
    [SerializeField] float strength;
    [SerializeField] int vibrato;
    [SerializeField] float randomness;
    [SerializeField] bool snapping;
    [SerializeField] bool fadeOut;

    Tweener shaketeener; // DOTweenのやつ
    Vector3 initPos;    // 手配書の初期位置

    // ステージナンバー
    [Header("ステージナンバー")]
    [SerializeField] Text StageNo;

    // UI
    [Header("UI")]
    [SerializeField] GameObject UI_Canvas;
    [SerializeField] GameObject Next_UI;
    [SerializeField] GameObject Next_UI_Big;
    [SerializeField] GameObject StageSelect_UI;
    [SerializeField] GameObject StageSelect_UI_Big;
    [SerializeField] Image Stump_Sprite;

    UI_COMMAND ui_command;

    // アニメータ変数
    [Header("アニメータ")]
    public Animator stump_animator;
    public Animator Wanted_animator;

    // アニメーションパラメータ
    [System.NonSerialized]public int Stump_Start;
    [System.NonSerialized]public int Stump_end;
    [System.NonSerialized]public int Shake_Start;
    [System.NonSerialized] public int Shake_End;

    // クリアランク用
    Sprite[] Stump_sprite;

    // デバッグ用
    [Header("以下デバッグコンソール")]
    [SerializeField] bool debug_check;
    [SerializeField][Range(0, 14)] int debug_stageNo;
    [SerializeField] bool debug_cicktime;
    [SerializeField] int debug_cickFlame;

    enum ClearRank
    {
        Rank_S = 0,
        Rank_A,
        Rank_B,
    }
    [SerializeField] ClearRank clearRank;

    enum UI_COMMAND
    {
        NextStage = 0,
        StageSelect
    };

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    async void Start()
    {
        anim_end = false;
        UI_Canvas.SetActive(false);
        ui_command = UI_COMMAND.NextStage;

        // UIアクティブ初期化
        Next_UI.SetActive(false);
        Next_UI_Big.SetActive(true);
        StageSelect_UI.SetActive(true);
        StageSelect_UI_Big.SetActive(false);

        Stump_Sprite.color = new Color(0f, 0f, 0f, 0f);
        Debug.Log(Stump_Sprite.color.a);

        initPos = Wanted_Sprite.transform.position;

        // アニメパラメータハッシュ
        Stump_Start = Animator.StringToHash("Stump_Start");
        Stump_end = Animator.StringToHash("Stump_End");
        Shake_Start = Animator.StringToHash("Shake_Start");
        Shake_End = Animator.StringToHash("Shake_End");
    }

    // Update is called once per frame
    void Update()
    {
            // デバッグ用
            if (debug_check)
            {
                switch (debug_stageNo)
                {
                    case 0:
                        StageNo.text = "1-1";
                        break;

                    case 1:
                        StageNo.text = "1-2";
                        break;

                    case 2:
                        StageNo.text = "1-3";
                        break;

                    case 3:
                        StageNo.text = "1-4";
                        break;

                    case 4:
                        StageNo.text = "1-5";
                        break;

                    case 5:
                        StageNo.text = "2-1";
                        break;

                    case 6:
                        StageNo.text = "2-2";
                        break;

                    case 7:
                        StageNo.text = "2-3";
                        break;

                    case 8:
                        StageNo.text = "2-4";
                        break;

                    case 9:
                        StageNo.text = "2-5";
                        break;

                    case 10:
                        StageNo.text = "3-1";
                        break;

                    case 11:
                        StageNo.text = "3-2";
                        break;

                    case 12:
                        StageNo.text = "3-3";
                        break;

                    case 13:
                        StageNo.text = "3-4";
                        break;

                    case 14:
                        StageNo.text = "3-5";
                        break;
                }
                switch (clearRank)
                {
                    case ClearRank.Rank_S:
                    Stump_Sprite.sprite = Resources.Load<Sprite>("Sprite/ClearRankStump/Stump_S");
                    break;

                    case ClearRank.Rank_A:
                    Stump_Sprite.sprite = Resources.Load<Sprite>("Sprite/ClearRankStump/Stump_A");
                    break;

                    case ClearRank.Rank_B:
                    Stump_Sprite.sprite = Resources.Load<Sprite>("Sprite/ClearRankStump/Stump_B");
                    break;
                }

        }
            else
            {
                // WANTED画像操作
                switch (GameStateManager.GetNowStage())
                {
                    case 0:
                        StageNo.text = "1-1";
                        break;

                    case 1:
                        StageNo.text = "1-2";
                        break;

                    case 2:
                        StageNo.text = "1-3";
                        break;

                    case 3:
                        StageNo.text = "1-4";
                        break;

                    case 4:
                        StageNo.text = "1-5";
                        break;

                    case 5:
                        StageNo.text = "2-1";
                        break;

                    case 6:
                        StageNo.text = "2-2";
                        break;

                    case 7:
                        StageNo.text = "2-3";
                        break;

                    case 8:
                        StageNo.text = "2-4";
                        break;

                    case 9:
                        StageNo.text = "2-5";
                        break;

                    case 10:
                        StageNo.text = "3-1";
                        break;

                    case 11:
                        StageNo.text = "3-2";
                        break;

                    case 12:
                        StageNo.text = "3-3";
                        break;

                    case 13:
                        StageNo.text = "3-4";
                        break;

                    case 14:
                        StageNo.text = "3-5";
                        break;
                }
            }

            if (anim_end == true)
            {
                stump_animator.SetBool(Stump_Start, true);
                Stump_Sprite.color = new Color(1f, 1f, 1f, 1f);
            }

            // UI操作
            if (stump_animator.GetBool(Stump_end) == true && UI_Canvas.activeSelf == false)
            {
                UI_Canvas.SetActive(true);
                Wanted_animator.SetBool(Shake_Start, true);
            }
            //if (stump_animator.GetBool(Shake) == true && UI_Canvas.activeSelf == false)
            //{
            //    UI_Canvas.SetActive(true);
            //}
            //if (UI_Canvas.activeSelf == false && stump_animator.GetBool(Shake_End) == true)
            //{
            //    bool end = ResultManager.instance.Wanted_animator.GetBool(ResultManager.instance.Shake_End);
            //    Debug.Log("Shake_End:" + end);
            //    UI_Canvas.SetActive(true);
            //}

            // アニメーションが終わっていたらUI操作可能
            if (stump_animator.GetBool(Stump_end) == true)
            {
                // スティック上
                if (Input.GetAxis("Vertical") > 0.8f)
                {
                    ui_command = UI_COMMAND.NextStage;
                    Next_UI.SetActive(false);
                    Next_UI_Big.SetActive(true);
                    StageSelect_UI.SetActive(true);
                    StageSelect_UI_Big.SetActive(false);
                }
                // スティック下
                if (Input.GetAxis("Vertical") < -0.8)
                {
                    ui_command = UI_COMMAND.StageSelect;
                    Next_UI.SetActive(true);
                    Next_UI_Big.SetActive(false);
                    StageSelect_UI.SetActive(false);
                    StageSelect_UI_Big.SetActive(true);
                }

                switch (ui_command)
                {
                    case UI_COMMAND.NextStage:
                        if (Input.GetButton("Jump"))
                        {
                            GameStateManager.LoadNextStage();
                        }
                        break;

                    case UI_COMMAND.StageSelect:
                        if (Input.GetButton("Jump"))
                        {
                            SceneManager.LoadScene("StageSelectScene");
                        }
                        break;
                }
            }
        
    }

    void Wanted_Shake(float duration, float strength, int vibrato, float randomness, bool fadeout)
    {
        if(shaketeener != null)
        {
            shaketeener.Kill();
            Wanted_Sprite.transform.position = initPos;
        }

        shaketeener = Wanted_Sprite.rectTransform.DOShakePosition(duration, strength, vibrato, randomness, false, fadeout);
    }
}
