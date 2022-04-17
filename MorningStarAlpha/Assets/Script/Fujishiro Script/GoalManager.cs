using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class GoalManager : MonoBehaviour
{
    [SerializeField] GameObject ClearCam;
    [SerializeField] GameObject MainCam;
    [SerializeField] GameObject PostProssece;
    [SerializeField] RawImage RawImage;

    private int counter_1; // 調整カウント用
    private float alpha;

    private bool AngleChange;

    // Start is called before the first frame update
    void Start()
    {
        // クリアカメラ非アクティブ
        ClearCam.SetActive(false);

        // 変数初期化
        AngleChange = false;

        counter_1 = 0;
        alpha = 0;

        RawImage.color = new Color(255, 255, 255, 0);
    }

    void FixedUpdate()
    {

        if (AngleChange == true)
        {
            MainCam.transform.Rotate(new Vector3(-0.2f, 0, 0));
            counter_1++;
        }

        if (counter_1 >= 4)
        {
            alpha += 0.00002f;
            RawImage.color += new Color(0, 0, 0, alpha);
        }

        // リザルト画面への遷移判定
        if (alpha >= 255)
        {
            SceneManager.LoadScene("ResultScene");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AngleChange = true;
            //PostProcessVolume ppv = PostProssece.GetComponent<PostProcessVolume>();
            //if (ppv == null) Debug.Log("ppv is not loading");

            //PostProcessProfile ppp = ppv.profile;
            //if (ppp == null) Debug.Log("ppp is not loading");

            //ppp.AddSettings<MotionBlur>().enabled.Override(true);

            ClearCam.SetActive(true);
            if (!ClearCam.activeSelf) Debug.Log("ClearCam is not Actived");

        }

        Debug.Log("Goal当たった");
    }
}
