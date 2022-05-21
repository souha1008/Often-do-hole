using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonCoin : MonoBehaviour
{
    private Animator CoinAnimator;
    private bool OnceFlag = false;

    private void Awake()
    {
        this.gameObject.GetComponent<Collider>().isTrigger = true;  // �g���K�[
        CoinAnimator = this.gameObject.GetComponent<Animator>();
    }

    public void OnTriggerEnter(Collider collider)
    {
        // �v���C���[�ƐڐG���R�C���擾
        if (!OnceFlag && (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("Bullet")))
        {
            OnceFlag = true;
            SoundManager.Instance.PlaySound("���艹");
            // �q�b�g�X�g�b�v
            GameSpeedManager.Instance.StartHitStop(0.1f);

            // �U��
            VibrationManager.Instance.StartVibration(0.8f, 0.8f, 0.12f);

            CoinAnimator.SetBool("GetCoin", true);
        }
    }

    public void Death()
    {
        // �R�C���擾�G�t�F�N�g
        EffectManager.Instance.CoinGetEffect(transform.position);
        this.gameObject.SetActive(false);
    }
}
