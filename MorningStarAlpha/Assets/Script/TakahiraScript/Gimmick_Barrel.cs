using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Barrel : Gimmick_Main
{
    [Label("���˂܂ł̑ҋ@����")]
    public float WaitTime = 1.5f;
    [Label("���˂̈З�")]
    public float SpringPower = 50.0f;

    private bool StartFlag; // �N���t���O
    private float NowTime;  // �o�ߎ���
    //private Vector3 StartPlayerPos;     // �v���C���[�̏������W
    private GameObject PlayerObject;    // �v���C���[�I�u�W�F�N�g
    private Vector3 SpringVec;          // ��ԃx�N�g����

    public override void Init()
    {
        // ������
        NowTime = 0.0f;
        StartFlag = false;
        PlayerObject = GameObject.Find("Player");

        SpringVec = JumpPower();
    }

    public override void FixedMove()
    {
        if (StartFlag)
        {
            if (NowTime >= WaitTime) // ���ˎ��Ԍo�߂���
            {
                Jump(); // ����
                NowTime = 0.0f; // ���ԃ��Z�b�g
                StartFlag = false; // �N���t���O�I�t
            }
            else
            {
                PlayerPosChange(); // �v���C���[�̈ʒu�ړ�
                NowTime += Time.fixedDeltaTime; // ���ԉ��Z
            }
        }
    }

    private void Jump()
    {
        if (PlayerObject != null)
        {
            //SpringVec = JumpPower();
        }   
    }

    private Vector3 JumpPower()
    {
        float ThisRad;           // ��]�p
        Vector3 VecPower = Vector3.zero;    // ������x�N�g����

        ThisRad = this.transform.localEulerAngles.z;  // �M�̉�]�p(�x)
        ThisRad = CalculationScript.AngleCalculation(ThisRad); // �p�x���W�A���ϊ�
        VecPower = CalculationScript.AngleVectorXY(ThisRad) * SpringPower;  // ��ԃx�N�g����

        if (VecPower.x < 1 && VecPower.x > -1) VecPower.x = 0;  // �������l�͌덷�Ƃ���0�ɂ���
        if (VecPower.y < 1 && VecPower.y > -1) VecPower.y = 0;

        return VecPower;
    }

    private void PlayerPosChange()
    {
        if (PlayerObject != null)
        {
            //if (NowTime <= WaitTime / 5.0f) // 1/5�̎��Ԃ������S�Ɍ������Ĉړ�
            //{
            //     PlayerObject.transform.position =
            //        new Vector3(Easing.QuadInOut(NowTime, WaitTime / 5.0f, StartPlayerPos.x, gameObject.transform.position.x), 
            //        Easing.QuadInOut(NowTime, WaitTime / 5.0f, StartPlayerPos.y, gameObject.transform.position.y), gameObject.transform.position.z);
            //}
            //else // ����ȊO�͒��S�łƂǂ܂�
            //{
            //    PlayerObject.transform.position = this.gameObject.transform.position;
            //}
        }
    }

    public override void Death()
    {

    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && !StartFlag)
        {
            PlayerObject = collider.gameObject;
            //StartPlayerPos = PlayerObject.transform.position;
            //PlayerObject.GetComponent<PlayerMain>().forciblyReturnBulletFlag = true;
            PlayerObject.GetComponent<PlayerMain>().mode = new PlayerState_Barrel(WaitTime, SpringVec, gameObject.transform.position, true);
            StartFlag = true;
        }
        if (collider.gameObject.tag == "Bullet" && !StartFlag)
        {
            PlayerObject = GameObject.Find("Player");
            //StartPlayerPos = PlayerObject.transform.position;
            //PlayerObject.GetComponent<PlayerMain>().forciblyReturnBulletFlag = true;
            PlayerObject.GetComponent<PlayerMain>().mode = new PlayerState_Barrel(WaitTime, SpringVec, gameObject.transform.position, false);
            StartFlag = true;
        }
    }
}
