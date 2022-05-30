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
        GameStateManager.SetGameState(GAME_STATE.RESULT);

        CameraMainShimokawara.instance.GoalZoom(25);

        PlayerScript.refState = EnumPlayerState.CLEAR;
        PlayerScript.canShotState = false;
        PlayerScript.rb.velocity = Vector3.zero;
        PlayerScript.vel.x = 0.0f;
        motionTimer = 0.0f;

        goal = GoalManager.Instance.gameObject;
        BulletScript.vel = Vector3.zero;
        BulletScript.rb.velocity = Vector3.zero;
        BulletScript.ReturnBullet();
        PlayerScript.dir = PlayerMoveDir.RIGHT; //強制右向き
        RotationStand();

        // 音停止
        SoundManager.Instance.FadeSound(SOUND_FADE_TYPE.OUT, 1.0f, 0.0f, true);

        PlayerScript.ResetAnimation();

        state = ClearState.MIDAIR;
        //if (PlayerScript.isOnGround)
        //{
        //    state = ClearState.WALK;

        //    ////SEVoice
        //    //SoundManager.Instance.PlaySound("goal");
        //}
        //else
        //{
        //    state = ClearState.MIDAIR;
        //}
    }

    public override void UpdateState()
    {
        BulletScript.ReturnBullet();
    }

    public override void Move()
    {
        Debug.Log(state);
        if (state == ClearState.MIDAIR)
        {
            PlayerScript.vel += Vector3.down * PlayerScript.FALL_GRAVITY * (fixedAdjust);
            PlayerScript.vel.y = Mathf.Max(PlayerScript.vel.y, PlayerScript.MAX_FALL_SPEED * -1);


            if (PlayerScript.isOnGround)
            {
                state = ClearState.WALK;
                PlayerScript.vel.y = 0.0f;

                ////SEVoice
                //SoundManager.Instance.PlaySound("goal");
            }
        }
        else if(state == ClearState.WALK)
        {
            //右に歩く
            PlayerScript.vel.x = Mathf.Min(PlayerScript.vel.x += 5.0f, 30.0f);

            PlayerScript.animator.SetBool("isRunning", true);
            Debug.Log(Vector3.Distance(PlayerScript.rb.position, goal.transform.position));

            //速度を参照
            float animBlend = Mathf.Abs(PlayerScript.vel.x);
            animBlend = Mathf.Clamp(animBlend, 0.0f, PlayerScript.MAX_RUN_SPEED);
            PlayerScript.animator.SetFloat(Animator.StringToHash("RunSpeed"), animBlend);
          
            //到着したら
            if (Vector3.Distance(PlayerScript.rb.position, goal.transform.position) < AnimStartDistance)
            {
                state = ClearState.ANIMMOTION;            
                PlayerScript.vel.x = 0.0f;

                PlayerScript.animator.SetBool("isRunning", false);
                PlayerScript.animator.SetTrigger("ClearTrigger");

                switch (GameStateManager.GetGameRank())
                {
                    case GAME_RANK.S:
                        PlayerScript.animator.SetInteger("ClearType", 0);
                        break;

                    case GAME_RANK.A:
                        PlayerScript.animator.SetInteger("ClearType", 1);
                        break;

                    case GAME_RANK.B:
                        PlayerScript.animator.SetInteger("ClearType", 2);
                        break;
                }

                //int animNum = Random.Range(0, 2);
                //animNum += 1;
                //PlayerScript.animator.SetInteger("ClearType", animNum);

                goal.GetComponent<Animator>().SetTrigger("OpenTrigger");
            }
        }
        else if (state == ClearState.ANIMMOTION)
        {
            //motionTimer += Time.fixedDeltaTime;

            //if(motionTimer > 5.9f)
            //{
            //    GoalManager.Instance.StartMotionBlur();
            //}
        }

    }
}
