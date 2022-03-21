using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_CannonChild : Gimmick_Main
{
    private float Speed;        // �e�̑��x
    private float LifeTime;     // �e�̐�������
    //private float ChasePower;   // �ǂ��������

    private float NowLifeTime;  // ���݂̐�������

    private GameObject PlayerObject;    // �v���C���[�I�u�W�F�N�g

    // �e���Z�b�g
    public void SetCannonChild(GameObject playerobject, float speed, float lifetime)
    {
        Speed = speed;
        LifeTime = lifetime;
        PlayerObject = playerobject;
    }

    public override void Init()
    {
        // ������

        //ChasePower = 0.1f;
        NowLifeTime = 0.0f;

        // ���݂̊p�x�����ɒe�̑��x��^����
        Vel = CalculationScript.AngleVectorXY(Rad.z) * Speed;

        // �v���C���[�I�u�W�F�N�g�擾
        //PlayerObject = GameObject.Find("Player");
    }

    public override void FixedMove()
    {
        Vector3 PlayerPos = PlayerObject.transform.position;    // �v���C���[���W
        Vector3 ThisPos = this.gameObject.transform.position;   // ���g�̍��W

        Rad.z = CalculationScript.UnityTwoPointAngle360(ThisPos, PlayerPos);    // ��]�p

        Vel = (PlayerPos - ThisPos).normalized * Speed; // �ǔ�

        if (NowLifeTime >= LifeTime) // �������ԂŎ��S
        {
            Death();
        }
        NowLifeTime += Time.fixedDeltaTime; // ���ԉ��Z
    }

    public override void Death()
    {
        // �������g������
        Destroy(this.gameObject);
    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            // �q�b�g�X�g�b�v
            GameSpeedManager.Instance.StartHitStop();

            // �v���C���[�����S��ԂɕύX
            PlayerMain.instance.mode = new PlayerStateDeath();

            // ���S
            Death();
        }

        if (collider.gameObject.tag == "Bullet")
        {
            // �q�b�g�X�g�b�v
            GameSpeedManager.Instance.StartHitStop();

            // ���S
            Death();
        }
    }
}
