using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private  GameObject Player;         // [SerializeField] private����������inspector��Őݒ�ł���悤�ɂ���
    [SerializeField] private  float CAMERA_DISTANCE;      //�J�����ƃv���C���[�̋���
    [SerializeField] private  Direction_UI DirectionUI;      // ���UI

    private void Start()
    {
        TracePlayer();

        if (DirectionUI != null) DirectionUI.UpdateDirectionUI();
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

        if (DirectionUI != null) DirectionUI.UpdateDirectionUI();
    }

    //�v���C���[���J�����̒����Ɏ��ߑ�����
    void TracePlayer()
    {
        Vector3 tempPos = Player.transform.position;

        tempPos.z -= CAMERA_DISTANCE;
        tempPos.y += 6;
        transform.position = tempPos;
    }
}
