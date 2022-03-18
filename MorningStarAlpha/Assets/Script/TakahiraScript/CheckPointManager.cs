using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CheckPointManager : SingletonMonoBehaviour<CheckPointManager>
{
    [Header("現在のリスポーン座標")]
    public static Vector3 RespawnPos;          // リスポーン座標
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
    }

    // チェックポイント使用処理
    public void CheckPointAction()
    {
        // ゲームシーンのリセット
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    // チェックポイントのセット
    public void SetCheckPoint(CheckPoint checkpoint)
    {
        RespawnPos = checkpoint.RespawnPointObject.transform.position;
        noTouchCheckPoint = true;
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