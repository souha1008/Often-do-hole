using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_SpringBoard : Gimmick_Main
{
    // �ϐ�
    public float SpringPower = 100.0f;   // �W�����v��̗�

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
        if (collider.gameObject.tag == "Player")
        {
            float Rad;           // ��]�p
            Vector3 VecPower = Vector3.zero;    // ������x�N�g����

            PlayerMain playermain = collider.gameObject.GetComponent<PlayerMain>(); // �v���C���[���C���X�N���v�g�擾
            Rad = this.transform.localEulerAngles.z;  // �W�����v��̉�]�p(�x)
            Rad = CalculationScript.AngleCalculation(Rad); // �p�x���W�A���ϊ�
            VecPower = CalculationScript.AngleVectorXY(Rad) * SpringPower;  // ��ԃx�N�g����

            if (VecPower.x < 1 && VecPower.x > -1) VecPower.x = 0;  // �������l�͌덷�Ƃ���0�ɂ���
            if (VecPower.y < 1 && VecPower.y > -1) VecPower.y = 0;

            //Debug.Log(VecPower.x);
            //Debug.Log(VecPower.y);

            playermain.vel = VecPower; // �v���C���[�̃x�N�g���ʕύX
        }
    }
}
