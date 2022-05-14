﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_FallBlock_2 : Gimmick_Main
{
    [Label("動き方")]
    public FALL_TYPE FallType;

    [Label("落下距離")]
    public float FallLength = 15.0f;    // 落ちる距離

    [Label("何秒かけて落下するか")]
    public float FallTime = 3.0f;       // 何秒かけて落下するか

    [Label("リスポーン時間")]
    public float RespawnTime = 3.0f;       // 何秒で復活するか

    private bool ActiveFlag = true;


    private bool NowFall;               // 落下中か
    private float NowTime;              // 経過時間
    private Vector3 StartPos;           // 初期座標
    private Vector3 FallPos;            // 落下座標

    private bool PlayerMoveFlag = false;
    private bool BulletMoveFlag = false;


    // 揺れ情報
    private struct ShakeInfo
    {
        public ShakeInfo(float shakeTime, float speed, float power, Vector2 randomOffset)
        {
            ShakeTime = shakeTime;
            Speed = speed;
            Power = power;
            RandomOffset = randomOffset;
        }
        public float ShakeTime { get; } // 時間
        public float Speed { get; } // 揺れの速さ
        public float Power { get; }  // 振動量
        public Vector2 RandomOffset { get; } // ランダムオフセット値
    }

    private ShakeInfo Shake;        // 揺れ情報
    private bool ShakeFlag;         // 揺れ実行中か
    private float NowShakeTime;     // 揺れ経過時間


    public override void Init()
    {
        // 初期化
        NowFall = false;
        NowTime = 0.0f;
        StartPos = FallPos = this.gameObject.transform.position;
        PlayerMoveFlag = false;
        BulletMoveFlag = false;

        ShakeFlag = false;
        NowShakeTime = 0.0f;
        ActiveFlag = true;

        // リジッドボディ
        Rb.isKinematic = true;  // キネマティックオン
    }

    public override void FixedMove()
    {
        if (ActiveFlag)
        {
            // 揺れ開始
            if (NowFall && NowTime <= 0)
                StartShake(FallTime * 0.2f, 20.0f, 0.2f);  // 揺れの情報セット

            // 床移動
            if (NowFall)
            {
                Vector3 OldPos = this.gameObject.transform.position;
                switch (FallType)
                {
                    case FALL_TYPE.SINE_IN:
                        FallPos = new Vector3(StartPos.x, Easing.SineIn(NowTime, FallTime, StartPos.y, StartPos.y - FallLength), StartPos.z);
                        break;
                    case FALL_TYPE.QUAD_IN:
                        FallPos = new Vector3(StartPos.x, Easing.QuadIn(NowTime, FallTime, StartPos.y, StartPos.y - FallLength), StartPos.z);
                        break;
                    case FALL_TYPE.CUBIC_IN:
                        FallPos = new Vector3(StartPos.x, Easing.CubicIn(NowTime, FallTime, StartPos.y, StartPos.y - FallLength), StartPos.z);
                        break;
                    case FALL_TYPE.QUART_IN:
                        FallPos = new Vector3(StartPos.x, Easing.QuartIn(NowTime, FallTime, StartPos.y, StartPos.y - FallLength), StartPos.z);
                        break;
                    case FALL_TYPE.QUINT_IN:
                        FallPos = new Vector3(StartPos.x, Easing.QuintIn(NowTime, FallTime, StartPos.y, StartPos.y - FallLength), StartPos.z);
                        break;
                }


                // 揺れによる移動
                if (ShakeFlag)
                {
                    // 揺れ位置情報更新
                    this.gameObject.transform.position = GetUpdateShakePosition(
                        Shake,
                        NowShakeTime,
                        FallPos);

                    // ShakeTime分の時間が経過したら揺らすのを止める
                    NowShakeTime += Time.fixedDeltaTime;
                    if (NowShakeTime >= Shake.ShakeTime)
                    {
                        ShakeFlag = false;
                        NowShakeTime = 0.0f;
                        // 初期位置に戻す
                        this.gameObject.transform.position = FallPos;
                    }
                }
                else
                {
                    this.gameObject.transform.position = FallPos;
                }

                // 時間更新
                NowTime += Time.fixedDeltaTime;


                // プレイヤー移動
                if (PlayerMoveFlag)
                {
                    PlayerMain.instance.transform.position +=
                                new Vector3(0, this.gameObject.transform.position.y - OldPos.y, 0);
                    //PlayerMainScript.addVel = new Vector3(0, this.gameObject.transform.position.y - OldPos.y, 0) / Time.fixedDeltaTime;
                }

                // 錨移動
                if (BulletMoveFlag)
                {
                    if (PlayerMain.instance.BulletScript.isTouched)
                    {
                        PlayerMain.instance.BulletScript.transform.position +=
                            new Vector3(0, this.gameObject.transform.position.y - OldPos.y, 0);
                        PlayerMain.instance.floorVel =
                            new Vector3(0, this.gameObject.transform.position.y - OldPos.y, 0) * 1 / Time.deltaTime;
                    }
                    else
                    {
                        PlayerMain.instance.floorVel = Vector3.zero;
                        BulletMoveFlag = false;
                    }
                }
            }
            if (NowTime > FallTime)
            {
                Death();
            }
        }
        else
        {
            if (NowTime > RespawnTime)
            {
                Active();
            }
            // 時間更新
            NowTime += Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// 更新後の揺れ位置を取得
    /// </summary>
    /// <param name="shakeInfo">揺れ情報</param>
    /// <param name="nowShakeTime">経過時間</param>
    /// <param name="shakeStartPos">初期位置</param>
    /// <returns>更新後の揺れ位置</returns>
    private Vector3 GetUpdateShakePosition(ShakeInfo shakeInfo, float nowShakeTime, Vector3 shakeStartPos)
    {
        // パーリンノイズ値(-1.0〜1.0)を取得
        var Speed = shakeInfo.Speed;
        var randomOffset = shakeInfo.RandomOffset;
        var randomX = CalculationScript.GetPerlinNoiseValue(randomOffset.x, Speed, nowShakeTime);
        var randomY = CalculationScript.GetPerlinNoiseValue(randomOffset.y, Speed, nowShakeTime);

        // -Speed ~ Speed の値に変換
        randomX *= Speed;
        randomY *= Speed;

        // -Power ~ Power の値に変換
        var vibrato = shakeInfo.Power;
        var ratio = 1.0f - nowShakeTime / shakeInfo.ShakeTime;
        vibrato *= ratio; // フェードアウトさせるため、経過時間により揺れの量を減衰
        randomX = Mathf.Clamp(randomX, -vibrato, vibrato);
        randomY = Mathf.Clamp(randomY, -vibrato, vibrato);

        // 初期位置に加えて設定
        var position = shakeStartPos;
        position.x += randomX;
        position.y += randomY;
        return position;
    }

    /// <summary>
    /// 揺れ開始
    /// </summary>
    /// <param name="shakeTime">時間</param>
    /// <param name="speed">揺れの速さ</param>
    /// <param name="power">どのくらい振動するか</param>
    public void StartShake(float shakeTime, float speed, float power)
    {
        // 揺れ情報を設定して開始
        var randomOffset = new Vector2(Random.Range(0.0f, 100.0f), Random.Range(0.0f, 100.0f)); // ランダム値はとりあえず0〜100で設定
        Shake = new ShakeInfo(shakeTime, speed, power, randomOffset);
        ShakeFlag = true;
        NowShakeTime = 0.0f;
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

        // 自分自身を消す
        //Destroy(this.gameObject);

        NowFall = false;
        BulletMoveFlag = false;
        PlayerMoveFlag = false;

        // 非アクティブ化
        NoActive();
    }

    public override void OnCollisionEnter(Collision collision)
    {
        // プレイヤーか錨と接触
        if (collision.gameObject.CompareTag("Bullet"))
        {
            NowFall = true; // 落下中
            BulletMoveFlag = true;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("プレイヤー当たった");
            // プレイヤーからレイ飛ばして真下にブロックがあったら落下
            if (PlayerMain.instance.getFootHit().collider != null &&
                PlayerMain.instance.getFootHit().collider.gameObject == this.gameObject)
            {
                NowFall = true; // 落下中
                PlayerMoveFlag = true;
            }
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (PlayerMain.instance.getFootHit().collider != null &&
                PlayerMain.instance.getFootHit().collider.gameObject == this.gameObject)
            {
                NowFall = true; // 落下中
                PlayerMoveFlag = true;
            }
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMoveFlag = false;
        }
    }

    private void NoActive()
    {
        // 非アクティブ化
        Collider[] colliders = GetComponents<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }
        GetComponent<MeshRenderer>().enabled = false;
        ActiveFlag = false;
        NowTime = 0;
        this.gameObject.transform.position = StartPos;
    }

    private void Active()
    {
        // アクティブ化
        Collider[] colliders = GetComponents<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = true;
        }
        GetComponent<MeshRenderer>().enabled = true;
        ActiveFlag = true;
        NowTime = 0;
    }
}
