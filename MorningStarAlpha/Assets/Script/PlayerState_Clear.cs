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
    private float motionTimer;
    
    public PlayerState_Clear()
    {
        PlayerScript.refState = EnumPlayerState.CLEAR;
        PlayerScript.canShotState = false;
        PlayerScript.rb.velocity = Vector3.zero;
        PlayerScript.vel.x = 0.0f;
        motionTimer = 0.0f;

        goal = GoalManager.Instance.gameObject;
        BulletScript.ReturnBullet();
        PlayerScript.dir = PlayerMoveDir.RIGHT; //�����E����
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
            }
        }
        else if(state == ClearState.WALK)
        {
            //�E�ɕ���
            PlayerScript.vel.x = 10.0f;
            PlayerScript.animator.SetBool("isRunning", true);
            Debug.Log(Vector3.Distance(PlayerScript.rb.position, goal.transform.position));

            //����������
            if (Vector3.Distance(PlayerScript.rb.position, goal.transform.position) < AnimStartDistance)
            {
                state = ClearState.ANIMMOTION;            
                PlayerScript.vel.x = 0.0f;

                PlayerScript.animator.SetBool("isRunning", false);
                PlayerScript.animator.SetTrigger("ClearTrigger");

                goal.GetComponent<Animator>().SetTrigger("OpenTrigger");
            }
        }
        else if (state == ClearState.ANIMMOTION)
        {
            motionTimer += Time.fixedDeltaTime;

            if(motionTimer > 10.0f)
            {
                GoalManager.Instance.StartMotionBlur();
            }
        }

    }
}
