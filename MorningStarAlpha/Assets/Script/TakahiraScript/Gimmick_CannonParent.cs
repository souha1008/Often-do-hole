using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

// Gimmick_CannonParentクラスを拡張
[CustomEditor(typeof(Gimmick_CannonParent))]

// 複数選択有効
[CanEditMultipleObjects]

public class Gimmick_CannonParentEditor : Editor
{
    // シリアライズオブジェクト
    SerializedProperty MoveInfo;
    SerializedProperty CannonChild, StartLength, ShootTime;
    SerializedProperty FixedRadFlag, ChaseFlag, Speed, LifeTime;

    void OnEnable()
    {
        // シリアライズオブジェクトを取得
        MoveInfo = serializedObject.FindProperty("MoveInfo");
        MoveInfo.isExpanded = false; // ドロップダウン最初から閉めとく

        CannonChild = serializedObject.FindProperty("CannonChild");
        StartLength = serializedObject.FindProperty("StartLength");
        ShootTime = serializedObject.FindProperty("ShootTime");
        FixedRadFlag = serializedObject.FindProperty("FixedRadFlag");
        ChaseFlag = serializedObject.FindProperty("ChaseFlag");
        Speed = serializedObject.FindProperty("Speed");
        LifeTime = serializedObject.FindProperty("LifeTime");
    }
    public override void OnInspectorGUI()
    {
        // 内部キャッシュから値をロードする
        serializedObject.Update();

        // PrayerMainの移動情報
        EditorGUILayout.PropertyField(MoveInfo, new GUIContent("現在の移動情報"));    // 移動処理表示
        EditorGUILayout.Space(5);


        // 砲台の設定
        EditorGUILayout.LabelField("[砲台の設定]");
        EditorGUILayout.Space(5);

        FixedRadFlag.boolValue = EditorGUILayout.Toggle("砲台の角度固定", FixedRadFlag.boolValue);
        CannonChild.objectReferenceValue = EditorGUILayout.ObjectField("打ち出す弾オブジェクト", CannonChild.objectReferenceValue, typeof(GameObject), true);
        StartLength.floatValue = EditorGUILayout.FloatField("動き出すプレイヤーとの距離", StartLength.floatValue);
        ShootTime.floatValue = EditorGUILayout.FloatField("打ち出す間隔の時間", ShootTime.floatValue);
        EditorGUILayout.Space(5);

        // 弾の設定
        EditorGUILayout.LabelField("[弾の設定]");
        EditorGUILayout.Space(5);

        

        if (!FixedRadFlag.boolValue) // 砲台固定でないなら
            ChaseFlag.boolValue = EditorGUILayout.Toggle("弾が追尾するか", ChaseFlag.boolValue);
        else
            ChaseFlag.boolValue = false;

        Speed.floatValue = EditorGUILayout.FloatField("弾の速度", Speed.floatValue);
        LifeTime.floatValue = EditorGUILayout.FloatField("弾の生存時間", LifeTime.floatValue);


        // 内部キャッシュに値を保存する
        serializedObject.ApplyModifiedProperties();
    }
}
#endif

public class Gimmick_CannonParent : Gimmick_Main
{
    [Header("[砲台の設定]")]
    [Label("打ち出す弾オブジェクト")]
    public GameObject CannonChild;  // 砲台から打ち出す弾オブジェクト
    [Label("動き出すプレイヤーとの距離")]
    public float StartLength = 15;  // 砲台が動き出すプレイヤーとの距離
    [Label("打ち出す間隔の時間")]
    public float ShootTime = 5;     // 打ち出す間隔

    [Header("[弾の設定]")]
    [Label("砲台の角度固定")]
    public bool FixedRadFlag;       // 角度の固定
    [Label("弾が追尾するか")]
    public bool ChaseFlag;          // 弾が追尾するか
    [Label("弾の速度")]
    public float Speed = 5;         // 弾の速度
    [Label("弾の生存時間")]
    public float LifeTime = 5;      // 弾の生存時間



    private bool StartFlag;     // 起動フラグ
    private float NowShootTime; // 経過時間

    [HideInInspector] public GameObject PlayerObject; // プレイヤーオブジェクト
    

    public override void Init()
    {
        // 初期化
        StartFlag = false;
        NowShootTime = ShootTime; // 最初に1回弾発射

        // プレイヤーオブジェクト取得
        PlayerObject = GameObject.Find("Player");
    }

    public override void UpdateMove()
    {
        // プレイヤーの座標
        Vector3 PlayerPos = PlayerObject.transform.position;
        Vector3 ThisPos = this.gameObject.transform.position;


        // プレイヤーとの距離を確認
        if (Vector2.Distance(PlayerPos, ThisPos) <= StartLength)
            StartFlag = true;
        else
            StartFlag = false;


        // 起動時の処理
        if (StartFlag)
        {
            if (!FixedRadFlag) // 砲台固定でないなら
            {
                // プレイヤーの方向に向く
                Rad.z = CalculationScript.UnityTwoPointAngle360(ThisPos, PlayerPos);
            }

            // 弾発射処理
            if (NowShootTime >= ShootTime)
            {
                Shoot();
                NowShootTime = 0.0f; // 経過時間リセット
            }

            NowShootTime += Time.deltaTime; // 経過時間加算
        }
    }

    // 弾発射
    public void Shoot()
    {
        if (CannonChild != null)
        {
            GameObject Child = Instantiate(CannonChild, gameObject.transform.position, Quaternion.Euler(Rad)); // 弾生成

            Child.GetComponent<Gimmick_CannonChild>().SetCannonChild(PlayerObject, Speed, LifeTime, ChaseFlag); // 弾の値セット
        }
    }

    public override void Death()
    {
        // 自分自身を消す
        Destroy(this.gameObject);
    }
}
