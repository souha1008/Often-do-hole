using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class KujiraPos : MonoBehaviour
{
    public GameObject CenterPos;

    public RectTransform CanvasObj;

    public GameObject ImageUI;
    Camera currentCamera;
    int FlagCnt = 0;
    bool Flag = false;
    bool TempSwitch = false;
    

    // Start is called before the first frame update
    void Start()
    {
        FlagCnt = 0;
        TraceObj();
    }

    void TraceObj()
    {
        transform.position = CenterPos.transform.position;
    }



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

    // Update is called once per frame
    void Update()
    {

        TraceObj();

        // ここから座標探索

        float depth = CameraMainShimokawara.instance.CAMERA_DISTANCE;
        //2点がワールド座標
        Vector3 CameraRightTop = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, depth));
        Vector3 CameraLeftBottom = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, depth));


        //横に長い
        Vector3 KujiraSize = transform.lossyScale;
        KujiraSize.x *= 2;

        Vector3 KujiraRightTop = transform.position + (KujiraSize * 0.5f);
        Vector3 KujiraLeftBottom = transform.position - (KujiraSize * 0.5f);

        if(KujiraRightTop.x > CameraLeftBottom.x && KujiraLeftBottom.x < CameraRightTop.x &&
            KujiraRightTop.y > CameraLeftBottom.y && KujiraLeftBottom.y < CameraRightTop.y)
        {
            if(TempSwitch == false)
            {
                FlagCnt = 0;
            }
            else
            {
                FlagCnt++;
            }

            if(FlagCnt > 20)
            {
                Flag = true;
            }

            TempSwitch = true;
        }
        else
        {
            if (TempSwitch == true)
            {
                FlagCnt = 0;
            }
            else
            {
                FlagCnt++;
            }

            if (FlagCnt > 20)
            {
                Flag = false;
            }

            TempSwitch = false;
        }

        //　ターゲットポイントがひとつでもカメラに写っていたらUI非表示
        if (Flag)
        {
            ImageUI.gameObject.SetActive(false);

            KujiraCenterPos.instance.MyKujiraSpeed = KujiraSpeed.Low;

            //Debug.Log("非表示");
        }
        else
        {
            ImageUI.gameObject.SetActive(false);

            KujiraCenterPos.instance.MyKujiraSpeed = KujiraSpeed.High;
            //Debug.Log("表示");


          


            //  X

            //縦軸三点
            float Bottom = 0;
            float Top = CameraRightTop.y - CameraLeftBottom.y;
            float KujiraY = transform.position.y - CameraLeftBottom.y;

            //クジラを下から数えた割合  0.n
            float Wariai = KujiraY / Top;

            //割合から座標計算
            float CanvasHeight = CanvasObj.rect.height;
            float CanvasPosY = CanvasObj.rect.y;
            float TempObjPosY = Wariai * CanvasHeight + CanvasPosY;

            float ImageHeight = ImageUI.GetComponent<RectTransform>().transform.localScale.y * ImageUI.GetComponent<RectTransform>().rect.height;
            float PosMin = CanvasPosY + ImageHeight * 0.5f;
            float PosMax = CanvasPosY + CanvasHeight - ImageHeight * 0.5f;

            TempObjPosY = Mathf.Clamp(TempObjPosY, PosMin, PosMax);


            //float HukidashiHeight = HukidasiUI.GetComponent<RectTransform>().transform.localScale.y * HukidasiUI.GetComponent<RectTransform>().rect.height;
            float CanvasPosX = CanvasObj.rect.x;
            float TempObjPosX;
            float ImageWidth = ImageUI.GetComponent<RectTransform>().transform.localScale.x * ImageUI.GetComponent<RectTransform>().rect.width;

            TempObjPosX = CanvasPosX + ImageWidth * 0.5f;

            ImageUI.GetComponent<RectTransform>().localPosition = new Vector3(TempObjPosX,
                TempObjPosY, ImageUI.transform.localPosition.z);
        }

       // Flag = false;
    }

    //void OnWillRenderObject()
    //{
    //    Flag = false;

    //    if (currentCamera.name == "Main Camera")
    //    {
    //        Flag = true;
    //        Debug.Log("表示");
    //    }
    //    else
    //    {
    //        Debug.Log("非表示");
    //    }

    //}
}
