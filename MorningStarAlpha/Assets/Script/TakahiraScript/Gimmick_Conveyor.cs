using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Conveyor : Gimmick_Main
{
    private bool StartFlag;

    public override void Init()
    {
        // ������


        // �R���W����
        //Cd.isTrigger = false; // �g���K�[�I�t

        // �p�x��0�x�Œ�
        Rad = Vector3.zero;
    }

    public override void FixedMove()
    {
        
    }


    public override void Death()
    {
        
    }

    public override void OnCollisionEnter(Collision collision)
    {
        
    }
}
