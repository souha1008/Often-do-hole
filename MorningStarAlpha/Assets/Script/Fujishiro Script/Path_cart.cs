using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Path_cart : MonoBehaviour
{
    public static Raen_Path waypoints; // ウェイポイント格納用
    public GameObject ikari;    // イカリオブジェクト格納

    public bool Rall_Start = false;
    private bool Rall_Now = false;
    int pointNo;

    void Start()
    {
        Rall_Start = false;
        Rall_Now = false;
        pointNo = waypoints.Path.Length;
    }

    void Update()
    {
        // データ保守用
        if(waypoints != null)
        {
            if(Rall_Start == true && Rall_Now == false)
            {
                PlayerState.PlayerScript.mode = new PlayerStateRail(); // ステートをレール状態に移行
                ikari.transform.DOPath(
                    waypoints.Path, 
                    6.0f)
                    .OnWaypointChange(pointNo =>{
                        if(pointNo > 0)
                        {

                        }
                    
                    });          // パス情報で動かす
                Rall_Now = true;                                       // 一回のみ用フラグをオン
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Bullet") && Rall_Now == false)
        {
            Rall_Start = true;
        }
    }
}
