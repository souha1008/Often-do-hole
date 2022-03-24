using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeDisplay : MonoBehaviour
{
    [SerializeField] private PlayerMain Player;
    [SerializeField] private BulletMain Bullet;
    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, Player.transform.position);
        lr.SetPosition(1, Player.transform.position);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Player.refState == EnumPlayerState.SHOT || Player.refState == EnumPlayerState.SWING)
        {
            //始点をプレイヤーに、終点を弾にする
            lr.SetPosition(0, Player.transform.position);
            lr.SetPosition(1, Bullet.transform.position);
        }
        else
        {
            //始点と終点を同じにして消す
            lr.SetPosition(0, Player.transform.position);
            lr.SetPosition(1, Player.transform.position);
        }
    }
}