using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainManager : MonoBehaviour
{
    [SerializeField] private PlayerMain Player;
    [SerializeField] private BulletMain Bullet;

    // 
    int MAX_CHAIN_NUM = 20;
    GameObject Obj ;
    GameObject[] Chain = new GameObject[20];


    float MaxChain;
    float OneChainLength;
    float LocalSize;
    

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

            //まだなければ足す 、 要らなければ消す
            for(int i = 0; i <  MAX_CHAIN_NUM; i++ )
            {
                if(i < ChainNum)
                {
                    if(!Chain[i])
                    {
                        Chain[i] = Instantiate(Obj);
                    }

                    //大きさ
                    //float Temp = OneChainLength / Chain[i].transform.lossyScale.y;
                    Chain[i].transform.localScale = new Vector3(LocalSize * 0.6f, LocalSize * 0.6f, LocalSize * 0.6f);
                    //Debug.Log(LocalSize);

                    //移動、回転
                    Chain[i].transform.position = Vector3.Lerp(StartPos, EndPos, (float) i / (MAX_CHAIN_NUM - 1));

                    float Radian = Mathf.Atan2(EndPos.y - StartPos.y, EndPos.x - StartPos.x);
                    float Do = Radian / Mathf.PI * 180 + 90;
                    if(i % 2 == 0)
                    {
                        Chain[i].transform.rotation = Quaternion.Euler(0, 0, Do);
                    }
                    else
                    {
                        Chain[i].transform.rotation = Quaternion.Euler(0, 90, Do);
                    }
                    //Chain[i].transform.rotation = Quaternion.Euler(0,0, Do);



                }
                else
                {
                    if(Chain[i])
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
                if(Chain[i])
                {
                    Destroy(Chain[i]);
                }
            }
        }
    }
}