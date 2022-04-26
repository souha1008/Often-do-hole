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
    int SideCnt = 0;//�E���{ �����}�C�i�X
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
            //�n�_�ƏI�_��T��
            Vector3 StartPos = Bullet.transform.position;
            Vector3 EndPos = Player.transform.position;
            float NowLength = (EndPos - StartPos).magnitude;
            //Debug.Log(NowLength);

            //���̍����K�v�H
            int ChainNum = 0;
            ChainNum = (int)((NowLength + (OneChainLength / 2)) / OneChainLength) + 1;

            //��Ԃɂ��ύX
            //�U��q�Ȃ�
            if (Player.refState == EnumPlayerState.SWING)
            {

                //�v���C���[�̂ق����E�ɂ���
                if (StartPos.x < EndPos.x)
                {
                    SideCnt = Mathf.Max(SideCnt - 1, -MAX_SIDE_CNT);
                }
                else
                {
                    SideCnt = Mathf.Min(SideCnt + 1, MAX_SIDE_CNT);
                }

#if false
                //��
                if (Player.dir == PlayerMoveDir.LEFT)
                {
                    //�v���C���[�̂ق����E�ɂ���
                    if(StartPos.x < EndPos.x)
                    {
                        SideCnt = Mathf.Max(SideCnt - 1, -MAX_SIDE_CNT);
                    }
                    else
                    {
                        SideCnt = Mathf.Min(SideCnt + 1, MAX_SIDE_CNT);
                    }
                }
                //�E
                else
                {
                    //�v���C���[�̂ق������ɂ���
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
            //�U��q����Ȃ�
            else
            {
                SideCnt = 0;

            }

            //�܂��Ȃ���Α��� �A �v��Ȃ���Ώ���

            for (int i = 0; i < MAX_CHAIN_NUM; i++)
            {
                if (i < ChainNum)
                {
                    if (!Chain[i])
                    {
                        Chain[i] = Instantiate(Obj);
                    }

                    //�傫��
                    Chain[i].transform.localScale = new Vector3(LocalSize * 0.6f, LocalSize * 0.6f, LocalSize * 0.6f);

                    //////��������ړ��A��]
                    //�U��q�Ȃ�
                    if (Player.refState == EnumPlayerState.SWING)
                    {
                        //Vector3 BtoP_angle = 

                        Vector3 CenterPos = Vector3.Lerp(StartPos, EndPos, 0.5f);
                        Vector3 HousenVector = new Vector3(StartPos.y - EndPos.y, -(StartPos.x - EndPos.x), 0).normalized;//���K�������E�ւ̃x�N�g��
                        CenterPos += (HousenVector * (MAX_VEZIE_DISTANCE * ((float)(SideCnt) / MAX_SIDE_CNT)));

                        //�Ȃ���Position
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
                    //�U��q����Ȃ�
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
                    //////�����܂ňړ��A��]
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