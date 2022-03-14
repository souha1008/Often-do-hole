using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private  GameObject Player;         // [SerializeField] private����������inspector��Őݒ�ł���悤�ɂ���
    [SerializeField] private  float CAMERA_DISTANCE;      //�J�����ƃv���C���[�̋���

    private void Start()
    {
        TracePlayer();
    }

    private void LateUpdate()
    {
        TracePlayer();
    }

    //�v���C���[���J�����̒����Ɏ��ߑ�����
    void TracePlayer()
    {
        Vector3 tempPos = Player.GetComponent<Transform>().position;

        tempPos.z -= CAMERA_DISTANCE;
        this.GetComponent<Transform>().position = tempPos;
    }
}
