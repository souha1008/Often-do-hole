using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMainShimokawara: MonoBehaviour
{
    [Header("�`�F�b�N�������Ă����烌�[���Ǐ]")]
    [SerializeField, Tooltip("�`�F�b�N�������Ă����烌�[���Ǐ]")] public bool isRail;        //����Ƀ`�F�b�N�������Ă����番��

    [SerializeField] private GameObject XObj /*= GameObject.Find("CameraCenterPos")*/;         // [SerializeField] private����������inspector��Őݒ�ł���悤�ɂ���
    [SerializeField] private GameObject YObj /*= GameObject.Find("CameraCenterPos")*/;         // [SerializeField] private����������inspector��Őݒ�ł���悤�ɂ���
    [SerializeField] public float CAMERA_DISTANCE;      //�J�����ƃv���C���[�̋���

    public static CameraMainShimokawara instance; 
    
    Vector3 TruePos = Vector3.zero;//�J�������{������ׂ��ꏊ�i�U��q�̎��̂݃��[�����炻���j
    int CameraTrueCnt;//�J�������������ʒu��T������t���[���𐔂���
    int CAMERA_RETURN_FLAME = 250;//�J���������t���[�������Đ������ꏊ�ɍs����

    //�U��q��߂��u�Ԃ��E�����߂ɁA����ɕۑ����Ă�
    EnumPlayerState PlayerState;
    EnumPlayerState OldPlayerState;

    private bool stopFlag; //�J������~�p

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        stopFlag = false;
        CameraTrueCnt = CAMERA_RETURN_FLAME;
        TraceObj();

        PlayerState = OldPlayerState = PlayerMain.instance.refState;
    }

    public void ManualUpdate()
    {
        if (stopFlag == false)
        {
            TraceObj();
        }
    }

    public void CameraReturn()
    {
        if (isRail)
        {
            if (VirtualCamera.instance. isReturn)
            {
                Vector3 Temp = TempCamera.instance.transform.position;
                Temp.z -= CAMERA_DISTANCE;

                transform.position = Temp;
            }
        }
    }

    //private void FixedUpdate()
    //{
    //    TracePlayer();
    //}

    //private void LateUpdate()
    //{
    //    TraceObj();
    //}

    //�v���C���[���J�����̒����Ɏ��ߑ�����
    void TraceObj()
    {
        OldPlayerState = PlayerState;
        PlayerState = PlayerMain.instance.refState;

        if(isRail)
        {
            //�U��q����Ȃ��Ȃ����^�C�~���O�ŁACameraTrueCnt��0�ɂ���
            if(OldPlayerState == EnumPlayerState.SWING && PlayerState != EnumPlayerState.SWING)
            {
                CameraTrueCnt = 0;
            }


            TruePos.x = XObj.transform.position.x;
            TruePos.y = YObj.transform.position.y;
            TruePos.z = -CAMERA_DISTANCE;


            //���ܐU��q�Ȃ�
            if(PlayerMain.instance.refState == EnumPlayerState.SWING)
            {
                this.transform.position = new Vector3(TruePos.x, transform.position.y, transform.position.z);

            }
            //�U��q����Ȃ���
            else
            {                
                //��Fixed�ɂ���Ă��炤
                //CameraTrueCnt++;

                if(CameraTrueCnt <= CAMERA_RETURN_FLAME)
                {
                    //���̃J�E���g���C�[�W���O���āA�����ƍďI����x�߂ɂ���
                    float EasingCnt;
                    if (CameraTrueCnt < CAMERA_RETURN_FLAME / 2)
                    {
                        EasingCnt = CameraTrueCnt * Easing.EasingTypeFloat(EASING_TYPE.QUAD_IN, CameraTrueCnt, (CAMERA_RETURN_FLAME / 2 - 1), 0, 1);
                    }
                    else if(CameraTrueCnt > CAMERA_RETURN_FLAME / 2)
                    {
                        EasingCnt = CameraTrueCnt * Easing.EasingTypeFloat(EASING_TYPE.QUAD_IN, CAMERA_RETURN_FLAME - CameraTrueCnt, (CAMERA_RETURN_FLAME / 2 - 1), 0, 1);

                    }
                    else
                    {
                        EasingCnt = CameraTrueCnt;
                    }

                    //20�t���[��������Y�����̏ꏊ�ɖ߂�
                    float Y = Mathf.Lerp(this.transform.position.y, TruePos.y, (float)EasingCnt / CAMERA_RETURN_FLAME);
                    this.transform.position = new Vector3(TruePos.x, Y, TruePos.z);
                }
                else
                {
                    this.transform.position = TruePos;
                }

                
            }
            
        }
        else
        {
            TruePos = XObj.transform.position;

            TruePos.z -= CAMERA_DISTANCE;
            this.transform.position = TruePos;
        }

    }
    
    public void StopCamera()
    {
        stopFlag = true;
    }

    void FixedUpdate()
    {
        CameraTrueCnt = Mathf.Min(CameraTrueCnt + 1, 500);


        //if (Input.GetKey(KeyCode.UpArrow))
        //{
        //    CameraShake.instance.Shake();
        //}

    }

    //����ׂ��ꏊ�Ɋ񂹂�֐�
    void Bring()
    {
        //���̃J�E���g���C�[�W���O���āA�����ƍďI����x�߂ɂ���
        float EasingCnt;
        //�񂹏I����Ă�
        if (CameraTrueCnt > CAMERA_RETURN_FLAME)
        {
            EasingCnt = CAMERA_RETURN_FLAME;
        }
        //�񂹒�
        else
        {
            if (CameraTrueCnt < CAMERA_RETURN_FLAME / 2)
            {
                EasingCnt = CameraTrueCnt * Easing.EasingTypeFloat(EASING_TYPE.QUAD_IN, CameraTrueCnt, (CAMERA_RETURN_FLAME / 2 - 1), 0, 1);
            }
            else if (CameraTrueCnt > CAMERA_RETURN_FLAME / 2)
            {
                EasingCnt = CameraTrueCnt * Easing.EasingTypeFloat(EASING_TYPE.QUAD_IN, CAMERA_RETURN_FLAME - CameraTrueCnt, (CAMERA_RETURN_FLAME / 2 - 1), 0, 1);
            }
            else
            {
                EasingCnt = CameraTrueCnt;
            }
        }

        //20�t���[�������Č��̏ꏊ�ɖ߂�
        this.transform.position = Vector3.Lerp(this.transform.position, TruePos, (float)EasingCnt / CAMERA_RETURN_FLAME);
    }
}


