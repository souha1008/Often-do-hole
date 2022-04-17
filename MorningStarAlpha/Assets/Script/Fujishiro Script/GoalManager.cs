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
    [SerializeField] PostProcessProfile PostProssece;
    [SerializeField] Image RawImage;

    private int counter_1; // 調整カウント用
    private float alpha;

    private bool AngleChange;

    // Start is called before the first frame update
    void Start()
    {
        // カメラを非アクティブ
        ClearCam.SetActive(false);

        // 変数初期化
        AngleChange = false;

        counter_1 = 0;
        alpha = 0;

        RawImage.color = new Color(255, 255, 255, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (AngleChange == true)
        {
            MainCam.transform.Rotate(new Vector3(0.2f, 0, 0));
            counter_1++;
        }

        if (counter_1 <= 4)
        {
            alpha++;
            RawImage.color += new Color(1, 1, 1, alpha);
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
            PostProssece.AddSettings<MotionBlur>().enabled.Override(true);
        }

        Debug.Log("Goal当たった");
    }
}
