using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class LoupeScript : MonoBehaviour
{
    float SPRIT_SIRCL = 0.72015466f;//吹き出しの画像内の円の割合高さ

    public RectTransform CanvasObj;

    public GameObject ImageUI;
    public GameObject HukidasiUI;

    bool Flag = false;


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

    // Update is called once per frame
    void Update()
    {

        //　ターゲットポイントがひとつでもカメラに写っていたらUI非表示
        if (Flag)
        {
            ImageUI.gameObject.SetActive(false);
            HukidasiUI.gameObject.SetActive(false);
            //Debug.Log("非表示");
        }
        else
        {
            ImageUI.gameObject.SetActive(true);
            HukidasiUI.gameObject.SetActive(true);
            //Debug.Log("表示");


            // ここから座標探索

            float depth = CameraMainShimokawara.instance.CAMERA_DISTANCE;
            //2点がワールド座標
            Vector3 rightTop = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, depth));
            Vector3 leftBottom = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, depth));

            //  X

            //横軸三点
            float Left = 0;
            float Right = rightTop.x - leftBottom.x;
            float PlayerX = PlayerMain.instance.transform.position.x - leftBottom.x;

            //プレイヤーを左から数えた割合  0.n
            float Wariai = PlayerX / Right;

            //割合から座標計算
            float CanvasWidth = CanvasObj.rect.width;
            float CanvasPosX = CanvasObj.rect.x;
            float TempObjPosX = Wariai * CanvasWidth  + CanvasPosX;



            //  Y

            //上か下で分岐
            float CenterY = (rightTop.y + leftBottom.y) * 0.5f;
            float PlayerY = PlayerMain.instance.transform.position.y;

            float HukidashiHeight = HukidasiUI.GetComponent<RectTransform>().transform.localScale.y * HukidasiUI.GetComponent<RectTransform>().rect.height;

            float CanvasHeight = CanvasObj.rect.height;
            float CanvasPosY = CanvasObj.rect.y;
            float TempObjPosY;

            //上
            if (PlayerY > CenterY)
            {
                TempObjPosY = CanvasPosY + CanvasHeight - HukidashiHeight * 0.5f;

                HukidasiUI.GetComponent<RectTransform>().localPosition = new Vector3(TempObjPosX, TempObjPosY, HukidasiUI.transform.localPosition.z);
                ImageUI.GetComponent<RectTransform>().localPosition = new Vector3(TempObjPosX, 
                    TempObjPosY - HukidashiHeight * (1 - SPRIT_SIRCL) * 0.5f, ImageUI.transform.localPosition.z);

                HukidasiUI.GetComponent<RectTransform>().rotation = new Quaternion(0, 0, 0, 0);
            }
            //下
            else
            {
                TempObjPosY = CanvasPosY + HukidashiHeight * 0.5f;

                HukidasiUI.GetComponent<RectTransform>().localPosition = new Vector3(TempObjPosX, TempObjPosY, HukidasiUI.transform.localPosition.z);
                ImageUI.GetComponent<RectTransform>().localPosition = new Vector3(TempObjPosX,
                     TempObjPosY + HukidashiHeight * (1 - SPRIT_SIRCL) * 0.5f, ImageUI.transform.localPosition.z);

                HukidasiUI.GetComponent<RectTransform>().rotation = new Quaternion(0, 0, 180, 0);
            }           
        }       
    }

    void OnWillRenderObject()
    {
        Flag = false;

        if (currentCamera.name == "Main Camera")
            Flag = true;
    }

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (Camera.current.name == "Main Camera")
    //    {

    //    }
    //}
}
