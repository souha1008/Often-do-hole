using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_MoveBlock : Gimmick_Main
{
    // �ϐ�
    public float MoveVelMax = 5.0f;     // �ő�ړ���
    public float MoveTime = 3.0f;       // �ړ�����
    private float NowTime;       // ���݂̈ړ�����

    public override void Init()
    {
        // ������
        NowTime = 0.0f;

        // �R���W����
        this.gameObject.GetComponent<Collider>().isTrigger = false;  // �g���K�[�I�t

        // ���W�b�h�{�f�B
        Rb.mass = 100000.0f; // �d�����ē����Ȃ��悤�ɂ���
    }

    public override void Move()
    {

    }

    public override void Death()
    {

    }

    public override void OnTriggerEnter(Collider collider)
    {
        
    }

    //public void OnCollisionEnter(Collision collision)
    //{

    //}
}
