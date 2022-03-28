using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMainShimokawara: MonoBehaviour
{
    [Header("チェックが入っていたらレール追従")]
    [SerializeField, Tooltip("チェックが入っていたらレール追従")] public bool isRail;        //これにチェックが入っていたら分割

    [SerializeField] private GameObject XObj /*= GameObject.Find("CameraCenterPos")*/;         // [SerializeField] private属性だけどinspector上で設定できるようにする
    [SerializeField] private GameObject YObj /*= GameObject.Find("CameraCenterPos")*/;         // [SerializeField] private属性だけどinspector上で設定できるようにする
    [SerializeField] public float CAMERA_DISTANCE;      //カメラとプレイヤーの距離

    public static CameraMainShimokawara instance; 

    private void Start()
    {
        instance = this;
        TraceObj();
    }

    public void ManualUpdate()
    {
        TraceObj();
    }

    //private void FixedUpdate()
    //{
    //    TracePlayer();
    //}


    //private void LateUpdate()
    //{
    //    TraceObj();
    //}

    //プレイヤーをカメラの中央に収め続ける
    void TraceObj()
    {
        if(isRail)
        {
            Vector3 tempPos = Vector3.zero;

            tempPos.x = XObj.transform.position.x;
            tempPos.y = YObj.transform.position.y;
            tempPos.z -= CAMERA_DISTANCE;

            transform.position = tempPos;
        }
        else
        {
            Vector3 tempPos = XObj.transform.position;

            tempPos.z -= CAMERA_DISTANCE;
            transform.position = tempPos;
        }
    }
}
