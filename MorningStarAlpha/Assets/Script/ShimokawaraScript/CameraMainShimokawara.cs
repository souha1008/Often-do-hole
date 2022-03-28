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

    private void Start()
    {
        instance = this;
        CameraTrueCnt = CAMERA_RETURN_FLAME;
        TraceObj();
    }

    public void ManualUpdate()
    {
        TraceObj();
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
        if(isRail)
        {


            TruePos.x = XObj.transform.position.x;
            TruePos.y = YObj.transform.position.y;
            TruePos.z = -CAMERA_DISTANCE;


            //���ܐU��q�Ȃ�
            if(PlayerMain.instance.refState == EnumPlayerState.SWING)
            {
                this.transform.position = new Vector3(TruePos.x, transform.position.y, transform.position.z);

                CameraTrueCnt = 0;
            }
            //�U��q����Ȃ���
            else
            {
                //Fixed�ɂ���Ă��炤
                //CameraTrueCnt++;

                if(CameraTrueCnt <= CAMERA_RETURN_FLAME)
                {
                    //20�t���[�������Č��̏ꏊ�ɖ߂�
                    this.transform.position = Vector3.Lerp(this.transform.position, TruePos,(float)CameraTrueCnt / CAMERA_RETURN_FLAME);
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

    void FixedUpdate()
    {
        CameraTrueCnt = Mathf.Min(CameraTrueCnt + 1, 500);
    }
}


