using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    bool ShakeNow = false;
    int ShakeCnt = 0;
    int[] ShakeArray = { 2, 4, 2, 0, -1, /*-2, -1,*/ 0 /*,1,2,1,0,-1,-2,-1,0*/};

    public float ShakeMulti = 0.5f;

    Vector3 Angle = Vector3.zero;

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
            transform.rotation = Quaternion.Euler(ShakeArray[ShakeCnt / 2] * ShakeMulti * -Angle.x, ShakeArray[ShakeCnt / 2] * ShakeMulti * -Angle.y, 0);

            ShakeCnt++;            

            if (ShakeCnt / 2 >= ShakeArray.Length)
            {
                ShakeCnt = 0;
                ShakeNow = false;
            }
        }

    }

    public void Shake(Vector3 angle)
    {
        if(ShakeNow == false)
        {
            Angle = angle.normalized;
            ShakeNow = true;
        }
    }
}
