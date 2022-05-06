using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [System.NonSerialized] public bool anim_end;
    [System.NonSerialized] public static ResultManager instance;

    // ステージナンバー
    [SerializeField] Text StageNo;

    // UI
    [SerializeField] GameObject UI_Canvas;
    [SerializeField] GameObject Next_UI;
    [SerializeField] GameObject Next_UI_Big;
    [SerializeField] GameObject StageSelect_UI;
    [SerializeField] GameObject StageSelect_UI_Big;

    UI_COMMAND ui_command;


    // デバッグ用
    [Header("以下デバッグコンソール")]
    [SerializeField] bool debug_check;
    [SerializeField][Range(0, 14)] int debug_stageNo;

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
    void Start()
    {
        anim_end = false;
        UI_Canvas.SetActive(false);
        ui_command = UI_COMMAND.NextStage;

        // UIアクティブ初期化
        Next_UI.SetActive(false);
        Next_UI_Big.SetActive(true);
        StageSelect_UI.SetActive(true);
        StageSelect_UI_Big.SetActive(false);
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

        // UI操作
        if(anim_end == true && UI_Canvas.activeSelf == false)
        {
            UI_Canvas.SetActive(true);
        }

        // アニメーションが終わっていたらUI操作可能
        if (anim_end == true)
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
                    if (Input.GetButton("Fire1") || Input.GetButton("Jump"))
                    {
                        GameStateManager.LoadNextStage();
                    }
                    break;

                case UI_COMMAND.StageSelect:
                    if (Input.GetButton("Fire1") || Input.GetButton("Jump"))
                    {
                        SceneManager.LoadScene("StageSelectScene");
                    }
                    break;
            }
        }
    }
}
