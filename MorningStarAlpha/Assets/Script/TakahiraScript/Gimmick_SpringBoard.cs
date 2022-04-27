using UnityEngine;

public class Gimmick_SpringBoard : Gimmick_Main
{
    // 変数
    [Label("ジャンプ台の力")]
    public float SpringPower = 100.0f;   // ジャンプ台の力

    private static float ReUseTime = 1.0f;
    private float Time;

    public override void Init()
    {
        Time = ReUseTime;
    }

    public override void FixedMove()
    {
        if (Time < ReUseTime)
            Time++;
    }

    public override void Death()
    {
        
    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player") && Time >= ReUseTime)
        {
            float Rad;           // 回転角
            Vector3 VecPower = Vector3.zero;    // 加えるベクトル量

            PlayerMain playermain = collider.gameObject.GetComponent<PlayerMain>(); // プレイヤーメインスクリプト取得
            PlayerMain.instance.BulletScript.rb.position =  PlayerMain.instance.rb.position;   //プレイヤーと弾のいちを等しくする
            Rad = this.transform.localEulerAngles.z;  // ジャンプ台の回転角(度)
            Rad = CalculationScript.AngleCalculation(Rad); // 角度ラジアン変換
            VecPower = CalculationScript.AngleVectorXY(Rad) * SpringPower;  // 飛ぶベクトル量

            if (VecPower.x < 1 && VecPower.x > -1) VecPower.x = 0;  // 小さい値は誤差として0にする
            if (VecPower.y < 1 && VecPower.y > -1) VecPower.y = 0;

            PlayerMain.instance.mode = new PlayerStateMidair(true, MidairState.BOOST);
            //PlayerMain.instance.ForciblyReleaseMode(false);

            PlayerMain.instance.vel = Vector3.zero;
            PlayerMain.instance.BulletScript.rb.velocity = Vector3.zero;
            PlayerMain.instance.addVel = VecPower;

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySound("決定音");

            VibrationManager.Instance.StartVibration(1, 1, 0.2f);

            Time = 0.0f;
        }
    }
}
