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

    private static float FallLength_X = 80.0f;  // X座標の降らせる幅
    private static float FallLength_Z = 100.0f;  // Z座標の降らせる幅
    private static float StartVel_Y = -100.0f;      // 最初に加えるY方向移動量
    private static float StartPos_Y = 90.0f;    // 初期Y座標
    private static float EndPos_Y = -120.0f;    // 消失Y座標
    private float JewelFallTime = 0.01f; // ジュエルが降る時間(秒)
    private int JewelFallNum = 2;       // ジュエルが一度に降る量
    private float NowTime = 0.0f;       // 現在時間
    //private float FallSpeed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 時間に達していたら降らせる
        if (NowTime >= JewelFallTime)
        {
            for (int i = 0; i < JewelFallNum; i++)
            {
                int Jewel_rand = Random.Range(0, 8);                            // ランダムでジュエル選ぶ
                float Pos_rand_X = Random.Range(-FallLength_X, FallLength_X);   // X座標指定
                float Pos_rand_Z = Random.Range(0.0f, FallLength_Z);            // Z座標指定

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
                        JewelList[j].Vec = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)); // 回転量ランダム
                        break;
                    }
                }

                if (!ReUseFlag)
                {
                    GameObject Jw = Instantiate(Jewel[Jewel_rand], FallPos, Quaternion.identity);
                    Rigidbody JwRb = Jw.GetComponent<Rigidbody>();
                    Vector3 JwVec = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)); // 回転量ランダム
                    JwRb.AddForce(0.0f, StartVel_Y, 0.0f, ForceMode.VelocityChange);   // 最初に加えるY方向移動量
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
