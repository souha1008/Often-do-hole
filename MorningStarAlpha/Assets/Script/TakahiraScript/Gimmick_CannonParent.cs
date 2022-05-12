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
    SerializedProperty WayRight;

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
        WayRight = serializedObject.FindProperty("WayRight");
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
        EditorGUILayout.Space(5);

        // 向き
        EditorGUILayout.LabelField("[向き]");
        EditorGUILayout.Space(5);

        WayRight.boolValue = EditorGUILayout.Toggle("右向きか", WayRight.boolValue);


        // 内部キャッシュに値を保存する
        serializedObject.ApplyModifiedProperties();
    }
}
#endif

public class Gimmick_CannonParent : Gimmick_Main
{
    public GameObject CannonChild;  // 砲台から打ち出す弾オブジェクト
    public float StartLength = 15;  // 砲台が動き出すプレイヤーとの距離
    public float ShootTime = 5;     // 打ち出す間隔

    public bool FixedRadFlag;       // 角度の固定
    public bool ChaseFlag;          // 弾が追尾するか
    public float Speed = 5;         // 弾の速度
    public float LifeTime = 5;      // 弾の生存時間

    private const float Length = 8;        // 弾の出現位置調整用

    public bool WayRight;           // サメの向き


    private float NowRotateZ;   // 回転Z
    private bool StartFlag;     // 起動フラグ
    private float NowShootTime; // 経過時間

    public override void Init()
    {
        // 初期化
        StartFlag = false;
        NowShootTime = ShootTime; // 最初に1回弾発射

        // コライダー
        Cd.isTrigger = false;

        // リジッドボディ
        Rb.isKinematic = true;

        // 向きを合わせる
        if (WayRight)
        {
            this.gameObject.transform.rotation = Quaternion.identity;
        }
        else
        {
            this.gameObject.transform.rotation = Quaternion.Euler(0, 180.0f, 0);
        }
    }

    public override void UpdateMove()
    {
        // プレイヤーの座標
        Vector3 PlayerPos = PlayerMain.instance.transform.position;
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
                float Rot = CalculationScript.UnityTwoPointAngle360(ThisPos, PlayerPos);
                if (WayRight)
                {
                    if ((Rot >= 0 && Rot <= 90) || (Rot >= 270 && Rot <= 360))
                    {
                        // プレイヤーの方向に向く
                        this.gameObject.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Rot);
                        NowRotateZ = Rot;
                    }
                }
                else
                {
                    if ((Rot >= 90 && Rot <= 270))
                    {
                        // プレイヤーの方向に向く
                        this.gameObject.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, (180.0f - Rot));
                        NowRotateZ = Rot;

                    }
                }
            }

            // 弾発射処理
            if (NowShootTime >= ShootTime)
            {
                Shoot();
                NowShootTime = 0.0f; // 経過時間リセット
            }
            else
                NowShootTime += Time.deltaTime; // 経過時間加算
        }
    }

    // 弾発射
    public void Shoot()
    {
        if (CannonChild != null)
        {
            VecQuaternion vecQuaternion = 
                CalculationScript.PointRotate(gameObject.transform.position, gameObject.transform.position + new Vector3(Length, -1.0f, 0), NowRotateZ, Vector3.forward);

            GameObject Child = 
                Instantiate(CannonChild, vecQuaternion.Pos, Quaternion.Euler(0, 0, NowRotateZ)); // 弾生成




            Child.GetComponent<Gimmick_CannonChild>().SetCannonChild(Speed, LifeTime, ChaseFlag); // 弾の値セット
        }
    }

    public override void Death()
    {
        // 自分自身を消す
        Destroy(this.gameObject);
    }
}
