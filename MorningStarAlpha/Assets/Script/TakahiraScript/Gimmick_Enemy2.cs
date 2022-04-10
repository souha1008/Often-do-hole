using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Enemy2 : Gimmick_Main
{
    public override void Init()
    {
        Cd.isTrigger = false;
    }

    public override void Death()
    {
        Destroy(this.gameObject);
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            // �q�b�g�X�g�b�v
            GameSpeedManager.Instance.StartHitStop();

            // ���S
            Destroy(this.gameObject);
        }
    }
}
