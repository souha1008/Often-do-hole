using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelFall : MonoBehaviour
{
    public GameObject[] Jewel = new GameObject[8];      // プレハブ格納用

    private class JEWEL_LIST
    {
        public GameObject Obj;   // 生成したプレハブ格納用
        public Rigidbody Rb;  // 生成したプレハブのリジッドボディ格納用
        public Vector3 Vec;   // 生成したプレハブのランダム回転量格納用
        public int Num;           // ジュエル番号

        public JEWEL_LIST(GameObject obj, Rigidbody rb, Vector3 vec, int num)
        {
            Obj = obj;
            Rb = rb;
            Vec = vec;
            Num = num;
        }
    }

    List<JEWEL_LIST> JewelList = new List<JEWEL_LIST>();

    private static float StartPos_Y = 90.0f;        // 初期Y座標
    private static float EndPos_Y = -140.0f;        // 消失Y座標
    private static float FallLength_X = 160.0f;     // X座標の降らせる最大幅
    private static float FallLength_Z = 125.0f;     // Z座標の降らせる最大幅
    private static float StartVel_Y = -100.0f;      // 最初に加えるY方向移動量
    private static float StartDelayTime = 1.65f;     // 最初に遅らせる時間(秒)
    private static float JewelFallTime = 0.02f;    // ジュエルが降る時間(秒)
    private static int JewelFallNum = 10;           // ジュエルが一度に降る量
    private static float JewelFallRot = 2.0f;       // ジュエルの最大回転量

    private bool StartDelayFlag = false;            // 最初に遅らせる用フラグ
    private float NowTime = 0.0f;                   // 現在時間
    //private float FallSpeed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 開始遅延処理
        if (!StartDelayFlag)
        {
            if (NowTime >= StartDelayTime)
            {
                SoundManager.Instance.PlaySound("Result_BGM");
                StartDelayFlag = true;
                NowTime = 0.0f;
            }
            else
            {
                NowTime += Time.deltaTime;
                return;
            }
        }

        // 時間に達していたら降らせる
        if (NowTime >= JewelFallTime)
        {
            for (int i = 0; i < JewelFallNum; i++)
            {
                int Jewel_rand = Random.Range(0, 8);                            // ランダムでジュエル選ぶ
                float Pos_rand_X = Random.Range(-FallLength_X, FallLength_X);   // X座標指定
                float Pos_rand_Z;   // Z座標指定

                // 後ろに多く降らす
                if (i <= JewelFallNum * 0.3f)
                    Pos_rand_Z = Random.Range(0.0f, FallLength_Z); 
                else
                    Pos_rand_Z = Random.Range(FallLength_Z * 0.5f, FallLength_Z); // Z座標指定

                Vector3 FallPos = new Vector3(Pos_rand_X, StartPos_Y, Pos_rand_Z);   // 座標入力

                bool ReUseFlag = false;
                for (int j = 0; j < JewelList.Count; j++)
                {
                    if (!JewelList[j].Obj.activeSelf && JewelList[j].Num == Jewel_rand)
                    {
                        ReUseFlag = true;

                        JewelList[j].Obj.SetActive(true);
                        JewelList[j].Obj.transform.position = FallPos;
                        JewelList[j].Rb.velocity = Vector3.zero;
                        JewelList[j].Rb.AddForce(0.0f, StartVel_Y, 0.0f, ForceMode.VelocityChange);   // 最初に加えるY方向移動量
                        JewelList[j].Vec = new Vector3(
                            Random.Range(-JewelFallRot, JewelFallRot), 
                            Random.Range(-JewelFallRot, JewelFallRot), 
                            Random.Range(-JewelFallRot, JewelFallRot)
                            ); // 回転量ランダム
                        break;
                    }
                }

                if (!ReUseFlag)
                {
                    GameObject Jw = Instantiate(Jewel[Jewel_rand], FallPos, Quaternion.identity);
                    Rigidbody JwRb = Jw.GetComponent<Rigidbody>();
                    JwRb.AddForce(0.0f, StartVel_Y, 0.0f, ForceMode.VelocityChange);   // 最初に加えるY方向移動量
                    Vector3 JwVec = new Vector3(
                            Random.Range(-JewelFallRot, JewelFallRot),
                            Random.Range(-JewelFallRot, JewelFallRot),
                            Random.Range(-JewelFallRot, JewelFallRot)
                            ); // 回転量ランダム
                    JewelList.Add(new JEWEL_LIST(Jw, JwRb, JwVec, Jewel_rand));
                }
                
            }

            NowTime = 0.0f;
        }
        else
            NowTime += Time.deltaTime;

        // プレハブ終了処理 or 移動処理
        for (int i = 0; i < JewelList.Count; i++)
        {
            if (JewelList[i].Obj.activeSelf)
            {
                if (JewelList[i].Obj.transform.position.y <= EndPos_Y)
                {
                    //Destroy(JewelList[i]);
                    //JewelList.RemoveAt(i);
                    //JewelRbList.RemoveAt(i);
                    //JewelVecList.RemoveAt(i);
                    //i--;
                    JewelList[i].Obj.SetActive(false);
                }
                else
                {
                    JewelList[i].Rb.rotation *= Quaternion.Euler(JewelList[i].Vec.x, JewelList[i].Vec.y, JewelList[i].Vec.z);
                }
            }
        }
    }
}
