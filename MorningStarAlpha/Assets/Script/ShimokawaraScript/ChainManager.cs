using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainManager : MonoBehaviour
{
    [SerializeField] private PlayerMain Player;
    [SerializeField] private BulletMain Bullet;

    static public ChainManager instance;
    // 
    int MAX_CHAIN_NUM = 20;
    GameObject Obj;
    GameObject[] Chain = new GameObject[20];
    MeshRenderer[] ChainMesh = new MeshRenderer[20];

    public Vector3 PlayerAngle = Vector3.zero;//渡す用のプレイヤー回転角

    float MaxChain;
    float OneChainLength;
    float LocalSize;
    int SideCnt = 0;//右が＋ 左がマイナス
    int MAX_SIDE_CNT = 40;
    float MAX_VEZIE_DISTANCE = 4;

    void Start()
    {
        instance = this;
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
        if (BulletMain.instance.NowBulletState != EnumBulletState.READY)
        {
            //始点と終点を探索
            Vector3 BulletPos = Bullet.transform.position;
            Vector3 PlayerPos = Player.transform.position;
            float NowLength = (PlayerPos - BulletPos).magnitude;
            //Debug.Log(NowLength);

            //何個の鎖が必要？
            int ChainNum = 0;
            ChainNum = (int)((NowLength + (OneChainLength / 2)) / OneChainLength) + 1;

            //状態による変更
            //振り子なら
            if (Player.refState == EnumPlayerState.SWING)
            {

                //プレイヤーのほうが右にいる
                if (BulletPos.x < PlayerPos.x)
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
                    if (Chain[i] == null)
                    {
                        Chain[i] = Instantiate(Obj);
                        ChainMesh[i] = Chain[i].GetComponent<MeshRenderer>();
                    }
                    else
                    {
                        ChainMesh[i].enabled = true;
                    }

                    //大きさ
                    Chain[i].transform.localScale = new Vector3(LocalSize * 0.6f, LocalSize * 0.6f, LocalSize * 0.6f);

                    //////ここから移動、回転
                    //振り子なら
                    if (Player.refState == EnumPlayerState.SWING)
                    {
                        float BtoP_angle = Mathf.Atan2(PlayerPos.y - BulletPos.y, PlayerPos.x - BulletPos.x);

                        if (BtoP_angle < 0)//右から上に一周に直す
                            BtoP_angle += (Mathf.PI * 2);

                        int Hugou;//符号
                        if (BtoP_angle < Mathf.PI * 0.5f || BtoP_angle > Mathf.PI * 1.5f)
                            Hugou = 1;
                        else
                            Hugou = -1;

                        float Difference = Mathf.Abs( BtoP_angle - (Mathf.PI * 1.5f)); //真下からの変化量
                        if(Difference > Mathf.PI)
                            Difference-= Mathf.PI;

                        float multi = 0.5f;//角度何倍？
                        float CenterWariai = 0.6f;//Bulletからなんわり？

                        float BtoC_angle = (Mathf.PI * 1.5f) + (Difference * multi * Hugou);//差にかける符号

                        Vector3 CenterPos = new Vector3(BulletPos.x + Mathf.Cos(BtoC_angle) * NowLength * CenterWariai, BulletPos.y + Mathf.Sin(BtoC_angle) * NowLength * CenterWariai, 0);


                        Chain[i].transform.position = Vezie.Vezie_3(BulletPos, CenterPos, PlayerPos, (float)(i) / Mathf.Max((ChainNum - 1) , 1));



                        Vector3 AngleStart;
                        Vector3 AngleEnd;

                        //角度
                        if (i == 0)
                        {
                            AngleStart = Vezie.Vezie_3(BulletPos, CenterPos, PlayerPos, (float)(i) / Mathf.Max((ChainNum - 1) , 1));
                            AngleEnd = Vezie.Vezie_3(BulletPos, CenterPos, PlayerPos, (float)(i + 1) / Mathf.Max((ChainNum - 1) , 1));
                        }
                        else if(i == ChainNum - 1)
                        {
                            AngleStart = Vezie.Vezie_3(BulletPos, CenterPos, PlayerPos, (float)(i - 1) / Mathf.Max((ChainNum - 1) , 1));
                            AngleEnd = Vezie.Vezie_3(BulletPos, CenterPos, PlayerPos, (float)(i) / Mathf.Max((ChainNum - 1) , 1));

                            //プレイヤーに渡すやつ
                            PlayerAngle = AngleStart - AngleEnd;
                        }
                        else
                        {
                            AngleStart = Vezie.Vezie_3(BulletPos, CenterPos, PlayerPos, (float)(i - 1) / Mathf.Max((ChainNum - 1) , 1));
                            AngleEnd = Vezie.Vezie_3(BulletPos, CenterPos, PlayerPos, (float)(i + 1) / Mathf.Max((ChainNum - 1) , 1));
                        }


                        float Radian = Mathf.Atan2(AngleEnd.y - AngleStart.y, AngleEnd.x - AngleStart.x);
                        float Do = Radian / Mathf.PI * 180 + 90;
                        if (i % 2 == 0)
                        {
                            Chain[i].transform.rotation = Quaternion.Euler(0, 0, Do);
                        }
                        else
                        {
                            Chain[i].transform.rotation = Quaternion.Euler(0, 0, Do) * Quaternion.Euler(0, -90, 0);
                        }

                    }
                    //振り子じゃない
                    else
                    {
                        Chain[i].transform.position = Vector3.Lerp(BulletPos, PlayerPos, (float)i / Mathf.Max((ChainNum - 1) , 1));

                        float Radian = Mathf.Atan2(PlayerPos.y - BulletPos.y, PlayerPos.x - BulletPos.x);
                        float Do = Radian / Mathf.PI * 180 + 90;
                        if (i % 2 == 0)
                        {
                            Chain[i].transform.rotation = Quaternion.Euler(0, 0, Do);
                        }
                        else
                        {
                            Chain[i].transform.rotation =  Quaternion.Euler(0, 0, Do) * Quaternion.Euler(0, -90, 0);
                        }
                    }
                    //////ここまで移動、回転
                }
                else
                {
                    if (Chain[i])
                    {
                        ChainMesh[i].enabled = false;
                        //Destroy(Chain[i]);
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
                    ChainMesh[i].enabled = false;
                    //Destroy(Chain[i]);
                }
            }
        }
    }
}