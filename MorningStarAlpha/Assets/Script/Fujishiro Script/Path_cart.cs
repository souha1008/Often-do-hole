//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using DG.Tweening;
//using System.Linq;

//public class Path_cart : MonoBehaviour
//{
//    public static Raen_Path waypoints; // ウェイポイント格納用
//    private Vector3[] path;
//    private Vector3[] player_path;


//    private bool Rall_Start = false;
//    private bool Rall_Now = false;

//    void Start()
//    {
//        Rall_Start = false;
//        Rall_Now = false;
//        path = waypoints.positions.Select(target => target.transform.position).ToArray();
//        player_path = waypoints.player_postions.Select(target => target.transform.position).ToArray();


//    }

//    void Update()
//    {
//        // データ保守
//        if (waypoints != null)
//        {
//            if (Rall_Start == true && Rall_Now == false)
//            {

//                BulletMain.instance.transform
//                .DOPath(path, 4.0f, PathType.Linear)
//                .OnUpdate(() =>
//                {

//                })
//                .OnComplete(() =>
//                {
//                    Debug.Log("Complete");
//                    PlayerState.PlayerScript.mode = new PlayerStateMidair(true, MidairState.NORMAL);
//                });

//                PlayerMain.instance.transform
//                .DOPath(player_path, 4.0f);

//                Rall_Now = true;                                       // 一回のみ用フラグをオン
//            }
//        }
//    }

//    void OnTriggerEnter(Collider collider)
//    {
//        if (collider.CompareTag("Bullet") && Rall_Now == false)
//        {
//            Rall_Start = true;
//        }
//    }
//}
