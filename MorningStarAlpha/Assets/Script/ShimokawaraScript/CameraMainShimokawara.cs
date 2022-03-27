using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMainShimokawara: MonoBehaviour
{
    [Header("チェックが入っていたらレール追従")]
    [SerializeField, Tooltip("チェックが入っていたらレール追従")] public bool isRail;        //これにチェックが入っていたら分割

    [SerializeField] private GameObject CenterObj /*= GameObject.Find("CameraCenterPos")*/;         // [SerializeField] private属性だけどinspector上で設定できるようにする
    [SerializeField] public float CAMERA_DISTANCE;      //カメラとプレイヤーの距離

    public static CameraMainShimokawara instance; 

    private void Start()
    {
        instance = this;
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
        if(!isRail)
        {
            TracePlayer();
        }
    }

    //プレイヤーをカメラの中央に収め続ける
    void TracePlayer()
    {
        Vector3 tempPos = CenterObj.transform.position;

        tempPos.z -= CAMERA_DISTANCE;
        transform.position = tempPos;
    }
}
