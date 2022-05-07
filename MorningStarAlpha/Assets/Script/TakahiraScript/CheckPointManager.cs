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

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject); // シーンが変わっても死なない
    }


    // チェックポイントのセット
    public void SetCheckPoint(CheckPoint checkpoint)
    {
        RespawnPos = checkpoint.RespawnPointObject.transform.position;
        RespawnObject = checkpoint.GetComponentInParent<MeshOnOff>().gameObject; // メッシュ切り替えの付いた親オブジェクト取得
        NowRespawnPos = GetCheckPointPos(); // 現在のリスポーン座標更新
    }

    // 現在のチェックポイントの座標ゲット
    public Vector3 GetCheckPointPos()
    {
        Debug.LogWarning(RespawnPos);
        return RespawnPos;
    }
}