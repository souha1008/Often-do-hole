using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Wind : Gimmick_Main
{
    // �ϐ�
    public float WindPower = 5.0f;

    public override void Init()
    {

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

    public void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            // �v���C���[�ɕ���^����(���W����n��)

        }
    }
}
