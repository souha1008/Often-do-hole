using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManeger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //�J�����߂�
        //VirtualCamera.instance.CameraReturn();

        CameraCenterY.instance.ManualUpdate();
        //VirtualCamera.instance.ManualUpdate();
        //TempCamera.instance.ManualUpdate();
        CameraCenterX.instance.ManualUpdate();
        CameraMainShimokawara.instance.ManualUpdate();

        CameraMainShimokawara.instance.CameraReturn();
    }
}
