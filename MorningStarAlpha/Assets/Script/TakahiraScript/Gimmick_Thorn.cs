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
            // �q�b�g�X�g�b�v
            GameSpeedManager.Instance.StartHitStop(0.1f);
            // �v���C���[�����S��ԂɕύX
            if (PlayerMain.instance.refState != EnumPlayerState.DEATH)
            {
                PlayerMain.instance.mode = new PlayerStateDeath_Thorn();
            }
            // �v���C���[�Ƀ_���[�W�G�t�F�N�g
        }
    }
}
