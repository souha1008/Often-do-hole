using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private  GameObject Player;         // [SerializeField] private属性だけどinspector上で設定できるようにする
    [SerializeField] private  float CAMERA_DISTANCE;      //カメラとプレイヤーの距離

    private void Start()
    {
        TracePlayer();
    }

    private void LateUpdate()
    {
        TracePlayer();
    }

    //プレイヤーをカメラの中央に収め続ける
    void TracePlayer()
    {
        Vector3 tempPos = Player.GetComponent<Transform>().position;

        tempPos.z -= CAMERA_DISTANCE;
        this.GetComponent<Transform>().position = tempPos;
    }
}
