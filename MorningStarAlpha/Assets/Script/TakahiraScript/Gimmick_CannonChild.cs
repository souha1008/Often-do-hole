using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_CannonChild : Gimmick_Main
{
    private float Speed;        // �e�̑��x
    private float LifeTime;     // �e�̐�������
    private bool ChaseFlag;     // �e���ǔ����邩
    //private float ChasePower;   // �ǂ��������

    private float NowLifeTime;  // ���݂̐�������
    private GameObject PlayerObject;    // �v���C���[�I�u�W�F�N�g

    // �e���Z�b�g
    public void SetCannonChild(GameObject playerobject, float speed, float lifetime, bool chaseflag)
    {
        Speed = speed;
        LifeTime = lifetime;
        PlayerObject = playerobject;
        ChaseFlag = chaseflag;
    }

    public override void Init()
    {
        // ������

        //ChasePower = 0.1f;
        NowLifeTime = 0.0f;

        // �R���C�_�[
        //this.GetComponent<Collider>().isTrigger = false; // �g���K�[�I�t

        // �e�ɑ��x��^����
        Vel = CalculationScript.AngleVectorXY(CalculationScript.AngleCalculation(transform.rotation.eulerAngles.z)) * Speed;
        //Vel = CalculationScript.AngleVectorXY(CalculationScript.AngleCalculation(Rad.z)).normalized * Speed; // (�m�[�}���C�Y)

        // �v���C���[�I�u�W�F�N�g�擾
        //PlayerObject = GameObject.Find("Player");
    }

    public override void UpdateMove()
    {
        // �ǔ�����
        if (ChaseFlag)
        {
            Vector3 PlayerPos = PlayerObject.transform.position;    // �v���C���[���W
            Vector3 ThisPos = this.gameObject.transform.position;   // ���g�̍��W

            transform.rotation = Quaternion.Euler(0, 0, CalculationScript.UnityTwoPointAngle360(ThisPos, PlayerPos));    // ��]�p

            Vel = CalculationScript.AngleVectorXY(CalculationScript.AngleCalculation(transform.rotation.eulerAngles.z)) * Speed; // �ǔ�
            //Vel = (PlayerPos - ThisPos).normalized * Speed; // �ǔ�(�m�[�}���C�Y)
        }

        if (NowLifeTime >= LifeTime) // �������ԂŎ��S
        {
            Death();
        }
        NowLifeTime += Time.deltaTime; // ���ԉ��Z
    }

    public override void Death()
    {
        // �������g������
        Destroy(this.gameObject);
    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            // �q�b�g�X�g�b�v
            //GameSpeedManager.Instance.StartHitStop(0.1f);

            // �v���C���[�����S��ԂɕύX
            PlayerMain.instance.mode = new PlayerStateDeath();

            // ���S
            Death();
        }

        if (collider.gameObject.CompareTag("Bullet"))
        {
            // �q�b�g�X�g�b�v
            GameSpeedManager.Instance.StartHitStop(0.1f);

            // ���S
            Death();
        }
    }
}
