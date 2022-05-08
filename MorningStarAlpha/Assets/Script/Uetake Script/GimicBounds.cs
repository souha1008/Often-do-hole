//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class GimicBounds : MonoBehaviour
//{
//    float Timer = 0.0f;

//    private void Update()
//    {
//        Timer += Time.deltaTime;

//        if (Timer > 1.0f)
//        {
//            if (Vector3.Distance(BulletMain.instance.rb.position, gameObject.transform.position) < 10.0f)
//            {
//                Debug.Log("AAA");
//                Vector3 vec = BulletMain.instance.vel.normalized;
//                BulletMain.instance.vel = PlayerMain.instance.adjustLeftStick * 80.0f;
//            }

//            Timer = 0.0f;
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
     
//    }
//}

