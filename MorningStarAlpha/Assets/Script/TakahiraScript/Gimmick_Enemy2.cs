using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Enemy2 : Gimmick_Main
{
    private static float Gravity = 0.98f;
    //private bool GravityFlag = false;

    public override void Init()
    {
        Cd.isTrigger = false;
        Rb.constraints |= RigidbodyConstraints.FreezePositionZ;   // ���WZ�̃t���[�Y���I��
        //GravityFlag = true;
    }

    public override void Death()
    {
        Destroy(this.gameObject);
    }

    public override void FixedMove()
    {
        Vel.y -= Gravity;
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
        if (collision.gameObject.tag == "Platform")
        {
            Vel.y = 0.0f;
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            Vel.y = 0.0f;
        }

        if (collision.gameObject.tag == "Box")
        {
            // �Փ˓_�擾
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.normal.y < -0.1f || contact.normal.y > 0.1f)
                {
                    Vel.y = 0.0f;
                }
            }
        }
    }

    //public void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Platform" ||
    //        collision.gameObject.tag == "Box")
    //        GravityFlag = true;
    //}
}
