using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonCoin : MonoBehaviour
{
    private void Awake()
    {
        this.gameObject.GetComponent<Collider>().isTrigger = true;  // �g���K�[
    }

    public void OnTriggerEnter(Collider collider)
    {
        // �v���C���[�ƐڐG���R�C���擾
        if (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("Bullet"))
        {
            SoundManager.Instance.PlaySound("���艹");
            // �q�b�g�X�g�b�v
            GameSpeedManager.Instance.StartHitStop(0.1f);
            Destroy(this.gameObject);
        }
    }
}
