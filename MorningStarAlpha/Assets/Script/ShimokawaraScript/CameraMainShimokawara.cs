using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMainShimokawara: MonoBehaviour
{
    [SerializeField] private GameObject CenterObj /*= GameObject.Find("CameraCenterPos")*/;         // [SerializeField] private����������inspector��Őݒ�ł���悤�ɂ���
    [SerializeField] private float CAMERA_DISTANCE;      //�J�����ƃv���C���[�̋���

    private void Start()
    {
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
        TracePlayer();
    }

    //�v���C���[���J�����̒����Ɏ��ߑ�����
    void TracePlayer()
    {
        Vector3 tempPos = CenterObj.transform.position;

        tempPos.z -= CAMERA_DISTANCE;
        transform.position = tempPos;
    }
}
