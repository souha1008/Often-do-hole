using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


/// <summary>
/// ���[���ړ����̃N���X
/// </summary>
public class PlayerState_Rail : PlayerState
{

    public PlayerState_Rail()
    {
        //Init();
        


        ////�e�̔���
        //BulletScript.GetComponent<Collider>().isTrigger = false;
        //BulletScript.VisibleBullet();

        ////if (is_slide_jump)
        ////{
        ////    BulletScript.ShotSlideJumpBullet();
        ////    Debug.Log("Slide Shot");
        ////}
        
        //{
        //    BulletScript.ShotBullet();
        //    Debug.Log("Normal Shot");
        //}
    }

    public override void UpdateState()
    {
        //�L�[���͕s��
    }

    public override void Move()
    {
        
    }

    public override void StateTransition()
    {
        //�I�������X�e�[�g

        ////�{�[�����G�ꂽ��X�C���O���
        //if (BulletScript.isTouched)
        //{
        //    PlayerScript.shotState = ShotState.NONE;
        //    if (BulletScript.swingEnd)
        //    {
        //        BulletScript.swingEnd = false;
        //        PlayerScript.mode = new PlayerStateSwing();
        //    }
        //}

    }
    public override void DebugMessage()
    {
        Debug.Log("PlayerState:Rail");
    }
}



