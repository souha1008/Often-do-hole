using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_CannonChild_Killer : Gimmick_CannonChild
{
    private bool BulletMoveFlag;

    [Label("動き方")]
    public FALL_TYPE FallType;

    [Label("落下距離")]
    public float FallLength = 15.0f;    // 落ちる距離

    [Label("何秒かけて落下するか")]
    public float FallTime = 3.0f;       // 何秒かけて落下するか


    private bool NowFall;               // 落下中か
    private float NowTime;              // 経過時間
    private float StartPosY;            // 初期座標
    private float FallPosY;            // 落下座標
    private float OldFallPosY;            // １つ前の落下座標

    public override void Init()
    {
        // 初期化
        Cd.isTrigger = true;
        BulletMoveFlag = false;

        NowFall = false;
        NowTime = 0.0f;
        StartPosY = FallPosY = OldFallPosY = this.gameObject.transform.position.y;

        Rb.mass = 1000000000;   // 重さ変更
    }

    public override void FixedMove()
    {
        base.FixedMove();
       
        // 落下移動
        if (NowFall)
        {
            switch (FallType)
            {
                case FALL_TYPE.SINE_IN:
                    FallPosY = Easing.SineIn(NowTime, FallTime, StartPosY, StartPosY - FallLength);
                    break;
                case FALL_TYPE.QUAD_IN:
                    FallPosY = Easing.QuadIn(NowTime, FallTime, StartPosY, StartPosY - FallLength);
                    break;
                case FALL_TYPE.CUBIC_IN:
                    FallPosY = Easing.CubicIn(NowTime, FallTime, StartPosY, StartPosY - FallLength);
                    break;
                case FALL_TYPE.QUART_IN:
                    FallPosY = Easing.QuartIn(NowTime, FallTime, StartPosY, StartPosY - FallLength);
                    break;
                case FALL_TYPE.QUINT_IN:
                    FallPosY = Easing.QuintIn(NowTime, FallTime, StartPosY, StartPosY - FallLength);
                    break;
                case FALL_TYPE.EXPO_IN:
                    FallPosY = Easing.ExpIn(NowTime, FallTime, StartPosY, StartPosY - FallLength);
                    break;
                case FALL_TYPE.BOUNCE_IN:
                    FallPosY = Easing.BounceIn(NowTime, FallTime, StartPosY, StartPosY - FallLength);
                    break;
                case FALL_TYPE.ELASTIC_IN:
                    FallPosY = Easing.ElasticIn(NowTime, FallTime, StartPosY, StartPosY - FallLength);
                    break;
            }

            // 移動量更新
            Vel.y += (FallPosY - OldFallPosY) * 1 / Time.fixedDeltaTime;

            OldFallPosY = FallPosY;

            // 時間更新
            NowTime += Time.fixedDeltaTime;

            if (NowTime > FallTime)
            {
                Death();
                //Debug.LogWarning("落下経過で死亡");
            }
        }


        // 錨オブジェクトの移動
        if (BulletMoveFlag)
        {
            if (PlayerMain.instance.BulletScript.isTouched)
            {
                PlayerMain.instance.BulletScript.transform.position += Vel * Time.fixedDeltaTime;
                PlayerMain.instance.addVel = Vel;

                //PlayerMain.instance.addVel =
                //    new Vector3(this.gameObject.transform.position.x - OldPos.x, this.gameObject.transform.position.y - OldPos.y, 0) * 1 / Time.deltaTime;
            }
            else
            {
                BulletMoveFlag = false;
                PlayerMain.instance.addVel = Vector3.zero;
            }
        }
    }

    public override void Death()
    {
        // プレイヤーの錨引き戻し
        if (BulletMoveFlag)
        {
            PlayerMain.instance.mode = new PlayerStateMidair(true, MidairState.NORMAL);
            PlayerMain.instance.ForciblyReleaseMode(true);
            PlayerMain.instance.endSwing = true;
            PlayerMain.instance.floorVel = Vector3.zero;
        }

        Init();

        // エフェクト
        EffectManager.Instance.SharkExplosionEffect(this.transform.position, 4.0f);

        // 非アクティブ化
        this.gameObject.SetActive(false);
    }

    public override void OnTriggerEnter(Collider collider)
    {
        // プレイヤーと衝突(このオブジェクトに刺さってない)
        if (collider.gameObject.CompareTag("Player") && !BulletMoveFlag)
        {
            if (PlayerMain.instance.refState == EnumPlayerState.SWING)
            {
                // 自身が死亡
                Death();
            }
            // ノックバック
            else
            {
                // ヒットストップ
                //GameSpeedManager.Instance.StartHitStop(0.1f);

                // 振動
                VibrationManager.Instance.StartVibration(0.7f, 0.7f, 0.2f);

                // プレイヤーをノックバック状態に変更
                PlayerMain.instance.mode = new PlayerState_Knockback(this.transform.position, false);

                //Debug.LogWarning("死亡:" + collider.gameObject.name);

                // 自身が死亡
                Death();
            }
            return;
        }
        
        // 壁とかに衝突
        if (!(collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("Bullet") || collider.gameObject.CompareTag("Chain")))
        {
            //Debug.LogWarning("死亡:" + collider.gameObject.name);

            // 自身が死亡
            Death();
            return;
        }
    }

    public override void GimmickBulletStart(GameObject collision)
    {
        if (collision.gameObject == this.gameObject)
        {
            BulletMoveFlag = true;
            StartPosY = FallPosY = this.gameObject.transform.position.y;
            NowFall = true;
        }
    }
}
