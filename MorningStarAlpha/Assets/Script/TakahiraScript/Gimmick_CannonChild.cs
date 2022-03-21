using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_CannonChild : Gimmick_Main
{
    private float Speed;    // ’e‚Ì‘¬“x
    private float LifeTime; // ’e‚Ì¶‘¶ŠÔ

    public override void Init()
    {
        
    }
    public override void Death()
    {
        // ©•ª©g‚ğÁ‚·
        Destroy(this.gameObject);
    }
}
