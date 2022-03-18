using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPointManager : SingletonMonoBehaviour<CheckPointManager>
{
    [Header("プレイヤーオブジェクト(ない場合は\"Player\"を探す)")]
    public GameObject PlayerObject;     // プレイヤーオブジェクト
    [Header("現在のリスポーン座標")]
    public Vector3 RespawnPos;          // リスポーン座標

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this.gameObject); // シーンが変わっても死なない
    }

    // チェックポイント使用処理
    public void CheckPointAction()
    {
        // ゲームシーンのリセット
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // プレイヤーの位置を変更
        if (PlayerObject == null)
        {
            PlayerObject = GameObject.Find("Player");
        }
        PlayerObject.GetComponent<Transform>().position = GetCheckPointPos();
        PlayerObject.GetComponent<PlayerMain>().vel = Vector3.zero;
        //Debug.Log("リスポーン座標：" + GetCheckPointPos());
    }


    // チェックポイントのセット
    public void SetCheckPoint(CheckPoint checkpoint)
    {
        RespawnPos = checkpoint.RespawnPointObject.transform.position;
    }

    // 現在のチェックポイントの座標ゲット
    public Vector3 GetCheckPointPos()
    {
        return RespawnPos;
    }
}