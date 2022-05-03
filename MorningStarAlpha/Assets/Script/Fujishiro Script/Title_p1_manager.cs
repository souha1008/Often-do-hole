using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Title_p1_manager : MonoBehaviour
{
    [Tooltip("メインカメラ")][SerializeField] Camera camera;
    [Tooltip("Playerオブジェクト")][SerializeField] GameObject Player;

    [Tooltip("カメラが動いているフレーム")][SerializeField] int camera_motion = 30;
    [Tooltip("カメラが動く速度")][Range(0.0f, 10.0f)] [SerializeField] float camera_speed = 5.4f;
    [Tooltip("カメラリセット角度")][SerializeField] float camera_resetRot = 281.0f;
    [Tooltip("カメラリセット位置")][SerializeField] Vector3 camera_RestPos = new Vector3(3.09f, -1, -9.29f);
    [Tooltip("旋回時間")][SerializeField] float duration = 10.0f;
    [Tooltip("旋回終了から何フレーム経ったか")][SerializeField] int derayFlame = 120;

    private bool transition_1;    // 一番目カメラのモーショントリガー
    private bool transition_2;    // 二番目カメラモーショントリガー
    private Vector3 pos;                // プレイヤー位置補正用
    private int flame_count;            // フレームカウント用

    private bool once_press;
    private bool sceneCange;    //シーンチェンジ変数＆シングルトン解決用



    void Awake()
    {
        // DOTweenキャパシティ警告回避用
        DOTween.SetTweensCapacity(tweenersCapacity: 800, sequencesCapacity: 200);

        // Playerポジション固定用
        pos = Player.transform.position;

        // 変数初期化
        flame_count = 0;
        transition_1 = false;
        transition_2 = false;
        once_press = false;
        sceneCange = false;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // PlayerのZ軸補正
        if (transition_1 == false && transition_2 == false)
        {
            Player.transform.position = pos;
        }

        // PressAnyButton
        if(Input.GetButton("Fire1") || Input.GetButton("Fire2")
            || Input.GetButton("Fire3") || Input.GetButton("Jump") && once_press == false)
        {
            transition_1 = true;
            once_press = true;
        }

        // カメラ回転処理
        if(transition_1 == true)
        {
            flame_count++;
            Debug.Log(flame_count);

            camera.transform.Rotate(new Vector3(0, camera_speed, 0));

            // 一定フレーム到達後カメラアニメーション終了
            if(flame_count >= camera_motion)
            {
                CameraReset();

                Player.SetActive(false);

                transition_1 = false;
                transition_2 = true;
                flame_count = 0;
            }
        }

        if(transition_2 == true)
        {
            camera.transform.DORotate(new Vector3(0, -1, 0), duration, RotateMode.Fast)
                .SetLink(camera.gameObject)
                .SetEase(Ease.OutCirc);

            flame_count++;
            if(flame_count >= derayFlame)
            {
                sceneCange = true;
                DOTween.Kill(camera);
            }

            if(sceneCange == true)
            {
                Debug.Log("シーン切り替え");
                SceneManager.LoadScene("Title_part2");
            }
        }
    }

    void CameraReset()
    {
        camera.transform.rotation = new Quaternion(0, camera_resetRot, 0, 0);
        camera.transform.position = camera_RestPos;
    }
}
