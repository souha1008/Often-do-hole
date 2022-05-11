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

    public static Vector3 RespawnPos = Vector3.zero;   // リスポーン座標
    public bool RespawnFlag = false;   // チェックポイントセット確認フラグ

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject); // シーンが変わっても死なない

        RespawnFlag = false;
    }


    // チェックポイントのセット
    public void SetCheckPoint(CheckPoint checkpoint)
    {
        if (RespawnPos != checkpoint.RespawnPointObject.transform.position)
        {
            RespawnPos = checkpoint.RespawnPointObject.transform.position;
            RespawnObject = checkpoint.GetComponentInParent<MeshOnOff>().gameObject; // メッシュ切り替えの付いた親オブジェクト取得
            NowRespawnPos = GetCheckPointPos(); // 現在のリスポーン座標更新

            CoinManager.Instance.SetCheckPointCoinData(); // コインの情報を入力
        }
        RespawnFlag = true;
    }

    // 現在のチェックポイントの座標ゲット
    public Vector3 GetCheckPointPos()
    {
        //Debug.LogWarning(RespawnPos);
        return RespawnPos;
    }

    // チェックポイントリセット
    public void ResetCheckPoint()
    {
        RespawnPos = Vector3.zero;
        NowRespawnPos = GetCheckPointPos();
        RespawnObject = null;
        RespawnFlag = false;
    }
}