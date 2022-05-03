using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Title_p2 : MonoBehaviour
{
    [Tooltip("Playerオブジェクト")] [SerializeField] GameObject Player;
    [Tooltip("MainCamera")][SerializeField] GameObject mainCamera;
    [Tooltip("CameraCenterX")][SerializeField] GameObject centerX;
    [Tooltip("Canvas")][SerializeField] GameObject Canvas;
    [Tooltip("足場")][SerializeField] GameObject Plane_Parent;

    [System.NonSerialized] public static Title_p2 instance;

    [Tooltip("カメラトランジション時間")][SerializeField] float cam_transition = 2.0f;
    [Tooltip("プレイヤーローテーション時間")][SerializeField] float player_Rotation = 2.0f;

    private Vector3 pos;

    private bool once_press;

    public bool changeScene;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
        pos = Player.transform.position;
        once_press = false;
        Plane_Parent.SetActive(false);
        PlayerMain.instance.mode = new PlayerState_Title();

        // 変数初期化
        changeScene = false;
    }

    // Update is called once per frame
    void Update()
    {
        Player.transform.position = pos;

        // PressAnyButton
        if (Input.GetButton("Fire1") || Input.GetButton("Fire2")
            || Input.GetButton("Fire3") || Input.GetButton("Jump") && once_press == false)
        { 
            Canvas.SetActive(false);
            once_press = true;
            Plane_Parent.SetActive(true);
        }

        if (once_press == true)
        {
            Player.transform.DORotate(new Vector3(0, 90, 0), player_Rotation)
                .SetRecyclable(false)
                .SetLink(Player.gameObject)
                .SetEase(Ease.OutCubic);

            // メインカメラのポジション移動
            Vector3 camPos = centerX.transform.position;
            camPos.z = -50.0f;

            mainCamera.transform.DORotate(new Vector3(0, 0, 0), 4.0f, RotateMode.Fast);
            
            mainCamera.transform.DOMove(camPos, cam_transition)
                .SetLink(mainCamera.gameObject)
                .SetRecyclable(false)
                .SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    changeScene = true;
                });

            // シーンチェンジ
            if(changeScene == true)
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }
}
