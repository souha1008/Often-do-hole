using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_CannonChild : Gimmick_Main
{
    private float Speed;    // 弾の速度
    private float LifeTime; // 弾の生存時間

    public override void Init()
    {
        
    }
    public override void Death()
    {
        // 自分自身を消す
        Destroy(this.gameObject);
    }
}
