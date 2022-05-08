using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Hole : Gimmick_Main
{
    public override void Init()
    {
        // ���b�V���������Ȃ�����
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    public override void FixedMove()
    {

    }

    public override void Death()
    {

    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (PlayerMain.instance.refState != EnumPlayerState.DEATH)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                // �v���C���[�����S��ԂɕύX
                PlayerMain.instance.mode = new PlayerStateDeath_Void();
                // �v���C���[�Ƀ_���[�W�G�t�F�N�g
            }
        }
    }
}
