using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMainShimokawara: MonoBehaviour
{
    [SerializeField] private GameObject CenterObj /*= GameObject.Find("CameraCenterPos")*/;         // [SerializeField] private属性だけどinspector上で設定できるようにする
    [SerializeField] private float CAMERA_DISTANCE;      //カメラとプレイヤーの距離

    private void Start()
    {
        TracePlayer();
    }

    //private void Update()
    //{
    //    TracePlayer();
    //}

    //private void FixedUpdate()
    //{
    //    TracePlayer();
    //}


    private void LateUpdate()
    {
        TracePlayer();
    }

    //プレイヤーをカメラの中央に収め続ける
    void TracePlayer()
    {
        Vector3 tempPos = CenterObj.transform.position;

        tempPos.z -= CAMERA_DISTANCE;
        transform.position = tempPos;
    }
}
