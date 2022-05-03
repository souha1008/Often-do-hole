using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class Path_cart : MonoBehaviour
{
    public static Raen_Path waypoints; // ウェイポイント格納用
    public Vector3[] path;
    public Vector3[] player_path;
    public GameObject ikari;    // イカリオブジェクト格納
    public GameObject player;
    public Sequence sequence;

    public bool Rall_Start = false;
    private bool Rall_Now = false;
    int pointNo = 0;

    void Start()
    {
        Rall_Start = false;
        Rall_Now = false;
        path = waypoints.positions.Select(target => target.transform.position).ToArray();
        player_path = waypoints.player_postions.Select(target => target.transform.position).ToArray();
    }

    void Update()
    {
        // データ保守用
        if(waypoints != null)
        {
            if(Rall_Start == true && Rall_Now == false)
            {
                PlayerState.PlayerScript.mode = new PlayerState_Rail(); // ステートをレール状態に移行
                ikari.transform.Rotate(new Vector3(0, 0, 180));
                ikari.transform
                .DOPath(path, 4.0f, PathType.Linear)
                .SetLookAt(0.001f, Vector3.left)
                .OnUpdate(() =>
                {
                    ikari.transform.Rotate(new Vector3(0, 0, 180));
                })
                .OnComplete(() =>
                {
                    Debug.Log("Complete");
                    PlayerState.PlayerScript.mode = new PlayerStateMidair(true, MidairState.NORMAL);
                });
                player.transform
                .DOPath(player_path, 4.0f);



                
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
