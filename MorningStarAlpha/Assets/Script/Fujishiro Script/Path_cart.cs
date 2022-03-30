using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;

public class Path_cart : MonoBehaviour 
{
    static public Raen_Path waypoint; // スクリプト情報 
    public bool RaenStart = false; // レーンスタート用フラグ
    private bool NowRaen = false; // 一回のみ実行用フラグ
    [SerializeField] GameObject ikari; // 動かすオブジェクト指定

    void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {
        // フラグがオンなら自動でレーンに乗らせる
        if (RaenStart)
        {


            // パス情報を元に、○秒で移動する
            ikari.transform.DOPath(waypoint.Waypoint, 10f);

            // フラグ設定
            RaenStart = false;
            NowRaen = true;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if(collision.CompareTag("Bullet") && NowRaen == false)
        {
            RaenStart = true;
        }
    }

}