using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Enemy_2 : Gimmick_Main
{
    // �ϐ�


    // �X�^�[�g����
    public override void Init()
    {

    }

    // �G�̓�������
    public override void Move()
    {

    }

    // �G���S����
    public override void Death() 
    {
        // ���S�G�t�F�N�g

        // ���g������
        Destroy(this.gameObject);
    }

    // �����ƏՓˏ���(�g���K�[)
    public override void OnTriggerEnter(Collider collider) 
    {
        if (collider.gameObject.tag == "Bullet")
        {
            // �q�b�g�X�g�b�v

            // ���������G�t�F�N�g

            Death(); // ���S����   
        }
    }
}
