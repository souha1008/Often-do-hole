using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Thorn : Gimmick_Main
{
    public override void Init()
    {
        
    }

    public override void FixedMove()
    {
        
    }

    public override void Death()
    {
        
    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            // �v���C���[�����S��ԂɕύX
            PlayerMain.instance.mode = new PlayerState_Knockback(this.transform.position, true);
            // �v���C���[�Ƀ_���[�W�G�t�F�N�g
        }
    }
}
