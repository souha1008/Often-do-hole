using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Thorn : Gimmick_Main
{
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
            // �v���C���[�����S��ԂɕύX
            PlayerMain.instance.mode = new PlayerStateDeath();
            // �v���C���[�Ƀ_���[�W�G�t�F�N�g
        }
    }
}
