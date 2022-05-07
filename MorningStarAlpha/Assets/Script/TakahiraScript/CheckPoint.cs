using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    // 変数
    [Label("復活座標オブジェクト")]
    public GameObject RespawnPointObject;                       // 復活座標オブジェクト
    [Label("メッシュ切り替え用スクリプトがついたオブジェクト")]
    [SerializeField] private GameObject MeshOnOffObject;        // メッシュの表示非表示切り替え用

    private void Awake()
    {
        // 初期リスセット
        if (MeshOnOffObject.GetComponent<MeshOnOff>().InitCheckPoint)
        {
            CheckPointManager.Instance.SetCheckPoint(this);
        }
    }


    // Start is called before the first frame update
    private void Start()
    {
        // コライダー
        this.gameObject.GetComponent<Collider>().isTrigger = true;  // トリガーオン
        RespawnPointObject.GetComponent<Collider>().isTrigger = true;     // トリガーオン

        // チェックポイントのメッシュオンオフ用スクリプトを参照して、見える or 見えなくする
        if (MeshOnOffObject.GetComponent<MeshOnOff>().MeshOn)
        {
            this.gameObject.GetComponent<MeshRenderer>().enabled = true;
            RespawnPointObject.GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
            RespawnPointObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            // チェックポイントのHitBoxに触れたらチェックポイント更新
            CheckPointManager.Instance.SetCheckPoint(this);
        }
    }
}
