using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerY_Ziku : MonoBehaviour
{
    public static PlayerY_Ziku instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        float TempX = PlayerMain.instance.transform.position.x;
        //float TempY = GameObject.Find("Main Camera").transform.position.y;
        float TempY = transform.position.y;

        transform.position = new Vector3(TempX, TempY, 0);
        transform.eulerAngles = Vector3.zero;
    }
}
