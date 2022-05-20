using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class KujiraLoupeScript : MonoBehaviour
{
    public static KujiraLoupeScript instans;

    float SPRIT_SIRCL = 0.72015466f;//�����o���̉摜���̉~�̊�������

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

    void Start()
    {
        instans = this;
    }

    // Update is called once per frame
    void Update()
    {
        //�t���O�ύX
        float depth1 = CameraMainShimokawara.instance.CAMERA_DISTANCE;
        //2�_�����[���h���W
        Vector3 leftBottom1 = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, depth1));

        float KujiraRight = transform.position.x + transform.lossyScale.x / 2 * 2;

        if(KujiraRight > leftBottom1.x)
        {
            Flag = true;
        }
        else
        {
            Flag = false;
        }

        //�@�^�[�Q�b�g�|�C���g���ЂƂł��J�����Ɏʂ��Ă�����UI��\��
        if (Flag)
        {
            ImageUI.gameObject.SetActive(false);
            HukidasiUI.gameObject.SetActive(false);
            //Debug.Log("��\��");
        }
        else
        {
            ImageUI.gameObject.SetActive(true);
            HukidasiUI.gameObject.SetActive(true);
            //Debug.Log("�\��");


            // ����������W�T��

            float depth = CameraMainShimokawara.instance.CAMERA_DISTANCE;
            //2�_�����[���h���W
            Vector3 rightTop = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, depth));
            Vector3 leftBottom = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, depth));

            //  X

            //�c���O�_
            float Under = 0;
            float Top = rightTop.y - leftBottom.y;
            float PlayerY = this.transform.position.y - leftBottom.y;

            //�v���C���[�������琔��������  0.n
            float Wariai = PlayerY / Top;

            //����������W�v�Z
            float CanvasHeight = CanvasObj.rect.height;
            float CanvasPosY = CanvasObj.rect.y;
            float TempObjPosY = Wariai * CanvasHeight + CanvasPosY;

            //�����̏ォ�牺�ɍ��킹��
            TempObjPosY = Mathf.Clamp(TempObjPosY, 
                CanvasPosY + HukidasiUI.GetComponent<RectTransform>().transform.localScale.x * HukidasiUI.GetComponent<RectTransform>().rect.width * 0.5f,
                CanvasPosY + CanvasHeight - HukidasiUI.GetComponent<RectTransform>().transform.localScale.x * HukidasiUI.GetComponent<RectTransform>().rect.width * 0.5f);

            //  Y

            //�ォ���ŕ���
            float HukidashiWidth = HukidasiUI.GetComponent<RectTransform>().transform.localScale.y * HukidasiUI.GetComponent<RectTransform>().rect.height;

            float CanvasWidth = CanvasObj.rect.width;
            float CanvasPosX = CanvasObj.rect.x;
            float TempObjPosX;

            TempObjPosX = CanvasPosX + HukidashiWidth * 0.5f;

            HukidasiUI.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 90);
            HukidasiUI.GetComponent<RectTransform>().localPosition = new Vector3(TempObjPosX, TempObjPosY, HukidasiUI.transform.localPosition.z);
            ImageUI.GetComponent<RectTransform>().localPosition = new Vector3(TempObjPosX + HukidashiWidth * (1 - SPRIT_SIRCL) * 0.5f,
                TempObjPosY , ImageUI.transform.localPosition.z);


#if false
            //��
            if (PlayerY > CenterY)
            {
                TempObjPosY = CanvasPosY + CanvasHeight - HukidashiHeight * 0.5f;

                HukidasiUI.GetComponent<RectTransform>().localPosition = new Vector3(TempObjPosX, TempObjPosY, HukidasiUI.transform.localPosition.z);
                ImageUI.GetComponent<RectTransform>().localPosition = new Vector3(TempObjPosX,
                    TempObjPosY - HukidashiHeight * (1 - SPRIT_SIRCL) * 0.5f, ImageUI.transform.localPosition.z);

                HukidasiUI.GetComponent<RectTransform>().rotation = new Quaternion(0, 0, 0, 0);
            }
            //��
            else
            {
                TempObjPosY = CanvasPosY + HukidashiHeight * 0.5f;

                HukidasiUI.GetComponent<RectTransform>().localPosition = new Vector3(TempObjPosX, TempObjPosY, HukidasiUI.transform.localPosition.z);
                ImageUI.GetComponent<RectTransform>().localPosition = new Vector3(TempObjPosX,
                     TempObjPosY + HukidashiHeight * (1 - SPRIT_SIRCL) * 0.5f, ImageUI.transform.localPosition.z);

                HukidasiUI.GetComponent<RectTransform>().rotation = new Quaternion(0, 0, 180, 0);
            }
#endif
        }
    }

    //void OnWillRenderObject()
    //{
    //    Flag = false;

    //    if (currentCamera.name == "Main Camera")
    //        Flag = true;
    //}

    public void FlagChange(bool flag)
    {
        Flag = flag;
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
