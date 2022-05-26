using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelFall : MonoBehaviour
{
    public GameObject[] Jewel = new GameObject[8];
    List<GameObject> JewelList = new List<GameObject>();
    List<Rigidbody> JewelRbList = new List<Rigidbody>();
    List<Vector3> JewelVecList = new List<Vector3>();

    private static float FallLength_X = 80.0f;
    private static float FallLength_Z = 80.0f;
    private float JewelFallTime = 0.02f; // ジュエルが降る時間
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
        if (NowTime >= JewelFallTime)
        {
            for (int i = 0; i < JewelFallNum; i++)
            {
                int Jewel_rand = Random.Range(0, 8);
                float Pos_rand_X = Random.Range(-FallLength_X, FallLength_X);
                float Pos_rand_Z = Random.Range(0.0f, FallLength_Z);

                Vector3 FallPos = new Vector3(Pos_rand_X, 30.0f, Pos_rand_Z);

                GameObject Jw = Instantiate(Jewel[Jewel_rand], FallPos, Quaternion.identity);
                JewelList.Add(Jw);
                JewelRbList.Add(Jw.GetComponent<Rigidbody>());
                JewelVecList.Add(new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)));
            }

            NowTime = 0.0f;
        }
        else
            NowTime += Time.deltaTime;

        for (int i = 0; i < JewelList.Count; i++)
        {
            if (JewelList[i].transform.position.y <= -200.0f)
            {
                Destroy(JewelList[i]);
                JewelList.RemoveAt(i);
                JewelRbList.RemoveAt(i);
                JewelVecList.RemoveAt(i);
                i--;
            }
            else
            {
                JewelRbList[i].rotation *= Quaternion.Euler(JewelVecList[i].x, JewelVecList[i].y, JewelVecList[i].z);
            }
        }

    }
}
