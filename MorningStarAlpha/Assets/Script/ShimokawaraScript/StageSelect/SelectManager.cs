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

    int CanStage = 0;//����ׂ�X�e�[�W
    bool CanStart = true;//�ړ����͊J�n�ł��Ȃ�


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

        //������X�^�[�g���Ăт܂�
        StageSelectCamera.instance.ManualStart();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        StickX = Input.GetAxis("Horizontal");
        OldSelectStage = NowSelectStage;

        //�����ɍX�V����
        if(PushRightMomentOrLongPush())//�������� or ��������������
        {
            //�֐���Ă�
            PushLeftMomentOrLongPush();

            NowSelectStage++;
        }
        else if(PushLeftMomentOrLongPush())//�������� or ��������������
        {
            NowSelectStage--;
        }


        NowSelectStage = Mathf.Clamp(NowSelectStage, 0, CanStage);

        //�Ō�Ƀ`�F�b�N
        ChangeStageCheck();

        //�X�e�[�W�N��
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
            //�ς�����^�C�~���O�̓X�^�[�g�󂯕t���Ȃ�
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
            int OldCanStage = -1;//���J���ς݃X�e�[�W���i�[  
            if (SaveDataManager.Instance)
            {
                for (int i = 0; i < SaveDataManager.Instance.MainData.Stage.Length; i++)
                {
                    //i�Ԗڂ��N���A�ς݂Ȃ�
                    if (SaveDataManager.Instance.MainData.Stage[i].Clear)
                        OldCanStage = i;
                    //�N���A���ĂȂ��Y�����܂ŗ���
                    else
                        break;
                }
            }

            //�N���A�����X�e�[�W + 1 �� �V�ׂ�X�e�[�W   ��1-1���N���A�Ȃ� (-1 + 1) = 0 �Ԗڂ܂ł��V�ׂ�X�e�[�W
            CanStage = OldCanStage + 1;
        }
       

    }

    //�X�e�[�W�N���A���ɌĂ�ŏ��Z�b�g
#if false
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
#endif

}
