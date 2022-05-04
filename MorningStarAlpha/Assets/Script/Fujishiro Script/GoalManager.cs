using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class GoalManager : MonoBehaviour
{
    [SerializeField] GameObject ClearCam;
    [SerializeField] GameObject MainCam;
    [SerializeField] Volume PostProssece;
    [SerializeField] RawImage RawImage;
    [SerializeField] GameObject Canvas;

    [SerializeField][Range(1.0f, 100.0f)] float AlphaSpeed; // 透明度の設定
    [SerializeField][Range(0.0f, 1.0f)] float CameraRotateSpeed;
    [SerializeField] float alpha_Flag;
    [SerializeField] int parsent = 1000000;

    private int counter_1; // 調整カウント用
    [SerializeField][ReadOnly] private float alpha;

    private bool AngleChange;

    void Awake()
    {
        //DontDestroyOnLoad(Canvas);
        //DontDestroyOnLoad(ClearCam);
    }


    // Start is called before the first frame update
    void Start()
    {
        // クリアカメラ非アクティブ
        ClearCam.SetActive(false);

        // 変数初期化
        AngleChange = false;

        counter_1 = 0;
        alpha = 0;

        
        RawImage.color = new Color(1, 1, 1, 0);
    }

    void FixedUpdate()
    {
        if (AngleChange == true)
        {
            MainCam.transform.DORotate(new Vector3(-78.0f, 0, 0), 0.08f, RotateMode.Fast)
                .SetLink(MainCam)
                .SetEase(Ease.InCirc);
            //MainCam.transform.Rotate(new Vector3(-CameraRotateSpeed, 0, 0));
            counter_1++;
        }

        if (counter_1 >= 4)
        {
            alpha += AlphaSpeed / parsent;
            RawImage.color += new Color(0, 0, 0, alpha);
            //Debug.Log(RawImage.color.a);
        }

        // リザルト画面への遷移判定
        if (alpha > alpha_Flag)
        {
            SceneManager.LoadScene("ResultScene");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AngleChange = true;
            if (PostProssece == null) Debug.Log("volume is not loading");

            PostProssece.profile.TryGet<MotionBlur>(out var motionBlur);
            //PostProssece.GetComponent<Volume>().TryGetComponent<MotionBlur>(out var motionBlur);
            //PostProssece.TryGetComponent<MotionBlur>(out var motionBlur);
            motionBlur.active = true;
            if (!motionBlur.active) Debug.Log("motionBlur is false");

            ClearCam.SetActive(true);
            if (!ClearCam.activeSelf) Debug.Log("ClearCam is not Actived");

        }

        Debug.Log("Goal当たった");
    }
}
