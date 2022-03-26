using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CheckPointManager : SingletonMonoBehaviour<CheckPointManager>
{
    [Label("現在のリスポーンオブジェクト")]
    public GameObject RespawnObject = null;
    [Label("現在のリスポーン座標")]    
    public Vector3 NowRespawnPos;       // 現在のリスポーン座標(見るだけ)

    public static Vector3 RespawnPos;   // リスポーン座標
    public static bool noTouchCheckPoint;
    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject); // シーンが変わっても死なない
        RespawnPos = Vector3.zero;
        noTouchCheckPoint = false;

        // 初期リスポーンセット
        if (RespawnObject != null)
        {
            SetCheckPoint(RespawnObject.GetComponentInChildren<CheckPoint>());
        }
    }


    // チェックポイントのセット
    public void SetCheckPoint(CheckPoint checkpoint)
    {
        RespawnPos = checkpoint.RespawnPointObject.transform.position;
        noTouchCheckPoint = true;
        RespawnObject = checkpoint.GetComponentInParent<MeshOnOff>().gameObject; // メッシュ切り替えの付いた親オブジェクト取得
        NowRespawnPos = GetCheckPointPos(); // 現在のリスポーン座標更新
    }

    // 現在のチェックポイントの座標ゲット
    public static Vector3 GetCheckPointPos()
    {
        return RespawnPos;
    }

    // 現在のチェックポイントの座標ゲット
    public static bool isTouchCheckPos()
    {
        return noTouchCheckPoint;
    }
}