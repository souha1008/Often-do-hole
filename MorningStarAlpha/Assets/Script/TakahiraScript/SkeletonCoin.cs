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
        if (collider.gameObject.CompareTag("Player"))
        {
            SoundManager.Instance.PlaySound("���艹");
            Destroy(this.gameObject);
        }
    }
}
