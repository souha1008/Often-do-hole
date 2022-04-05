using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


// Gimmick_Enemyクラスを拡張
[CustomEditor(typeof(Gimmick_Enemy))]

// 複数選択有効
[CanEditMultipleObjects]

public class Gimmick_EnemyEditor : Editor
{
    // シリアライズオブジェクト
    SerializedProperty MoveInfo;

    void OnEnable()
    {
        // シリアライズオブジェクトを取得
        MoveInfo = serializedObject.FindProperty("MoveInfo");
        MoveInfo.isExpanded = false; // ドロップダウン最初から閉めとく
    }
    public override void OnInspectorGUI()
    {
        // 内部キャッシュから値をロードする
        serializedObject.Update();
  

        // イージングタイプ用のテキスト
        string[] EasingTypeText = new string[]
                {
                    "直線移動",
                    "加速(最弱)_IN", "加速(最弱)_OUT", "加速(最弱)_INOUT",
                    "加速(弱)_IN", "加速(弱)_OUT", "加速(弱)_INOUT",
                    "加速(中)_IN", "加速(中)_OUT", "加速(中)_INOUT",
                    "加速(強)_IN", "加速(強)_OUT", "加速(強)_INOUT",
                    "ゆるふわ_IN", "ゆるふわ_OUT", "ゆるふわ_INOUT",
                    "キレがある_IN", "キレがある_OUT", "キレがある_INOUT",
                    "滑り加速_IN", "滑り加速_OUT", "滑り加速_INOUT",
                    "ゴム移動_IN", "ゴム移動_OUT", "ゴム移動_INOUT",
                    "少し行き過ぎ_IN", "少し行き過ぎ_OUT", "少し行き過ぎ_INOUT",
                    "バウンド_IN", "バウンド_OUT", "バウンド_INOUT"
                };

        Gimmick_Enemy Enemy = target as Gimmick_Enemy;

        // エディターの変更確認
        EditorGUI.BeginChangeCheck(); 

        // PrayerMainの移動情報
        EditorGUILayout.PropertyField(MoveInfo, new GUIContent("現在の移動情報"));    // 移動処理表示


        // X・Y方向移動
        Enemy.Move_X = EditorGUILayout.BeginToggleGroup("X方向移動", Enemy.Move_X); // グループ化始まり
        if (Enemy.Move_X)
        {
            Enemy.MoveDirection_X=
                (MOVE_DIRECTION_X)EditorGUILayout.Popup("移動方向", (int)Enemy.MoveDirection_X, new string[] { "右移動", "左移動" });
            Enemy.MoveLength_X = EditorGUILayout.FloatField("移動距離", Enemy.MoveLength_X);
            Enemy.MoveTime_X1 = EditorGUILayout.FloatField("何秒かけて移動するか[行き]", Enemy.MoveTime_X1);
            Enemy.MoveTime_X2 = EditorGUILayout.FloatField("何秒かけて移動するか[帰り]", Enemy.MoveTime_X2);
            Enemy.StartTime_X = EditorGUILayout.Slider("初期経過時間", Enemy.StartTime_X, 0, Enemy.MoveTime_X1);    // スライダーを表示（引数は「初期値,最小値,最大値」）
            Enemy.EasingType_X1 =
                (EASING_TYPE)EditorGUILayout.Popup("イージング(動き方)[行き]", (int)Enemy.EasingType_X1, EasingTypeText);
            Enemy.EasingType_X2 =
                (EASING_TYPE)EditorGUILayout.Popup("イージング(動き方)[帰り]", (int)Enemy.EasingType_X2, EasingTypeText);
        }
        EditorGUILayout.EndToggleGroup();   // グループ化


        Enemy.Move_Y = EditorGUILayout.BeginToggleGroup("Y方向移動", Enemy.Move_Y); // グループ化始まり
        if (Enemy.Move_Y)
        {
            Enemy.MoveDirection_Y =
                (MOVE_DIRECTION_Y)EditorGUILayout.Popup("移動方向", (int)Enemy.MoveDirection_Y, new string[] { "上移動", "下移動" });
            Enemy.MoveLength_Y = EditorGUILayout.FloatField("移動距離", Enemy.MoveLength_Y);
            Enemy.MoveTime_Y1 = EditorGUILayout.FloatField("何秒かけて移動するか[行き]", Enemy.MoveTime_Y1);
            Enemy.MoveTime_Y2 = EditorGUILayout.FloatField("何秒かけて移動するか[帰り]", Enemy.MoveTime_Y2);
            Enemy.StartTime_Y = EditorGUILayout.Slider("初期経過時間", Enemy.StartTime_Y, 0, Enemy.MoveTime_Y1);    // スライダーを表示（引数は「初期値,最小値,最大値」）
            Enemy.EasingType_Y1 =
                (EASING_TYPE)EditorGUILayout.Popup("イージング(動き方)[行き]", (int)Enemy.EasingType_Y1, EasingTypeText);
            Enemy.EasingType_Y2 =
                (EASING_TYPE)EditorGUILayout.Popup("イージング(動き方)[帰り]", (int)Enemy.EasingType_Y2, EasingTypeText);
        }
        EditorGUILayout.EndToggleGroup();   // グループ化

        // エディターの変更確認
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target); // 選択オブジェクト更新

            //選択されている全てものについて処理
            foreach (Object EnemyObject in targets)
            {
                Gimmick_Enemy subEnemy = EnemyObject as Gimmick_Enemy;

                subEnemy.Move_X = Enemy.Move_X;
                subEnemy.Move_Y = Enemy.Move_Y;
                subEnemy.MoveDirection_X = Enemy.MoveDirection_X;
                subEnemy.MoveDirection_Y = Enemy.MoveDirection_Y;
                subEnemy.MoveLength_X = Enemy.MoveLength_X;
                subEnemy.MoveLength_Y = Enemy.MoveLength_Y;
                subEnemy.MoveTime_X1 = Enemy.MoveTime_X1;
                subEnemy.MoveTime_X2 = Enemy.MoveTime_X2;
                subEnemy.MoveTime_Y1 = Enemy.MoveTime_Y1;
                subEnemy.MoveTime_Y2 = Enemy.MoveTime_Y2;
                subEnemy.StartTime_X = Enemy.StartTime_X;
                subEnemy.StartTime_Y = Enemy.StartTime_Y;
                subEnemy.EasingType_X1 = Enemy.EasingType_X1;
                subEnemy.EasingType_X2 = Enemy.EasingType_X2;
                subEnemy.EasingType_Y1 = Enemy.EasingType_Y1;
                subEnemy.EasingType_Y2 = Enemy.EasingType_Y2;

                EditorUtility.SetDirty(EnemyObject);
            }
        }    

        // 内部キャッシュに値を保存する
        //serializedObject.ApplyModifiedProperties();
    }
}
#endif

public class Gimmick_Enemy : Gimmick_Main
{
    // 変数
    public EASING_TYPE EasingType_X1, EasingType_X2;        // イージングタイプ
    public EASING_TYPE EasingType_Y1, EasingType_Y2;        // イージングタイプ

    public float MoveLength_X = 8.0f;          // 動く距離
    public float MoveLength_Y = 8.0f;          // 動く距離
    public float MoveTime_X1, MoveTime_X2 = 2.0f; // 何秒かけて移動するか
    public float MoveTime_Y1, MoveTime_Y2 = 2.0f; // 何秒かけて移動するか
    public float StartTime_X;                   // 初期経過時間
    public float StartTime_Y;                   // 初期経過時間
    public MOVE_DIRECTION_X MoveDirection_X;    // 移動方向X
    public MOVE_DIRECTION_Y MoveDirection_Y;    // 移動方向Y

    public bool Move_X;                         // X方向移動が使われているか
    public bool Move_Y;                         // Y方向移動が使われているか

    private bool MoveRight, MoveUp;             // 移動方向(右)(上)
    private bool StartMoveRight, StartMoveUp;   // 初期移動方向(右)(上)
    private bool NowMove_X, NowMove_Y;          // 移動中か
    private float NowTime_X, NowTime_Y;         // 経過時間
    private float StartPos_X, StartPos_Y;       // 初期座標
    private float Fugou_X, Fugou_Y;             // 符号

    private Vector3 OldPos;                     // ひとつ前の座標確認用

    // スタート処理
    public override void Init()
    {
        // 初期化
        NowMove_X = true;
        NowMove_Y = true;
        NowTime_X = StartTime_X;
        NowTime_Y = StartTime_Y;
        StartPos_X = this.gameObject.transform.position.x;
        StartPos_Y = this.gameObject.transform.position.y;
        MoveRight = MoveDirectionBoolChangeX(MoveDirection_X);
        MoveUp = MoveDirectionBoolChangeY(MoveDirection_Y);
        Fugou_X = CalculationScript.FugouChange(MoveRight);
        Fugou_Y = CalculationScript.FugouChange(MoveUp);
        StartMoveRight = MoveRight;
        StartMoveUp = MoveUp;
        OldPos = this.gameObject.transform.position;
    }

    // 敵の動き処理
    public override void FixedMove()
    {
        OldPos = this.gameObject.transform.position;
        if (Move_X) // X方向移動が使用されていたら
        {
            if (NowMove_X)
            {
                // 移動処理
                EASING_TYPE EasingTypeX;
                float MoveTimeX;

                if (StartMoveRight == MoveRight)
                {
                    EasingTypeX = EasingType_X1;
                    MoveTimeX = MoveTime_X1;
                }    
                else
                {
                    EasingTypeX = EasingType_X2;
                    MoveTimeX = MoveTime_X2;
                }   

                this.gameObject.transform.position = 
                    new Vector3(Easing.EasingTypeFloat(EasingTypeX, NowTime_X, MoveTimeX, StartPos_X, StartPos_X + MoveLength_X * Fugou_X), gameObject.transform.position.y, gameObject.transform.position.z);

                NowTime_X += Time.fixedDeltaTime; // 時間加算

                // 移動終了
                if (NowTime_X > MoveTimeX)
                    NowMove_X = false;
            }
            else
            {
                StartPos_X = this.gameObject.transform.position.x;  // 初期座標セット
                NowTime_X = 0.0f; // 時間リセット
                MoveRight = !MoveRight; // 向き反転
                Fugou_X = CalculationScript.FugouChange(MoveRight);   // 符号反転
                MoveDirection_X = BoolMoveDirectionChangeX(MoveRight); // 向き表示変化
                NowMove_X = true; // 動く
            }
        }

        if (Move_Y) // Y方向移動が使用されていたら
        {
            if (NowMove_Y)
            {
                // 移動処理
                EASING_TYPE EasingTypeY;
                float MoveTimeY;

                if (StartMoveUp == MoveUp)
                {
                    EasingTypeY = EasingType_Y1;
                    MoveTimeY = MoveTime_Y1;
                }     
                else
                {
                    EasingTypeY = EasingType_Y2;
                    MoveTimeY = MoveTime_Y2;
                }
                
                this.gameObject.transform.position =
                    new Vector3(gameObject.transform.position.x, Easing.EasingTypeFloat(EasingTypeY, NowTime_Y, MoveTimeY, StartPos_Y, StartPos_Y + MoveLength_Y * Fugou_Y), gameObject.transform.position.z);

                NowTime_Y += Time.fixedDeltaTime; // 時間加算

                // 移動終了
                if (NowTime_Y > MoveTimeY)
                    NowMove_Y = false;
            }
            else
            {
                StartPos_Y = this.gameObject.transform.position.y; // 初期座標セット
                NowTime_Y = 0.0f; // 時間リセット
                MoveUp = !MoveUp; // 向き反転
                Fugou_Y = CalculationScript.FugouChange(MoveUp);   // 符号反転
                MoveDirection_Y = BoolMoveDirectionChangeY(MoveUp); // 向き表示変化
                NowMove_Y = true; // 動く
            }
        }

        transform.rotation *= Quaternion.Euler(0, 0, 10);  // 回転
    }

    // 敵死亡処理
    public override void Death()
    {
        // ※死亡エフェクト

        // 自身を消す
        Destroy(this.gameObject);
    }

    // 何かと衝突処理(トリガー)
    public override void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Bullet")
        {
            // ヒットストップ
            GameSpeedManager.Instance.StartHitStop();
            // ※当たったエフェクト

            Death(); // 死亡処理   
        }

        if (collider.gameObject.tag == "Player")
        {
            // プレイヤーノックバックステートに移行

        }
    }

    private bool MoveDirectionBoolChangeX(MOVE_DIRECTION_X MoveDirection_X)
    {
        if (MoveDirection_X == MOVE_DIRECTION_X.MoveRight)
            return true;
        else
            return false;
    }

    private MOVE_DIRECTION_X BoolMoveDirectionChangeX(bool MoveRight)
    {
        if (MoveRight)
            return MOVE_DIRECTION_X.MoveRight;
        else
            return MOVE_DIRECTION_X.MoveLeft;
    }

    private bool MoveDirectionBoolChangeY(MOVE_DIRECTION_Y MoveDirection_Y)
    {
        if (MoveDirection_Y == MOVE_DIRECTION_Y.MoveUp)
            return true;
        else
            return false;
    }

    private MOVE_DIRECTION_Y BoolMoveDirectionChangeY(bool MoveUp)
    {
        if (MoveUp)
            return MOVE_DIRECTION_Y.MoveUp;
        else
            return MOVE_DIRECTION_Y.MoveDown;
    }
}
