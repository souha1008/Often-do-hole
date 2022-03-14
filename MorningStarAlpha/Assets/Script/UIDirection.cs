using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDirection : MonoBehaviour
{
    [SerializeField] private PlayerMain Player;
    [SerializeField] private float LINE_START_DISTANCE;  //矢印の開始位置（プレイヤー中心からの長さ）
    [SerializeField] private float LINE_LENGTH;          //矢印の長さ
    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, Player.transform.position);
        lr.SetPosition(1, Player.transform.position);
    }

    private void LateUpdate()
    {
        Vector3 vec = Player.leftStick.normalized;

        if (Player.canShot)
        {
            //始点と終点を設定
            lr.SetPosition(0, Player.transform.position + (vec * LINE_START_DISTANCE));
            lr.SetPosition(1, Player.transform.position + (vec * (LINE_START_DISTANCE + LINE_LENGTH)));
        }
        else
        {
            //同じ位置に設定して消す
            lr.SetPosition(0, Player.transform.position);
            lr.SetPosition(1, Player.transform.position);
        }
       
    }
}
