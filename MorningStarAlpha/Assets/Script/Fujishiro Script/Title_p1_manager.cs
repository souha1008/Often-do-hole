using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title_p1_manager : MonoBehaviour
{
    [Tooltip("メインカメラ")][SerializeField] Camera camera;
    [Tooltip("Playerオブジェクト")][SerializeField] GameObject Player;

    [Tooltip("カメラが動いているフレーム")][SerializeField] int camera_motion = 30;

    private bool transition;    // カメラのモーショントリガー
    Vector3 pos;                // プレイヤー位置補正用
    int flame_count;

    void Awake()
    {
        pos = Player.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        transition = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        // PlayerのZ軸補正
        Player.transform.position = pos;

        if(Input.GetButton("Fire1") || Input.GetButton("Fire2")
            || Input.GetButton("Fire3") || Input.GetButton("Jump"))
        {
            transition = true;
        }

        if(transition == true)
        {
            flame_count++;
            camera.transform.Rotate(new Vector3(0, 5.4f, 0));
            if(flame_count >= camera_motion)
            {
                transition = false;
            }
        }
    }
}
