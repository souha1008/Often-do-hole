using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCamera : MonoBehaviour
{
    static public TempCamera instance;
    [SerializeField] private GameObject CenterObj /*= GameObject.Find("CameraCenterPos")*/;         // [SerializeField] private属性だけどinspector上で設定できるようにする

    // Start is called before the first frame update
    private void Start()
    {
        instance = this;

        if (!CameraMainShimokawara.instance.isRail)
        {
            this.gameObject.SetActive(false);
        }

        TraceObj();
    }

    public void ManualUpdate()
    {
        if (!CameraMainShimokawara.instance.isRail)
        {
            this.gameObject.SetActive(false);
        }

        if (!CameraMainShimokawara.instance.isRail)
        {
            TraceObj();
        }
    }

    //プレイヤーをカメラの中央に収め続ける
    void TraceObj()
    {
        Vector3 tempPos = CenterObj.transform.position;
        transform.position = tempPos;
    }
}