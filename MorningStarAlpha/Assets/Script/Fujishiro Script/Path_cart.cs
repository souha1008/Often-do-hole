using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class Path_cart : MonoBehaviour
{
    public static Raen_Path waypoints; // ウェイポイント格納用
    private Vector3[] path;
    private Vector3[] player_path;

   
    private bool Rall_Start = false;
    private bool Rall_Now = false;

    void Start()
    {
        Rall_Start = false;
        Rall_Now = false;
        path = waypoints.positions.Select(target => target.transform.position).ToArray();
        player_path = waypoints.player_postions.Select(target => target.transform.position).ToArray();

        
    }

    void Update()
    {
        // データ保守
        if(waypoints != null)
        {
            if(Rall_Start == true && Rall_Now == false)
            {
                PlayerState.PlayerScript.mode = new PlayerState_Rail(); // ステートをレール状態に移行
                Sequence sequence = DOTween.Sequence(); //Sequence生成


                sequence.Append(BulletMain.instance.transform.DOLocalMove(path[0], 2.0f))
                    .Append(BulletMain.instance.transform.DOLocalMove(path[1], 2.0f));

                //for (int i = 1; i < path.Length - 1; i++)
                //{
                //    sequence.Append(BulletMain.instance.transform.DOLocalMove(path[1], 2.0f));
                //}

                //sequence.Append(BulletMain.instance.transform.DOMove(path[1], 1.0f));
                //sequence.Append(BulletMain.instance.transform.DOMove(path[2], 100.0f));
                //BulletMain.instance.transform
                //.DOPath(path, 4.0f, PathType.Linear)
                //.OnUpdate(() =>
                //{

                //})
                //.OnComplete(() =>
                //{
                //    Debug.Log("Complete");
                //    PlayerState.PlayerScript.mode = new PlayerStateMidair(true, MidairState.NORMAL);
                //});

                //PlayerMain.instance.transform
                //.DOPath(player_path, 4.0f);

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
