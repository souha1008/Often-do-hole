using UnityEngine;

public class MeshOnOff : MonoBehaviour
{
    // 変数
    [Header("ゲーム再生中のチェックポイントの表示")]
    [Label("表示する")]
    public bool MeshOn = false;

    [Header("初期リスポーン地点")]
    [Label("初期リスポーン")]
    public bool InitCheckPoint = false;
}