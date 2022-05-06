using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateStan : PlayerState
{
    private bool shotButton;
    private const float SLIDE_END_TIME = 0.3f;
    private float slideEndTimer;

    float StartTime;
    static public float TIME_LENGTH = 0.5f;

    public PlayerStateStan()//�R���X�g���N�^
    {
        PlayerScript.refState = EnumPlayerState.STAN;
        shotButton = false;
        PlayerScript.vel = Vector3.zero;
        PlayerScript.canShotState = false;
        slideEndTimer = 0.0f;
        StartTime = Time.time;

        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            PlayerScript.rb.MoveRotation(Quaternion.Euler(0, 140, 0));
        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            PlayerScript.rb.MoveRotation(Quaternion.Euler(0, -140, 0));
        }


        //�{�[���֘A
        BulletScript.ReturnBullet();
        //�A�j���p
        PlayerScript.animator.SetTrigger("StanTrigger");
    }

    public override void UpdateState()
    {

    }

    public override void Move()
    {

    }


    public override void StateTransition()
    {
        if(Time.time > StartTime + TIME_LENGTH)
        {
            RotationStand();


            if (PlayerScript.isOnGround == true)
            {
                PlayerScript.mode = new PlayerStateOnGround();
            }
            else
            {
                PlayerScript.mode = new PlayerStateMidair(true, MidairState.NORMAL);
            }
        }
    }
}
