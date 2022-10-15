using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_CannonChild_Bullet : Gimmick_CannonChild
{
    public override void OnTriggerEnter(Collider collider)
    {
        //Debug.LogWarning(collider.gameObject.name);
        // �e����������
        if (collider.gameObject.CompareTag("Bullet") ||
            (collider.gameObject.CompareTag("Player") && PlayerMain.instance.refState == EnumPlayerState.SWING))
        {
            // �q�b�g�X�g�b�v
            GameSpeedManager.Instance.StartHitStop(0.1f);

            // �U��
            VibrationManager.Instance.StartVibration(0.4f, 0.4f, 0.2f);

            // ���g�����S
            Death();
        }

        // �m�b�N�o�b�N
        else if (collider.gameObject.CompareTag("Player"))
        {
            // �q�b�g�X�g�b�v
            //GameSpeedManager.Instance.StartHitStop(0.1f);

            // �G�t�F�N�g
            EffectManager.Instance.SharkExplosionEffect(this.transform.position);

            // �U��
            VibrationManager.Instance.StartVibration(0.7f, 0.7f, 0.2f);

            // �v���C���[���m�b�N�o�b�N��ԂɕύX
            PlayerMain.instance.mode = new PlayerState_Knockback(this.transform.position, false);

            // ���g�����S
            Death();
        }

        // ���S
        Death();
    }
}
