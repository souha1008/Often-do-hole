using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_FallBlock : Gimmick_Main
{
    private bool NowFall = false;    // 落下中か

    public override void Init()
    {

    }

    public override void Move()
    {

    }

    public override void Death()
    {

    }

    public override void OnTriggerEnter(Collider collider)
    {
        // プレイヤーか錨と接触
        if (collider.gameObject.tag == "Player" && collider.gameObject.tag == "Bullet")
        {
            NowFall = true;
        }
    }
}
