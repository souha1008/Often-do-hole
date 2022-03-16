using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Interface : MonoBehaviour
{
    [SerializeField] protected Vector3 Vel;          // 移動量
    [SerializeField] protected Vector3 TotalMoveVel; // 自分の合計移動量
    protected Rigidbody Rb;         // リジッドボディ


    virtual public void Init() { } // スタート処理
    virtual public void Move() { }  // 敵の動き処理
    virtual public void Death() { } // 敵死亡処理
    virtual public void OnTriggerEnter(Collider collider) { }    // 何かと衝突判定(トリガー)
    //virtual public void OnCollisionEnter(Collision collision) { }   // 何かと衝突判定(コリジョン)

    protected void Update()
    {
        
    }

    protected void Start() 
    {
        // 初期化
        Vel = new Vector3(0, 0, 0);
        TotalMoveVel = new Vector3(0, 0, 0);
        Rb = null;

        // リジッドボディ
        if ((Rb = this.GetComponent<Rigidbody>()) == null)
        {
            Rb = this.gameObject.AddComponent<Rigidbody>();
        }
        Rb.isKinematic = false; // キネマティックオフ
        Rb.useGravity = false;  // 重力オフ

        // コライダー
        this.GetComponent<Collider>().isTrigger = true; // トリガーオン

        // スタート処理
        Init();
    }
    protected void FixedUpdate() 
    {
        Move();                 // 敵の動き処理
        TotalMoveVel += Vel;    // 合計移動量変更
        Rb.velocity = Vel;      // 移動量変更
    }
}
