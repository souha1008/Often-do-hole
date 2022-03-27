using Cinemachine;
using UnityEngine;

public class VirtualCamera : CinemachineExtension
{
    //public float CAMERA_DISTANCE = -100;

    static public VirtualCamera instance;

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
        if (!CameraMainShimokawara.instance.isRail)
        {
            this.gameObject.SetActive(false);
        }

        //this.transform.position = new Vector3(transform.position.x, transform.position.y, CAMERA_DISTANCE);
    }

    private void Start()
    {
        instance = this;


        if (!CameraMainShimokawara.instance.isRail)
        {
            this.gameObject.SetActive(false);
        }
    }
}