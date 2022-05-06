using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerState_Clear : PlayerState
{
    private enum ClearState
    {
        MIDAIR,
        WALK,
        ANIMMOTION,
    }

    const float AnimStartDistance = 5.0f;

    ClearState state;
    GameObject goal;
    bool goRight;

    public PlayerState_Clear()
    {
        PlayerScript.refState = EnumPlayerState.CLEAR;
        PlayerScript.canShotState = false;
        PlayerScript.rb.velocity = Vector3.zero;
        PlayerScript.vel.x = 0.0f;



        goal = GameObject.Find("Gole");
        BulletScript.ReturnBullet();
        PlayerScript.dir = PlayerMoveDir.RIGHT; //ã≠êßâEå¸Ç´
        RotationStand();

        PlayerScript.ResetAnimation();
        if (PlayerScript.isOnGround)
        {
            state = ClearState.WALK;
        }
        else
        {
            state = ClearState.MIDAIR;
        }
    }

    public override void UpdateState()
    {
        
    }

    public override void Move()
    {
        if (state == ClearState.MIDAIR)
        {
            PlayerScript.vel += Vector3.down * PlayerScript.FALL_GRAVITY * (fixedAdjust);
            PlayerScript.vel.y = Mathf.Max(PlayerScript.vel.y, PlayerScript.MAX_FALL_SPEED * -1);


            if (PlayerScript.isOnGround)
            {
                state = ClearState.WALK;

                PlayerScript.vel.y = 0.0f;
                ////GoRightÇæÇØÇ≈ÇÊÇ≠ÇÀ
                //if (PlayerScript.rb.position.x - goal.transform.position.x < 0.0f)
                //{
                //    goRight = true;
                //}
                //else
                //{
                //    goRight = false;
                //}
            }
        }
        else if(state == ClearState.WALK)
        {
            //âEÇ…ï‡Ç≠
            PlayerScript.vel.x = 10.0f;
            PlayerScript.animator.SetBool("isRunning", true);

            //ìûíÖÇµÇΩÇÁ
            if (Vector3.Distance(PlayerScript.rb.position, goal.transform.position) < AnimStartDistance)
            {
                state = ClearState.ANIMMOTION;
                PlayerScript.animator.SetBool("isRunning", false);
                PlayerScript.animator.SetTrigger("ClearTrigger");
                PlayerScript.vel.x = 0.0f;
            }
        }
        else if (state == ClearState.ANIMMOTION)
        {

        }

    }
}
