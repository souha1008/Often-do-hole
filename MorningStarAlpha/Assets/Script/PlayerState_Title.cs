using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class PlayerState_Title : PlayerState
{

    public PlayerState_Title()
    {
        
    }

    public override void UpdateState()
    {
        
    }

    public override void Move()
    {
        // �����Ȃ�
    }

    public override void StateTransition()
    {
        if (Title_p2.instance.changeScene == true)
        {
            PlayerScript.mode = new PlayerStateOnGround();
            
        }
    }

    
}
