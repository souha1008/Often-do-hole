using Cinemachine;
using UnityEngine;

/// <summary>
/// マウスホイールによるズームを行えるようにする拡張
/// </summary>
public class VirtualCamera : CinemachineExtension
{
    public float CAMERA_DISTANCE = -100;

    // カメラワーク処理
    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime
    )
    {
        // Aimの直後だけ処理を実施
        if (stage != CinemachineCore.Stage.Body)
            return;


        this.transform.position = new Vector3(transform.position.x, transform.position.y, CAMERA_DISTANCE);
        
    }

    private void Update()
    {
        //this.transform.position = new Vector3(transform.position.x, transform.position.y, CAMERA_DISTANCE);
    }
}