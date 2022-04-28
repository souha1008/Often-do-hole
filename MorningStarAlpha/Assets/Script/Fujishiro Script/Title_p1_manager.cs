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

    private bool transition_1;    // 一番目カメラのモーショントリガー
    private bool transition_2;    // 二番目カメラモーショントリガー
    private Vector3 pos;                // プレイヤー位置補正用
    private int flame_count;            // フレームカウント用

    private bool once_press;

    private Sequence sequence;


    void Awake()
    {
        pos = Player.transform.position;
        flame_count = 0;
        once_press = false;
        transition_1 = false;
        transition_2 = false;
    }

    // Start is called before the first frame update
    void Start()
    {

        sequence = DOTween.Sequence();
    }

    // Update is called once per frame
    void FixedUpdate()
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

        if(transition_1 == true)
        {
            flame_count++;
            Debug.Log(flame_count);
            //camera.transform.DORotate(new Vector3(0, 359, 0), camera_motion, RotateMode.Fast);

            camera.transform.Rotate(new Vector3(0, camera_speed, 0));

            // 一定フレーム到達後カメラアニメーション終了
            if(flame_count >= camera_motion)
            {
                CameraReset();

                // プレイヤー移動
                //Player.transform.position = new Vector3(-70.0f, 5.8f, 88.5f);
                //Player.transform.SetPositionAndRotation(new Vector3(-70.0f, 5.8f, 88.5f), 
                //                                        new Quaternion(0, 13.496f, 0, 0));

                //Vector3 scale;
                //scale = Player.transform.localScale;
                //scale = new Vector3(35.1997f, 35.1997f, 35.1997f);
                //Player.transform.localScale = scale;

                //Player.transform.rotation = ;

                Player.SetActive(false);

                transition_1 = false;
                transition_2 = true;
            }
        }

        if(transition_2 == true)
        {

            //sequence = DOTween.Sequence().Append(camera.transform.DORotate(new Vector3(0, -1, 0), 1.0f, RotateMode.Fast)
            //    .SetEase(Ease.OutCirc)
            //    .OnComplete(() =>
            //    {
            //        Debug.Log("シーン切り替え");
            //        SceneManager.LoadScene("Title_part2");
            //    }));

            camera.transform.DORotate(new Vector3(0, -1, 0), 10.0f, RotateMode.Fast)
                .SetEase(Ease.OutCirc)
                .OnComplete(() =>
                {

                    Debug.Log("シーン切り替え");
                    SceneManager.LoadScene("Title_part2");
                    
                });
            //camera.transform.Rotate(new Vector3(0, camera_speed, 0));
            //if (camera.transform.rotation.y >= 360 || camera.transform.rotation.y <= 0)
            //{
            //    SceneManager.LoadScene("Title_part2");
            //    transition_2 = false;
            //}
        }
    }

    void CameraReset()
    {
        camera.transform.rotation = new Quaternion(0, 281, 0, 0);
    }
}
