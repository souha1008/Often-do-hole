using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_CannonChild : Gimmick_Main
{
    private float Speed;    // �e�̑��x
    private float LifeTime; // �e�̐�������

    public override void Init()
    {
        
    }
    public override void Death()
    {
        // �������g������
        Destroy(this.gameObject);
    }
}
