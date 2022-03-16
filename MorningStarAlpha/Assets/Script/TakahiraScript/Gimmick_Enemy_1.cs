using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Enemy_1 : Enemy_Interface
{
    public float MoveVelX = 10.0f;
    public float MoveVelXMax = 1400.0f;
    private float MoveVelXMaxHalf;

    // �X�^�[�g����
    public override void Init()
    {
        Vel.x = MoveVelX;
        MoveVelXMaxHalf = MoveVelXMax / 2;
    }

    // �G�̓�������
    public override void Move() 
    {
        if (TotalMoveVel.x > MoveVelXMaxHalf)
        {
            Vel.x = -MoveVelX;
        }
        else if (TotalMoveVel.x < -MoveVelXMaxHalf)
        {
            Vel.x = MoveVelX;
        }
    }

    // �G���S����
    public override void Death() 
    {
        // ���S�G�t�F�N�g

        // ���g������
        Destroy(this.gameObject);
    } 

    // �����ƏՓ˔���
    public override void OnTriggerEnter(Collider collider) 
    { 
        if (collider.gameObject.tag == "Player") // ���������I�u�W�F�N�g���d��������ɕύX�\��
        {
            // �q�b�g�X�g�b�v

            // ���������G�t�F�N�g

            Death(); // ���S����   
        }
    }
}
