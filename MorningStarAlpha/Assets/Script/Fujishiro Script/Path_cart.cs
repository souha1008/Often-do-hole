using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class Path_cart : MonoBehaviour
{
    public static Raen_Path waypoints; // ウェイポイント格納用
    public Vector3[] path;
    public GameObject ikari;    // イカリオブジェクト格納
    public Sequence sequence;

    public bool Rall_Start = false;
    private bool Rall_Now = false;
    int pointNo = 0;

    void Start()
    {
        Rall_Start = false;
        Rall_Now = false;
        path = waypoints.positions.Select(target => target.transform.position).ToArray();
    }

    void Update()
    {
        // データ保守用
        if(waypoints != null)
        {
            if(Rall_Start == true && Rall_Now == false)
            {
                PlayerState.PlayerScript.mode = new PlayerState_Rail(); // ステートをレール状態に移行
                ikari.transform
                .DOPath(path, 4.0f)
                .OnComplete(() =>
                {
                    Debug.Log("Complete");
                    PlayerState.PlayerScript.mode = new PlayerStateOnGround();
                });



                
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
