using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainManager : MonoBehaviour
{
    [SerializeField] private PlayerMain Player;
    [SerializeField] private BulletMain Bullet;

    // 
    int MAX_CHAIN_NUM = 20;
    GameObject Obj;
    GameObject[] Chain = new GameObject[20];


    float MaxChain;
    float OneChainLength;
    float LocalSize;
    int SideCnt = 0;//右が＋ 左がマイナス
    int MAX_SIDE_CNT = 40;
    float MAX_VEZIE_DISTANCE = 4;

    void Start()
    {
        Obj = (GameObject)Resources.Load("_05_chain_o5bj");
        MaxChain = Bullet.GetComponent<BulletMain>().BULLET_ROPE_LENGTH;
        OneChainLength = MaxChain / (MAX_CHAIN_NUM - 1);
        //Debug.Log(OneChainLength);
        LocalSize = OneChainLength / Obj.transform.lossyScale.y;
        //Debug.Log(Sample.transform.lossyScale);
        //lr.SetPosition(0, Player.transform.position);
        //lr.SetPosition(1, Player.transform.position);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Player.refState == EnumPlayerState.SHOT || Player.refState == EnumPlayerState.SWING)
        {
            //始点と終点を探索
            Vector3 StartPos = Bullet.transform.position;
            Vector3 EndPos = Player.transform.position;
            float NowLength = (EndPos - StartPos).magnitude;
            //Debug.Log(NowLength);

            //何個の鎖が必要？
            int ChainNum = 0;
            ChainNum = (int)((NowLength + (OneChainLength / 2)) / OneChainLength) + 1;

            //状態による変更
            //振り子なら
            if (Player.refState == EnumPlayerState.SWING)
            {

                //プレイヤーのほうが右にいる
                if (StartPos.x < EndPos.x)
                {
                    SideCnt = Mathf.Max(SideCnt - 1, -MAX_SIDE_CNT);
                }
                else
                {
                    SideCnt = Mathf.Min(SideCnt + 1, MAX_SIDE_CNT);
                }

#if false
                //左
                if (Player.dir == PlayerMoveDir.LEFT)
                {
                    //プレイヤーのほうが右にいる
                    if(StartPos.x < EndPos.x)
                    {
                        SideCnt = Mathf.Max(SideCnt - 1, -MAX_SIDE_CNT);
                    }
                    else
                    {
                        SideCnt = Mathf.Min(SideCnt + 1, MAX_SIDE_CNT);
                    }
                }
                //右
                else
                {
                    //プレイヤーのほうが左にいる
                    if(StartPos.x > EndPos.x)
                    {
                        SideCnt = Mathf.Min(SideCnt + 1, MAX_SIDE_CNT);
                    }
                    else
                    {
                        SideCnt = Mathf.Max(SideCnt - 1, -MAX_SIDE_CNT);
                    }
                }
#endif
            }
            //振り子じゃない
            else
            {
                SideCnt = 0;

            }

            //まだなければ足す 、 要らなければ消す

            for (int i = 0; i < MAX_CHAIN_NUM; i++)
            {
                if (i < ChainNum)
                {
                    if (!Chain[i])
                    {
                        Chain[i] = Instantiate(Obj);
                    }

                    //大きさ
                    Chain[i].transform.localScale = new Vector3(LocalSize * 0.6f, LocalSize * 0.6f, LocalSize * 0.6f);

                    //////ここから移動、回転
                    //振り子なら
                    if (Player.refState == EnumPlayerState.SWING)
                    {
                        //Vector3 BtoP_angle = 

                        Vector3 CenterPos = Vector3.Lerp(StartPos, EndPos, 0.5f);
                        Vector3 HousenVector = new Vector3(StartPos.y - EndPos.y, -(StartPos.x - EndPos.x), 0).normalized;//正規化した右へのベクトル
                        CenterPos += (HousenVector * (MAX_VEZIE_DISTANCE * ((float)(SideCnt) / MAX_SIDE_CNT)));

                        //なぞのPosition
                        //Vector3 TempEnd = StartPos + (StartPos - EndPos);

                        Chain[i].transform.position = Vezie.Vezie_3(StartPos, CenterPos, EndPos, (float)(i) / (ChainNum - 1));

                        float Radian = Mathf.Atan2(EndPos.y - StartPos.y, EndPos.x - StartPos.x);
                        float Do = Radian / Mathf.PI * 180 + 90;
                        if (i % 2 == 0)
                        {
                            Chain[i].transform.rotation = Quaternion.Euler(0, 0, Do);
                        }
                        else
                        {
                            Chain[i].transform.rotation = Quaternion.Euler(0, 90, Do);
                        }

                    }
                    //振り子じゃない
                    else
                    {
                        Chain[i].transform.position = Vector3.Lerp(StartPos, EndPos, (float)i / (ChainNum - 1));

                        float Radian = Mathf.Atan2(EndPos.y - StartPos.y, EndPos.x - StartPos.x);
                        float Do = Radian / Mathf.PI * 180 + 90;
                        if (i % 2 == 0)
                        {
                            Chain[i].transform.rotation = Quaternion.Euler(0, 0, Do);
                        }
                        else
                        {
                            Chain[i].transform.rotation = Quaternion.Euler(0, 90, Do);
                        }
                    }
                    //////ここまで移動、回転
                }
                else
                {
                    if (Chain[i])
                    {
                        Destroy(Chain[i]);
                    }
                }
            }


        }
        else
        {
            for (int i = 0; i < MAX_CHAIN_NUM; i++)
            {
                if (Chain[i])
                {
                    Destroy(Chain[i]);
                }
            }
        }
    }
}