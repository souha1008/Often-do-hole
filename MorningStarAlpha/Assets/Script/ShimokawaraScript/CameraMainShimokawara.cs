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

    private void Start()
    {
        instance = this;
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
            Vector3 tempPos = Vector3.zero;

            tempPos.x = XObj.transform.position.x;
            tempPos.y = YObj.transform.position.y;
            tempPos.z -= CAMERA_DISTANCE;

            transform.position = tempPos;
        }
        else
        {
            Vector3 tempPos = XObj.transform.position;

            tempPos.z -= CAMERA_DISTANCE;
            transform.position = tempPos;
        }
    }
}
