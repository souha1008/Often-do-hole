using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    public static SelectManager instance;

    [SerializeField] public GameObject[] StageObj;
    //[SerializeField] private GameObject SelectObj;
    [SerializeField] private float PosDistance;

#if false
    string[] SceneName = { "Stage1-2" , "Stage1-3" , "Stage1-4" , "Stage1-5" , "Stage1-6",
    "Stage2-1","Stage2-2","Stage2-3","Stage2-4","Stage2-5",
    "Stage3-1","Stage3-2","Stage3-3","Stage3-4","Stage3-5"};
#endif

    //int Max;
    public int NowSelectStage = 0;
    int OldSelectStage = -1;

    float StickX;

    bool RightPushFlag = false;
    bool LeftPushFlag = false;
    int RightPushCnt = 0;
    int LeftPushCnt = 0;

    public int LONG_PUSH_COOL_TIME;
    public int LONG_PUSH_PITCH;

    int CanStage = 0;//えらべるステージ
    bool CanStart = true;//移動中は開始できない


    public bool DEBUG_ALL_STAGE_SELECT;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        //Max = StageObj.Length - 1;
        NowSelectStage = GameStateManager.GetNowStage();
        OldSelectStage = -1;

        RightPushFlag = false;
        LeftPushFlag = false;

        RightPushCnt = 0;
        LeftPushCnt = 0;

        CanStart = true;

        SerchCanStage();
        ChangeStageCheck();

        //無理矢理スタートを呼びます
        StageSelectCamera.instance.ManualStart();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        StickX = Input.GetAxis("Horizontal");
        OldSelectStage = NowSelectStage;

        //ここに更新処理
        if(PushRightMomentOrLongPush())//今押した or 長押しいい感じ
        {
            //関数空呼び
            PushLeftMomentOrLongPush();

            NowSelectStage++;
        }
        else if(PushLeftMomentOrLongPush())//今押した or 長押しいい感じ
        {
            NowSelectStage--;
        }


        NowSelectStage = Mathf.Clamp(NowSelectStage, 0, CanStage);

        //最後にチェック
        ChangeStageCheck();

        //ステージ侵入
        if(CanStart && Input.GetKey(KeyCode.Return))
        {
            GameStateManager.LoadStage(NowSelectStage);
            
        }
        //Debug.Log(CanStart);
    }

    void ChangeStageCheck()
    {
        if(NowSelectStage != OldSelectStage)
        {
            //変わったタイミングはスタート受け付けない
            CanStart = false;

            Vector3 TempPos = StageObj[NowSelectStage].transform.position;
            TempPos.y += PosDistance;

            //SelectObj.transform.position = TempPos;
        }
    }

    bool PushRight()
    {
        if(StickX > 0.7f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool PushRightMomentOrLongPush()//押した瞬間と長押しでいい感じに帰ってくる
    {
        //押してる
        if(PushRight())
        {
            //さっき押してない
            if (RightPushFlag == false)
            {
                RightPushCnt++;
                RightPushFlag = true;
                return true;
            }
            //さっき押してる
            else
            {
                RightPushCnt++;

                //いい感じのタイミングならtrueを返す
                if((RightPushCnt - LONG_PUSH_COOL_TIME) >= 0 && (RightPushCnt - LONG_PUSH_COOL_TIME) % LONG_PUSH_PITCH == 0)
                {
                    return true;
                }
                //基本false
                return false;
            }
            
        }
        //押してない
        else
        {
            RightPushCnt = 0;
            RightPushFlag = false;
            return false;
        }
    }

    bool PushLeft()
    {
        if (StickX < -0.7f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool PushLeftMomentOrLongPush()//押した瞬間と長押しでいい感じに帰ってくる
    {
        //押してる
        if (PushLeft())
        {
            //さっき押してない
            if (LeftPushFlag == false)
            {
                LeftPushCnt++;
                LeftPushFlag = true;
                return true;
            }
            //さっき押してる
            else
            {
                LeftPushCnt++;

                //いい感じのタイミングならtrueを返す
                if ((LeftPushCnt - LONG_PUSH_COOL_TIME) >= 0 && (LeftPushCnt - LONG_PUSH_COOL_TIME) % LONG_PUSH_PITCH == 0)
                {
                    return true;
                }
                //基本false
                return false;
            }

        }
        //押してない
        else
        {
            LeftPushCnt = 0;
            LeftPushFlag = false;
            return false;
        }
    }

    public void SetCanStart (bool flag)
    {
        CanStart = flag;
    }

    void SerchCanStage()
    {
        if(DEBUG_ALL_STAGE_SELECT)
        {
            CanStage = 10;
        }
        else
        {
            int OldCanStage = -1;//今開放済みステージを格納  
            if (SaveDataManager.Instance)
            {
                for (int i = 0; i < SaveDataManager.Instance.MainData.Stage.Length; i++)
                {
                    //i番目がクリア済みなら
                    if (SaveDataManager.Instance.MainData.Stage[i].Clear)
                        OldCanStage = i;
                    //クリアしてない添え字まで来た
                    else
                        break;
                }
            }

            //クリアしたステージ + 1 が 遊べるステージ   ※1-1未クリアなら (-1 + 1) = 0 番目までが遊べるステージ
            CanStage = OldCanStage + 1;
        }
       

    }

    //ステージクリア時に呼んで情報セット
#if false
    public void SetStage(string scene_name)
    {
        if(scene_name == "Tutrial")
        {
            //一個めのステージまで
            NowStage = CanStage = 0;
        }
        else
        {
            int StageNum = -1;

            for (int i = 0; i < SceneName.Length; i++)
            {
                if (SceneName[i] == scene_name)
                {
                    //引数のシーンはi番目だよ (0から数える)
                    StageNum = i;
                }

            }

            if (StageNum == -1)
            {
                Debug.LogError("ステージ名が悪い");
            }

            //引数のステージを指す用に戻る
            NowStage = StageNum;

            //可能ステージを増やす？
            if (StageNum + 1 > CanStage)//更新必要なら
            {
                CanStage = StageNum + 1;
            }
        }
    }
#endif

}
