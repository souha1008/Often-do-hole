using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDirection : MonoBehaviour
{
    [SerializeField] private PlayerMain PlayerScript;
    [SerializeField] private float LINE_START_DISTANCE;  //矢印の開始位置（プレイヤー中心からの長さ）
    [SerializeField] private float LINE_LENGTH;          //矢印の長さ

    Color activeScol, activeEcol;
    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, PlayerScript.transform.position);
        lr.SetPosition(1, PlayerScript.transform.position);

        activeScol = lr.startColor;
        activeEcol = lr.endColor;
    }

    private void LateUpdate()
    {
        Vector3 vec = PlayerScript.adjustLeftStick.normalized;


        //打てる可能性があるステートなら表示
        if (PlayerScript.refState == EnumPlayerState.ON_GROUND || PlayerScript.refState == EnumPlayerState.MIDAIR)
        {
            //クールタイム回復したら色つける
            if (PlayerScript.canShotState)
            {
                lr.startColor = activeScol;
                lr.endColor = activeEcol;
            }
            else
            {
                lr.startColor = Color.gray;
                lr.endColor = Color.gray;
            }

            if (PlayerScript.stickCanShotRange)
            {
                //始点と終点を設定
                lr.SetPosition(0, PlayerScript.transform.position + (vec * LINE_START_DISTANCE));
                lr.SetPosition(1, PlayerScript.transform.position + (vec * (LINE_START_DISTANCE + LINE_LENGTH)));
            }
            else
            {
                //同じ位置に設定して消す
                lr.SetPosition(0, PlayerScript.transform.position);
                lr.SetPosition(1, PlayerScript.transform.position);
            }
        }
        else
        {
            //同じ位置に設定して消す
            lr.SetPosition(0, PlayerScript.transform.position);
            lr.SetPosition(1, PlayerScript.transform.position);
        }



    }
}
