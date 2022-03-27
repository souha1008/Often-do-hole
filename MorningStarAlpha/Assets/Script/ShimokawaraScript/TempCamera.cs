using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCamera : MonoBehaviour
{

    [SerializeField] private GameObject CenterObj /*= GameObject.Find("CameraCenterPos")*/;         // [SerializeField] private����������inspector��Őݒ�ł���悤�ɂ���

    // Start is called before the first frame update
    private void Start()
    {
        if (!CameraMainShimokawara.instance.isRail)
        {
            this.gameObject.SetActive(false);
        }

        TraceObj();
    }

    private void LateUpdate()
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

    //�v���C���[���J�����̒����Ɏ��ߑ�����
    void TraceObj()
    {
        Vector3 tempPos = CenterObj.transform.position;
        transform.position = tempPos;
    }
}
