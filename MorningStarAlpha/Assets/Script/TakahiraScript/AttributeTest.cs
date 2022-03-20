using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeTest : MonoBehaviour
{
    //+++++++++++++++++++++++++++++++++++++++++++++
    // アトリビュートの機能テストスクリプト(使い方)
    //+++++++++++++++++++++++++++++++++++++++++++++


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
    [ReadOnly] public string Tooltip = "←カーソルをTolltipに合わせる";

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

    //==================
    // コンテキストメニュー追加(右クリックであらかじめ用意した関数・メソッドを実行出来る)
    //==================
    [ContextMenuItem("Random", "RandomNumber")] // "メニューの名前", "関数名"
    public int speed;
    void RandomNumber()
    {
        speed = Random.Range(0, 1000);
    }

    //==================
    // テキスト追加
    //==================
    [TextArea(1, 10)] // "最小表示行数", "スクロールバー出るまでの行数"
    public string Text;



    //==================
    // Enumのラベルの変更
    //==================
    public enum TEST_ENUM
    {
        [EnumCName("てすと１")] // 列挙したやつに名前つける
        Test_1,
        [EnumCName("てすと２")] // 列挙したやつに名前つける
        Test_2,
        [EnumCName("てすと３")] // 列挙したやつに名前つける
        Test_3
    }
    //[EnumPName(typeof(TEST_ENUM))] // 列挙したEnumの使用方法(ラベルなし)
    [EnumPName("てすとEnum", typeof(TEST_ENUM))] // 列挙したEnumの使用方法(ラベルあり)
    public TEST_ENUM TestEnum;






    private void Start()
    {
        NoOutput = Output;
    }
}
