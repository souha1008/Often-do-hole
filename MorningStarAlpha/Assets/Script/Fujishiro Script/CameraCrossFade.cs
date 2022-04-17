using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraCrossFade : MonoBehaviour
{
    [SerializeField] private Camera ClearCam; // �؂�ւ���J����

    [SerializeField] private RawImage FadeImage; // �N���X�t�F�[�h���邽�߂�RawImage

    [SerializeField] private float fadeDuration = 1; // �t�F�[�h����

    private RenderTexture renderTexture;
    private Coroutine fadeCoroutine;

    private bool IsChange = false; // �t�F�[�h�����H

    public bool IsFadeStart;

    void Awake()
    {

        // �N���X�t�F�[�h�p��RenderTexture�쐬
        renderTexture = new RenderTexture(Screen.width, Screen.height, 0);

        // RawImage������
        FadeImage.texture = renderTexture;
        FadeImage.gameObject.SetActive(false);

        // bool������
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
        // �t�F�[�h�p��RawImage�\��
        FadeImage.gameObject.SetActive(true);

        var nextCamera = ClearCam;
        nextCamera.targetTexture = renderTexture;

        // RawImage���t�F�[�h
        var startTime = Time.time;

        while(true)
        {
            var time = Time.time - startTime;
            if (time > fadeDuration) break;

            var alpha = time / fadeDuration;
            FadeImage.color = new Color(1, 1, 1, alpha);

            yield return null;
        }

        // �؂�ւ���J������L����
        nextCamera.targetTexture = null;
        nextCamera.enabled = true;

        // RawImage��\��
        FadeImage.gameObject.SetActive(false);

        fadeCoroutine = null;
    }
}
