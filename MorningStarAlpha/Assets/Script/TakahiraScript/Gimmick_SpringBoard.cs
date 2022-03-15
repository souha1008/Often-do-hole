using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_SpringBoard : MonoBehaviour
{
    public float SpringPower = 3000.0f;   // ジャンプ台の力

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            float Rad;           // 回転角
            Vector3 VecPower;    // 加えるベクトル量

            PlayerMain playermain = collider.gameObject.GetComponent<PlayerMain>(); // プレイヤーメインスクリプト取得
            Rigidbody Rb = collider.gameObject.transform.GetComponent<Rigidbody>(); // リジッドボディ
            Rad = this.transform.localEulerAngles.z;  // ジャンプ台の回転角
            Rad = CalculationScript.AngleCalculation(Rad); // 角度ラジアン変換
            VecPower = CalculationScript.AngleVectorXY(Rad) * SpringPower;  // 飛ぶベクトル量

            if (VecPower.x < 1 && VecPower.x > -1) VecPower.x = 0;
            if (VecPower.y < 1 && VecPower.y > -1) VecPower.y = 0;

            Debug.Log(VecPower.x);
            Debug.Log(VecPower.y);

            playermain.vel = VecPower; // プレイヤーのベクトル量変更
        }
    }
}
