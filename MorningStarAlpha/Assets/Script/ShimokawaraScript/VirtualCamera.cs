using Cinemachine;
using UnityEngine;

public class VirtualCamera : CinemachineExtension
{
    //public float CAMERA_DISTANCE = -100;

    // ƒJƒƒ‰ƒ[ƒNˆ—
    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime
    )
    {
        // Aim‚Ì’¼Œã‚¾‚¯ˆ—‚ğÀ{
        if (stage != CinemachineCore.Stage.Body)
            return;


        //this.transform.position = new Vector3(transform.position.x, transform.position.y, CAMERA_DISTANCE);
        
    }

    private void Update()
    {
        if (!CameraMainShimokawara.instance.isRail)
        {
            this.gameObject.SetActive(false);
        }

        //this.transform.position = new Vector3(transform.position.x, transform.position.y, CAMERA_DISTANCE);
    }

    private void Start()
    {
        if (!CameraMainShimokawara.instance.isRail)
        {
            this.gameObject.SetActive(false);
        }
    }
}