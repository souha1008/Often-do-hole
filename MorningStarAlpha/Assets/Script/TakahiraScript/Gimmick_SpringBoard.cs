using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_SpringBoard : MonoBehaviour
{
    // 変数
    public float SpringPower = 3000.0f;   // ジャンプ台の力

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Collider>().isTrigger = true;  // トリガーオン
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 何かと衝突処理(トリガー)
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            float Rad;           // 回転角
            Vector3 VecPower;    // 加えるベクトル量

<<<<<<< HEAD
            PlayerMain playermain = collider.gameObject.GetComponent<PlayerMain>();
            //Rigidbody Rb = collider.gameObject.transform.GetComponent<Rigidbody>(); // リジッドボディ
            Rad = this.transform.localEulerAngles.z;  // ジャンプ台の回転角
=======
            PlayerMain playermain = collider.gameObject.GetComponent<PlayerMain>(); // プレイヤーメインスクリプト取得
            Rad = this.transform.localEulerAngles.z;  // ジャンプ台の回転角(度)
>>>>>>> d9a0efad51fe98f2d16617ef743cb64dfe8d7231
            Rad = CalculationScript.AngleCalculation(Rad); // 角度ラジアン変換
            VecPower = CalculationScript.AngleVectorXY(Rad) * SpringPower;  // 飛ぶベクトル量

            if (VecPower.x < 1 && VecPower.x > -1) VecPower.x = 0;  // 小さい値は誤差として0にする
            if (VecPower.y < 1 && VecPower.y > -1) VecPower.y = 0;

            //Debug.Log(VecPower.x);
            //Debug.Log(VecPower.y);

<<<<<<< HEAD
            playermain.vel = new Vector3 (VecPower.x, VecPower.y, 0);
            //Rb.velocity = new Vector3 (VecPower.x, VecPower.y, 0);
            //Rb.AddForce(VecPower,ForceMode.VelocityChange);
=======
            playermain.vel = VecPower; // プレイヤーのベクトル量変更
>>>>>>> d9a0efad51fe98f2d16617ef743cb64dfe8d7231
        }
    }
}
