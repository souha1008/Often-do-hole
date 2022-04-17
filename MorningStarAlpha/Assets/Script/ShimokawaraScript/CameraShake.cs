using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    bool ShakeNow = false;
    int ShakeCnt = 0;
    int[] ShakeArray = { 1, 2, 1, 0, -1, -2, -1, 0 }; 

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        ShakeNow = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(ShakeNow)
        {
            transform.rotation = Quaternion.Euler(ShakeArray[ShakeCnt], 0, 0);

            ShakeCnt++;            

            if (ShakeCnt >= 8)
            {
                ShakeCnt = 0;
                ShakeNow = false;
            }
        }

    }

    public void Shake()
    {
        ShakeNow = true;
    }
}
