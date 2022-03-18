using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_FallBlock : Gimmick_Main
{
    public float FallPower = 0.05f;     // �������
    private bool NowFall;               // ��������

    public override void Init()
    {
        // ������
        NowFall = false;

        // �R���W����
        this.gameObject.GetComponent<Collider>().isTrigger = false;  // �g���K�[�I�t

        // ���W�b�h�{�f�B
        Rb.mass = 100000.0f; // �d�����ē����Ȃ��悤�ɂ���
    }

    public override void Move()
    {
        if (NowFall)
        {
            Vel.y += -FallPower;
        }
        if (TotalMoveVel.y <= -500.0f)
        {
            Death();
        }
    }

    public override void Death()
    {
        // �������g������
        Destroy(this.gameObject);
    }

    public override void OnTriggerEnter(Collider collider)
    {
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        // �v���C���[���d�ƐڐG
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Bullet")
        {
            NowFall = true; // ������
        }
    }
}
