using Cinemachine;
using UnityEngine;

public class VirtualCamera : CinemachineExtension
{
    //public float CAMERA_DISTANCE = -100;

    static public VirtualCamera instance;
    [SerializeField] private GameObject Obj;

    public bool isReturn = true;
    float ReturnPos = 1.0f;

    private CinemachineTrackedDolly _dolly;

    // �J�������[�N����
    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime
    )
    {
        // Aim�̒��ゾ�����������{
        if (stage != CinemachineCore.Stage.Body)
            return;

        //this.transform.position = new Vector3(transform.position.x, transform.position.y, CAMERA_DISTANCE);
        
    }

    public void ManualUpdate()
    {
        //Debug.Log(_dolly.m_PathPosition);

        if (!CameraMainShimokawara.instance.isRail)
        {
            this.gameObject.SetActive(false);
        }

        //this.transform.position = new Vector3(transform.position.x, transform.position.y, CAMERA_DISTANCE);
    }

    //�J���������̍Ō�ɌĂ�
    public void CameraReturn()
    {
        if (CameraMainShimokawara.instance.isRail)
        {
            if (isReturn)
            {
                _dolly.VirtualCamera.GetComponent<CinemachineVirtualCamera>().m_Follow = null;

                ReturnPos -= 0.001f;

                if(ReturnPos <= 0.0f)
                {
                    ReturnPos = 0.0f;
                    _dolly.VirtualCamera.GetComponent<CinemachineVirtualCamera>().m_Follow = Obj.transform;
                    isReturn = false;
                }

                _dolly.m_PathPosition = ReturnPos;
               
               // _dolly.VirtualCamera.GetComponent<CinemachineVirtualCamera>().m_Follow = null;

            }
        }
    }

    private void Start()
    {
        instance = this;

        ReturnPos = 1.0f;
        _dolly = this.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTrackedDolly>();

        if (!CameraMainShimokawara.instance.isRail)
        {
            this.gameObject.SetActive(false);
        }
    }
}