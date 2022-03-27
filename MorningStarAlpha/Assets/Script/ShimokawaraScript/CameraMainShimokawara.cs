using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMainShimokawara: MonoBehaviour
{
    [Header("�`�F�b�N�������Ă����烌�[���Ǐ]")]
    [SerializeField, Tooltip("�`�F�b�N�������Ă����烌�[���Ǐ]")] public bool isRail;        //����Ƀ`�F�b�N�������Ă����番��

    [SerializeField] private GameObject CenterObj /*= GameObject.Find("CameraCenterPos")*/;         // [SerializeField] private����������inspector��Őݒ�ł���悤�ɂ���
    [SerializeField] public float CAMERA_DISTANCE;      //�J�����ƃv���C���[�̋���

    public static CameraMainShimokawara instance; 

    private void Start()
    {
        instance = this;
        TracePlayer();
    }

    //private void Update()
    //{
    //    TracePlayer();
    //}

    //private void FixedUpdate()
    //{
    //    TracePlayer();
    //}


    private void LateUpdate()
    {
        if(!isRail)
        {
            TracePlayer();
        }
    }

    //�v���C���[���J�����̒����Ɏ��ߑ�����
    void TracePlayer()
    {
        Vector3 tempPos = CenterObj.transform.position;

        tempPos.z -= CAMERA_DISTANCE;
        transform.position = tempPos;
    }
}
