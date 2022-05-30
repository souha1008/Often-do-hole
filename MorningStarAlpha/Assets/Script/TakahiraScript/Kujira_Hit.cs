using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kujira_Hit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.tag = "Thorn";
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            if (PlayerMain.instance.refState != EnumPlayerState.DEATH)
            {
                // �q�b�g�X�g�b�v
                GameSpeedManager.Instance.StartHitStop(0.1f);

                // �U��
                VibrationManager.Instance.StartVibration(1.0f, 1.0f, 0.3f);

                // ��
                //SoundManager.Instance.PlaySound("sound_21", 0.2f, 0.1f);

                // �v���C���[�X�e�[�g�𞙂̎��S�ɕύX
                PlayerMain.instance.mode = new PlayerStateDeath_Thorn(this.gameObject.transform.position);
            }
        }
    }

    public void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            if (PlayerMain.instance.refState != EnumPlayerState.DEATH)
            {
                // �q�b�g�X�g�b�v
                GameSpeedManager.Instance.StartHitStop(0.1f);

                // �U��
                VibrationManager.Instance.StartVibration(1.0f, 1.0f, 0.3f);

                // ��
                //SoundManager.Instance.PlaySound("sound_21", 1.0f, 0.2f);

                // �v���C���[�X�e�[�g�𞙂̎��S�ɕύX
                PlayerMain.instance.mode = new PlayerStateDeath_Thorn(this.gameObject.transform.position);
            }
        }
    }
}
