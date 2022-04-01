using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Bridge : Gimmick_Main
{
    [Label("倒れてくる時間")]
    public float MoveTime = 3.0f;

    private bool StartFlag;
    private bool Endflag;
    private float NowTime;
    private Vector3 StartPos;
    private float OldRad;
    private GameObject Player;

    public override void Init()
    {
        // 初期化
        StartFlag = false;
        Endflag = false;
        NowTime = 0.0f;
        OldRad = 0.0f;

        // リジッドボディ
        Rb.isKinematic = true;

        // コライダー
        Cd.isTrigger = false;       

        // プレイヤー取得
        Player = GameObject.Find("Player");

        // 初期回転
        Vector3 ThisPos = StartPos = this.gameObject.transform.position; // 自分の座標
        Vector3 RotatePos = new Vector3(ThisPos.x, ThisPos.y, ThisPos.z + this.gameObject.transform.localScale.z * 0.5f); // 回転基準座標
        Quaternion AngleAxis = Quaternion.AngleAxis(90.0f, Vector3.right); // 回転軸と角度
        
        Vector3 Pos = ThisPos;

        Pos -= RotatePos;
        Pos = AngleAxis * Pos;
        Pos += RotatePos;

        this.gameObject.transform.position = Pos; // 現在の座標更新

        this.gameObject.transform.rotation *= AngleAxis; // 回転更新
    }

    public override void UpdateMove()
    {
        //if(Input.GetKeyDown(KeyCode.Q))
        float Distance = Vector2.Distance(Player.transform.position, StartPos);
        if (Distance < 55 && !StartFlag)
        {
            NowTime = 0.0f;
            StartFlag = true;
            Endflag = false;
        }
    }

    public override void FixedMove()
    {
        if (StartFlag && !Endflag)
        {
            if (NowTime > MoveTime)
            {
                NowTime = MoveTime;
                Endflag = true;
            }

            Rotate();

            NowTime += Time.fixedDeltaTime; // 時間加算
        }
    }

    public override void Death()
    {
        
    }

    private void Rotate()
    {
        Vector3 ThisPos = this.gameObject.transform.position; // 自分の座標
        Vector3 RotatePos = new Vector3(StartPos.x, StartPos.y, StartPos.z + this.gameObject.transform.localScale.z * 0.5f); // 回転基準座標
        Quaternion AngleAxis = Quaternion.AngleAxis(-(Easing.BounceOut(NowTime, MoveTime, 0, 90) - OldRad), Vector3.right); // 回転軸と角度
        OldRad = Easing.BounceOut(NowTime, MoveTime, 0, 90);

        Vector3 Pos = ThisPos;

        Pos -= RotatePos;
        Pos = AngleAxis * Pos;
        Pos += RotatePos;

        this.gameObject.transform.position = Pos; // 現在の座標更新

        this.gameObject.transform.rotation *= AngleAxis; // 回転更新
    }
}
