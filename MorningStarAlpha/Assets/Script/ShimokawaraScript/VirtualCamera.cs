using Cinemachine;
using UnityEngine;

/// <summary>
/// �}�E�X�z�C�[���ɂ��Y�[�����s����悤�ɂ���g��
/// </summary>
public class VirtualCamera : CinemachineExtension
{
    public float CAMERA_DISTANCE = -100;

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


        this.transform.position = new Vector3(transform.position.x, transform.position.y, CAMERA_DISTANCE);
        
    }

    private void Update()
    {
        //this.transform.position = new Vector3(transform.position.x, transform.position.y, CAMERA_DISTANCE);
    }
}