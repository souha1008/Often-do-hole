using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JwelFall : MonoBehaviour
{
    GameObject[] jwel;

    // Start is called before the first frame update
    void Start()
    {
        jwel[0] = Resources.Load<GameObject>("Prefabs/jowel/aoJewel.prefab");
        jwel[1] = Resources.Load<GameObject>("Prefabs/jowel/emerald.prefab");
        jwel[2] = Resources.Load<GameObject>("Prefabs/jowel/gold.prefab");
        jwel[3] = Resources.Load<GameObject>("Prefabs/jowel/greenJewel.prefab");
        jwel[4] = Resources.Load<GameObject>("Prefabs/jowel/kin.prefab");
        jwel[5] = Resources.Load<GameObject>("Prefabs/jowel/Lcoin.prefab");
        jwel[6] = Resources.Load<GameObject>("Prefabs/jowel/redJewel.prefab");
        jwel[7] = Resources.Load<GameObject>("Prefabs/jowel/ring.prefab");
        jwel[8] = Resources.Load<GameObject>("Prefabs/jowel/Scoin.prefab");
    }

    // Update is called once per frame
    void Update()
    {
        int juwel_rand = Random.Range(0, 9);

    }
}
