using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    public static SelectManager instance;

    [SerializeField] public GameObject[] StageObj;
    [SerializeField] private GameObject SelectObj;
    [SerializeField] private float PosDistance;

    string[] SceneName = { "Stage1-2" , "Stage1-3" , "Stage1-4" , "Stage1-5" , "Stage1-6",
    "Stage2-1","Stage2-2","Stage2-3","Stage2-4","Stage2-5",
    "Stage3-1","Stage3-2","Stage3-3","Stage3-4","Stage3-5"};

    int Max;
    public int NowStage = 0;
    int OldStage = -1;

    float StickX;

    bool RightPushFlag = false;
    bool LeftPushFlag = false;
    int RightPushCnt = 0;
    int LeftPushCnt = 0;

    public int LONG_PUSH_COOL_TIME;
    public int LONG_PUSH_PITCH;

    int CanStage = 14;//えらべるステージ

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        Max = StageObj.Length - 1;
        //NowStage = 0;
        OldStage = -1;

        RightPushFlag = false;
        LeftPushFlag = false;

        RightPushCnt = 0;
        LeftPushCnt = 0;


        ChangeStageCheck();

        //無理矢理スタートを呼びます
        StageSelectCamera.instance.ManualStart();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        StickX = Input.GetAxis("Horizontal");
        OldStage = NowStage;

        //ここに更新処理
        if(PushRightMomentOrLongPush())//今押した or 長押しいい感じ
        {
            //関数空呼び
            PushLeftMomentOrLongPush();

            NowStage++;
        }
        else if(PushLeftMomentOrLongPush())//今押した or 長押しいい感じ
        {
            NowStage--;
        }


        NowStage = Mathf.Clamp(NowStage, 0, CanStage);

        //最後にチェック
        ChangeStageCheck();
    }

    void ChangeStageCheck()
    {
        if(NowStage != OldStage)
        {
            Vector3 TempPos = StageObj[NowStage].transform.position;
            TempPos.y += PosDistance;

            SelectObj.transform.position = TempPos;
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

    //ステージクリア時に呼んで情報セット
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

}
