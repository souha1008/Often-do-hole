using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Enemy2 : Gimmick_Main
{
    private static float Gravity = 0.98f;
    private bool GravityFlag = false;

    public override void Init()
    {
        Cd.isTrigger = false;
        GravityFlag = true;
    }

    public override void Death()
    {
        Destroy(this.gameObject);
    }

    public override void FixedMove()
    {
        if (GravityFlag)
        {
            Vel.y -= Gravity;
        }
        else
        {
            Vel.y = 0.0f;
        }
    }


    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            // ヒットストップ
            GameSpeedManager.Instance.StartHitStop();

            // 死亡
            Destroy(this.gameObject);
        }
        if (collision.gameObject.tag == "Platform" ||
            collision.gameObject.tag == "Box")
            GravityFlag = false;
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Platform" ||
            collision.gameObject.tag == "Box")
            GravityFlag = false;
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Platform" ||
            collision.gameObject.tag == "Box")
            GravityFlag = true;
    }
}
