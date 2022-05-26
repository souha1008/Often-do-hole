using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Thorn : Gimmick_Main
{
    public override void Init()
    {
        Rb.isKinematic = true;
        Cd.isTrigger = false;
        this.gameObject.tag = "Thorn";
    }

    public override void FixedMove()
    {
        
    }

    public override void Death()
    {
        
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {   
            if (PlayerMain.instance.refState != EnumPlayerState.DEATH)
            {
                // �q�b�g�X�g�b�v
                GameSpeedManager.Instance.StartHitStop(0.1f);

                // �U��
                VibrationManager.Instance.StartVibration(1.0f, 1.0f, 0.22f);

                // ��
                SoundManager.Instance.PlaySound("sound_21", 0.2f, 0.1f);

                // �v���C���[�X�e�[�g�����S�ɕύX
                PlayerMain.instance.mode = new PlayerStateDeath_Thorn(gameObject.transform.position);
            }
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        if (PlayerMain.instance.refState != EnumPlayerState.DEATH)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                // �q�b�g�X�g�b�v
                GameSpeedManager.Instance.StartHitStop(0.1f);

                // �U��
                VibrationManager.Instance.StartVibration(1.0f, 1.0f, 0.22f);

                // ��
                SoundManager.Instance.PlaySound("sound_21", 1.0f, 0.2f);

                // �v���C���[�X�e�[�g�����S�ɕύX
                PlayerMain.instance.mode = new PlayerStateDeath_Thorn(gameObject.transform.position);
            }
        }
    }
}
