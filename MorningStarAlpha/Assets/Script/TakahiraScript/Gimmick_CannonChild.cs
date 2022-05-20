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


    // �e���Z�b�g
    public void SetCannonChild(float speed, float lifetime, bool chaseflag, Vector3 pos, Quaternion quaternion)
    {
        // �l�Z�b�g
        NowLifeTime = 0.0f;
        Speed = speed;
        LifeTime = lifetime;
        ChaseFlag = chaseflag;
        transform.SetPositionAndRotation(pos, quaternion);
    }

    public override void Init()
    {
        // ������

        //ChasePower = 0.1f;
        //NowLifeTime = 0.0f;

        // �R���C�_�[
        //this.GetComponent<Collider>().isTrigger = false; // �g���K�[�I�t

        // �e�ɑ��x��^����
        //Vel = CalculationScript.AngleVectorXY(CalculationScript.AngleCalculation(transform.rotation.eulerAngles.z)) * Speed;
        //Vel = CalculationScript.AngleVectorXY(CalculationScript.AngleCalculation(Rad.z)).normalized * Speed; // (�m�[�}���C�Y)
    }

    public override void UpdateMove()
    {
        // �ǔ�����
        if (ChaseFlag)
        {
            Vector3 PlayerPos = PlayerMain.instance.transform.position;    // �v���C���[���W
            Vector3 ThisPos = this.gameObject.transform.position;   // ���g�̍��W

            transform.rotation = Quaternion.Euler(0, 0, CalculationScript.UnityTwoPointAngle360(ThisPos, PlayerPos));    // ��]�p
        }

        // �e�ɑ��x��^����
        Vel = CalculationScript.AngleVectorXY(CalculationScript.AngleCalculation(transform.rotation.eulerAngles.z)) * Speed;
        //Vel = (PlayerPos - ThisPos).normalized * Speed; // �ǔ�(�m�[�}���C�Y)

        if (NowLifeTime >= LifeTime) // �������ԂŎ��S
        {
            Death();
        }
        NowLifeTime += Time.deltaTime; // ���ԉ��Z
    }

    public override void Death()
    {
        Debug.LogWarning("���S");
        // �������g������
        //Destroy(this.gameObject);
        this.gameObject.SetActive(false);
    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Bullet") ||
            (collider.gameObject.CompareTag("Player") && PlayerMain.instance.refState == EnumPlayerState.SWING))
        {
            // �q�b�g�X�g�b�v
            GameSpeedManager.Instance.StartHitStop(0.1f);

            // ���S
            Death();
        }
        else if (collider.gameObject.CompareTag("Player"))
        {
            // �q�b�g�X�g�b�v
            //GameSpeedManager.Instance.StartHitStop(0.1f);

            // �v���C���[�����S��ԂɕύX
            PlayerMain.instance.mode = new PlayerState_Knockback(this.transform.position, false);

            // ���S
            Death();
        }

        // ���S
        Death();
    }
}
