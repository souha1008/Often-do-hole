using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ギミックメインクラス(継承して使う)
public abstract class Gimmick_Main : MonoBehaviour
{
    [System.Serializable]
    public class NowMoveInfo
    {
        [Header("現在の角度")]
        [Tooltip("0～360度で表示しています")]
        public Vector3 Rad;          // 角度

        [Header("現在の移動量")]
        public Vector3 Vel;          // 移動量

        [Header("合計移動量")]
        public Vector3 TotalMoveVel; // 自分の合計移動量
    }
    [Tooltip("現在の移動情報")]
    public NowMoveInfo MoveInfo;

    // 変数
    protected Vector3 Rad;          // 角度
    protected Vector3 Vel;          // 移動量
    protected Vector3 TotalMoveVel; // 自分の合計移動量

    protected Rigidbody Rb;         // リジッドボディ

    // 継承するもの
    public abstract void Init(); // スタート処理
    public virtual void FixedMove() { }  // ギミックの動き処理
    public virtual void UpdateMove() { }   // Updateを使った動き処理
    public abstract void Death(); // ギミック死亡処理
    public virtual void OnTriggerEnter(Collider collider) { }    // 何かと衝突処理(トリガー)
    public virtual void OnCollisionEnter(Collision collision) { }   // 何かと衝突処理(コリジョン)


    // 継承先で自動で動く処理(プロテクト)
    protected void Start() 
    {
        // 初期化
        Rad = this.gameObject.transform.rotation.eulerAngles;
        Vel = Vector3.zero;
        TotalMoveVel = Vector3.zero;
        Rb = null;

        // リジッドボディ
        if ((Rb = this.GetComponent<Rigidbody>()) == null)
        {
            Rb = this.gameObject.AddComponent<Rigidbody>();
        }
        Rb.isKinematic = false; // キネマティックオフ
        Rb.useGravity = false;  // 重力オフ
        Rb.constraints = RigidbodyConstraints.None;             // フリーズを全部解除
        Rb.constraints = RigidbodyConstraints.FreezeRotation;   // 回転のフリーズを全部オン


        // コライダー
        this.GetComponent<Collider>().isTrigger = true; // トリガーオン

        // スタート処理
        Init();
    }
    protected void Update()
    {
        UpdateMove();
    }
    protected void FixedUpdate() 
    {
        FixedMove();                 // ギミックの動き処理
        TotalMoveVel += Vel;    // 合計移動量変更
        Rb.velocity = Vel;      // 移動量変更

        // 0～360度に変更
        if (Rad.x > 360 || Rad.x < 0 ||
            Rad.y > 360 || Rad.y < 0 ||
            Rad.z > 360 || Rad.z < 0)
        {
            if (Rad.x > 360) Rad.x -= 360;
            if (Rad.x < 0) Rad.x += 360;
            if (Rad.y > 360) Rad.y -= 360;
            if (Rad.y < 0) Rad.y += 360;
            if (Rad.z > 360) Rad.z -= 360;
            if (Rad.z < 0) Rad.z += 360;
        }
        Rb.rotation = Quaternion.Euler(Rad);    // 角度変更

        MoveInfo.Rad = Rad;
        MoveInfo.Vel = Vel;
        MoveInfo.TotalMoveVel = TotalMoveVel;
    }
}
