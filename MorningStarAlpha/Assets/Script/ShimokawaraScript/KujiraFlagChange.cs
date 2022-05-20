using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class KujiraFlagChange : MonoBehaviour
{
    Camera currentCamera;

    void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
    }

    void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
    }

    void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        currentCamera = camera;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnWillRenderObject()
    {
        Debug.Log("‚Ç‚Á‚©‚É‰f‚Á‚½");
        KujiraLoupeScript.instans.FlagChange(false);

        if (currentCamera.name == "Main Camera")
        {
            KujiraLoupeScript.instans.FlagChange(true);

            Debug.Log("main‚É‰f‚Á‚½");
        }
    }
}
