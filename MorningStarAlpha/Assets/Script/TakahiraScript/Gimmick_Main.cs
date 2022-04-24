using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ギミックメインクラス(継承して使う)
public abstract class Gimmick_Main : MonoBehaviour
{
    [System.Serializable]
    public class NowMoveInfo
    {
        [SerializeField, Label("現在の移動量")]
        public Vector3 Vel;          // 移動量

        //[SerializeField, Label("合計移動量")]
        //public Vector3 TotalMoveVel; // 自分の合計移動量
    }
    [Label("現在の移動情報")]
    public NowMoveInfo MoveInfo;


    // 変数
    protected Vector3 Vel;          // 移動量
    protected Vector3 TotalMoveVel; // 自分の合計移動量

    protected Rigidbody Rb;         // リジッドボディ
    protected Collider Cd;          // コライダー

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
        Cd = this.GetComponent<Collider>(); // コライダー取得
        Cd.isTrigger = true;    // トリガーオン

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
        //TotalMoveVel += Vel;    // 合計移動量変更
        Rb.velocity = Vel;      // 移動量変更


        MoveInfo.Vel = Vel;
        //MoveInfo.TotalMoveVel = TotalMoveVel;
    }
}
