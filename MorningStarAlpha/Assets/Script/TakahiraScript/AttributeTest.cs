using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeTest : MonoBehaviour
{
    // アトリビュートの機能テストスクリプト(使い方)

    //==================
    // 表示しない
    //==================
    [HideInInspector]
    public float NoOutput = 0.0f;

    //==================
    // 表示する
    //==================
    [SerializeField]
    private float Output = 0.0f;

    //==================
    // ラベルの変更
    //==================
    [Label("ラベル名")]
    public float Label = 0.0f;

    //==================
    // ヘッダーの追加
    //==================
    [Header("ヘッダー名")]
    public float Header = 0.0f;

    //==================
    // 空白の追加
    //==================
    [Space(50.0f)] // int, float型


    //==================
    // ヒントの表示(カーソルを合わせるとチップスがでる)
    //==================
    [Tooltip("ヒント名")]
    public float Tooltip = 0.0f;

    //==================
    // スライダーの表示
    //==================
    [Range(0.0f, 100.0f)] // int, float型
    public float Range = 0.0f;

    //==================
    // 改行追加
    //==================
    [Multiline(10)] // int型
    public string Multiline; // string型

    private void Start()
    {
        NoOutput = Output;
    }
}
