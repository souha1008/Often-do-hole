using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class Raen_Path : MonoBehaviour
{
    public GameObject[] positions; // ウェイポイント配列
    public GameObject[] player_postions;

    private Vector3[] path;
    private Vector3[] player_path;


    private bool Rall_Start = false;
    private bool Rall_Now = false;

    void Start()
    {
        Rall_Start = false;
        Rall_Now = false;
    }

    void Update()
    {
      
        if (Rall_Start == true && Rall_Now == false)
        {
            positions[0] = BulletMain.instance.gameObject;
            player_postions[0] = PlayerMain.instance.gameObject;

            path = positions.Select(target => target.transform.position).ToArray();
            player_path = player_postions.Select(target => target.transform.position).ToArray();

            BulletMain.instance.transform
            .DOPath(path, 4.0f, PathType.Linear)
            .OnUpdate(() =>
            {

            })
            .OnComplete(() =>
             {
                 Debug.Log("Complete");
                 PlayerState.PlayerScript.mode = new PlayerStateMidair(true, MidairState.NORMAL);
             });

            PlayerMain.instance.transform
            .DOPath(player_path, 4.0f);

            Rall_Now = true;                                       // 一回のみ用フラグをオン
        }
        
}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Bullet") && Rall_Now == false)
        {
            Rall_Start = true;
        }
    }

}
