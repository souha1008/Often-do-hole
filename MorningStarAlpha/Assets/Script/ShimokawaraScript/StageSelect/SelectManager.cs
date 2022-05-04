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

    int CanStage = 14;//����ׂ�X�e�[�W

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

        //������X�^�[�g���Ăт܂�
        StageSelectCamera.instance.ManualStart();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        StickX = Input.GetAxis("Horizontal");
        OldStage = NowStage;

        //�����ɍX�V����
        if(PushRightMomentOrLongPush())//�������� or ��������������
        {
            //�֐���Ă�
            PushLeftMomentOrLongPush();

            NowStage++;
        }
        else if(PushLeftMomentOrLongPush())//�������� or ��������������
        {
            NowStage--;
        }


        NowStage = Mathf.Clamp(NowStage, 0, CanStage);

        //�Ō�Ƀ`�F�b�N
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

    bool PushRightMomentOrLongPush()//�������u�Ԃƒ������ł��������ɋA���Ă���
    {
        //�����Ă�
        if(PushRight())
        {
            //�����������ĂȂ�
            if (RightPushFlag == false)
            {
                RightPushCnt++;
                RightPushFlag = true;
                return true;
            }
            //�����������Ă�
            else
            {
                RightPushCnt++;

                //���������̃^�C�~���O�Ȃ�true��Ԃ�
                if((RightPushCnt - LONG_PUSH_COOL_TIME) >= 0 && (RightPushCnt - LONG_PUSH_COOL_TIME) % LONG_PUSH_PITCH == 0)
                {
                    return true;
                }
                //��{false
                return false;
            }
            
        }
        //�����ĂȂ�
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

    bool PushLeftMomentOrLongPush()//�������u�Ԃƒ������ł��������ɋA���Ă���
    {
        //�����Ă�
        if (PushLeft())
        {
            //�����������ĂȂ�
            if (LeftPushFlag == false)
            {
                LeftPushCnt++;
                LeftPushFlag = true;
                return true;
            }
            //�����������Ă�
            else
            {
                LeftPushCnt++;

                //���������̃^�C�~���O�Ȃ�true��Ԃ�
                if ((LeftPushCnt - LONG_PUSH_COOL_TIME) >= 0 && (LeftPushCnt - LONG_PUSH_COOL_TIME) % LONG_PUSH_PITCH == 0)
                {
                    return true;
                }
                //��{false
                return false;
            }

        }
        //�����ĂȂ�
        else
        {
            LeftPushCnt = 0;
            LeftPushFlag = false;
            return false;
        }
    }

    //�X�e�[�W�N���A���ɌĂ�ŏ��Z�b�g
    public void SetStage(string scene_name)
    {
        if(scene_name == "Tutrial")
        {
            //��߂̃X�e�[�W�܂�
            NowStage = CanStage = 0;
        }
        else
        {
            int StageNum = -1;

            for (int i = 0; i < SceneName.Length; i++)
            {
                if (SceneName[i] == scene_name)
                {
                    //�����̃V�[����i�Ԗڂ��� (0���琔����)
                    StageNum = i;
                }

            }

            if (StageNum == -1)
            {
                Debug.LogError("�X�e�[�W��������");
            }

            //�����̃X�e�[�W���w���p�ɖ߂�
            NowStage = StageNum;

            //�\�X�e�[�W�𑝂₷�H
            if (StageNum + 1 > CanStage)//�X�V�K�v�Ȃ�
            {
                CanStage = StageNum + 1;
            }
        }
    }

}
