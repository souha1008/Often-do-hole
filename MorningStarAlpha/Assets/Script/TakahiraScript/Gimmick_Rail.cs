using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Rail : Gimmick_Main
{
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
        if (collider.gameObject.tag == "Player")
        {
            // レールステートに変更

        }
    }
}
