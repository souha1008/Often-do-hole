using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraCrossFade : MonoBehaviour
{
    [SerializeField] private Camera ClearCam; // 切り替えるカメラ

    [SerializeField] private RawImage FadeImage; // クロスフェードするためのRawImage

    [SerializeField] private float fadeDuration = 1; // フェード時間

    private RenderTexture renderTexture;
    private Coroutine fadeCoroutine;

    private bool IsChange = false; // フェード中か？

    public bool IsFadeStart;

    void Awake()
    {

        // クロスフェード用のRenderTexture作成
        renderTexture = new RenderTexture(Screen.width, Screen.height, 0);

        // RawImage初期化
        FadeImage.texture = renderTexture;
        FadeImage.gameObject.SetActive(false);

        // bool初期化
        IsFadeStart = false;

    }

    void Start()
    {
        Goal_kari.ccf = this;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsFadeStart)
        { 
            ChangeCamera();
            IsFadeStart=false;
        }
    }

    public void ChangeCamera()
    {
        fadeCoroutine = StartCoroutine("CrossFadeCorutine");
    }

    private IEnumerator CrossFadeCoroutine()
    {
        // フェード用のRawImage表示
        FadeImage.gameObject.SetActive(true);

        var nextCamera = ClearCam;
        nextCamera.targetTexture = renderTexture;

        // RawImageをフェード
        var startTime = Time.time;

        while(true)
        {
            var time = Time.time - startTime;
            if (time > fadeDuration) break;

            var alpha = time / fadeDuration;
            FadeImage.color = new Color(1, 1, 1, alpha);

            yield return null;
        }

        // 切り替え後カメラを有効化
        nextCamera.targetTexture = null;
        nextCamera.enabled = true;

        // RawImage非表示
        FadeImage.gameObject.SetActive(false);

        fadeCoroutine = null;
    }
}
