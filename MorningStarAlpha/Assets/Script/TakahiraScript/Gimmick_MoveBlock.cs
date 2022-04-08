using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


// Gimmick_MoveBlockクラスを拡張
[CustomEditor(typeof(Gimmick_MoveBlock))]

// 複数選択有効
[CanEditMultipleObjects]

public class Gimmick_MoveBlockEditor : Editor
{
    // シリアライズオブジェクト
    SerializedProperty MoveInfo;
    SerializedProperty Move_X, Move_Y;
    SerializedProperty MoveDirection_X, MoveDirection_Y;
    SerializedProperty MoveLength_X, MoveLength_Y;
    SerializedProperty MoveTime_X, MoveTime_Y;
    SerializedProperty StartTime_X, StartTime_Y;

    void OnEnable()
    {
        // シリアライズオブジェクトを取得
        MoveInfo = serializedObject.FindProperty("MoveInfo");
        MoveInfo.isExpanded = false; // ドロップダウン最初から閉めとく

        Move_X = serializedObject.FindProperty("Move_X");
        Move_Y = serializedObject.FindProperty("Move_Y");
        MoveDirection_X = serializedObject.FindProperty("MoveDirection_X");
        MoveDirection_Y = serializedObject.FindProperty("MoveDirection_Y");
        MoveLength_X = serializedObject.FindProperty("MoveLength_X");
        MoveLength_Y = serializedObject.FindProperty("MoveLength_Y");
        MoveTime_X = serializedObject.FindProperty("MoveTime_X");
        MoveTime_Y = serializedObject.FindProperty("MoveTime_Y");
        StartTime_X = serializedObject.FindProperty("StartTime_X");
        StartTime_Y = serializedObject.FindProperty("StartTime_Y");
    }
    public override void OnInspectorGUI()
    {
        // 内部キャッシュから値をロードする
        serializedObject.Update();

        // PrayerMainの移動情報
        EditorGUILayout.PropertyField(MoveInfo, new GUIContent("現在の移動情報"));    // 移動処理表示


        // X・Y方向移動
        Move_X.boolValue = EditorGUILayout.BeginToggleGroup("X方向移動", Move_X.boolValue); // グループ化始まり
        if (Move_X.boolValue)
        {
            MoveDirection_X.enumValueIndex =
            EditorGUILayout.Popup("移動方向", MoveDirection_X.enumValueIndex, new string[] { "右移動", "左移動" });
            MoveLength_X.floatValue = EditorGUILayout.FloatField("移動距離", MoveLength_X.floatValue);
            MoveTime_X.floatValue = EditorGUILayout.FloatField("何秒かけて移動するか", MoveTime_X.floatValue);
            StartTime_X.floatValue = EditorGUILayout.Slider("初期経過時間", StartTime_X.floatValue, 0, MoveTime_X.floatValue);    // スライダーを表示（引数は「初期値,最小値,最大値」）
        }     
        EditorGUILayout.EndToggleGroup();   // グループ化


        Move_Y.boolValue = EditorGUILayout.BeginToggleGroup("Y方向移動", Move_Y.boolValue); // グループ化始まり
        if (Move_Y.boolValue)
        {
            MoveDirection_Y.enumValueIndex =
            EditorGUILayout.Popup("移動方向", MoveDirection_Y.enumValueIndex, new string[] { "上移動", "下移動" });
            MoveLength_Y.floatValue = EditorGUILayout.FloatField("移動距離", MoveLength_Y.floatValue);
            MoveTime_Y.floatValue = EditorGUILayout.FloatField("何秒かけて移動するか", MoveTime_Y.floatValue);
            StartTime_Y.floatValue = EditorGUILayout.Slider("初期経過時間", StartTime_Y.floatValue, 0, MoveTime_Y.floatValue);    // スライダーを表示（引数は「初期値,最小値,最大値」）
        }
        EditorGUILayout.EndToggleGroup();   // グループ化

        // 内部キャッシュに値を保存する
        serializedObject.ApplyModifiedProperties();
    }
}
#endif

public enum MOVE_DIRECTION_X
{
    [EnumCName("右方向")]
    MoveRight,
    [EnumCName("左方向")]
    MoveLeft
}
public enum MOVE_DIRECTION_Y
{
    [EnumCName("上方向")]
    MoveUp,
    [EnumCName("下方向")]
    MoveDown
}

public class Gimmick_MoveBlock : Gimmick_Main
{
    // 変数
    public float MoveLength_X = 15.0f;    // 動く距離
    public float MoveLength_Y = 15.0f;    // 動く距離
    public float MoveTime_X = 3.0f;       // 何秒かけて移動するか
    public float MoveTime_Y = 3.0f;       // 何秒かけて移動するか
    public float StartTime_X;             // 初期経過時間
    public float StartTime_Y;             // 初期経過時間
    public MOVE_DIRECTION_X MoveDirection_X; // 移動方向
    public MOVE_DIRECTION_Y MoveDirection_Y; // 移動方向

    public bool Move_X;                 // X方向移動が使われているか
    public bool Move_Y;                 // Y方向移動が使われているか

    private bool MoveRight, MoveUp;             // 移動方向(右)(上)
    private bool NowMove_X, NowMove_Y;          // 移動中か
    private float NowTime_X, NowTime_Y;         // 経過時間
    private float StartPos_X, StartPos_Y;       // 初期座標
    private float Fugou_X, Fugou_Y;             // 符号

    private bool PlayerMoveFlag;
    private bool BulletMoveFlag;

    private Vector3 OldPos;

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
        OldPos = this.gameObject.transform.position;

        PlayerMoveFlag = false;
        BulletMoveFlag = false;

        // コリジョン
        this.gameObject.GetComponent<Collider>().isTrigger = false;  // トリガーオフ

        // リジッドボディ
        Rb.isKinematic = true;
    }

    public override void FixedMove()
    {
        OldPos = this.gameObject.transform.position;
        if (Move_X) // X方向移動が使用されていたら
        {
            if (NowMove_X)
            {
                this.gameObject.transform.position =
                    new Vector3(Easing.QuadInOut(NowTime_X, MoveTime_X, StartPos_X, StartPos_X + MoveLength_X * Fugou_X), gameObject.transform.position.y, gameObject.transform.position.z);
                NowTime_X += Time.fixedDeltaTime;

                // 移動終了
                if (NowTime_X > MoveTime_X)
                    NowMove_X = false;
            }
            else
            {
                StartPos_X = this.gameObject.transform.position.x;
                NowTime_X = 0.0f;
                MoveRight = !MoveRight;   // 向き反転
                Fugou_X = CalculationScript.FugouChange(MoveRight);   // 符号反転
                MoveDirection_X = BoolMoveDirectionChangeX(MoveRight); // 向き表示変化
                NowMove_X = true;
            }
        }

        if (Move_Y) // Y方向移動が使用されていたら
        {
            if (NowMove_Y)
            {
                this.gameObject.transform.position =
                    new Vector3(gameObject.transform.position.x, Easing.QuadInOut(NowTime_Y, MoveTime_Y, StartPos_Y, StartPos_Y + MoveLength_Y * Fugou_Y), gameObject.transform.position.z);
                NowTime_Y += Time.fixedDeltaTime;

                // 移動終了
                if (NowTime_Y > MoveTime_Y)
                    NowMove_Y = false;
            }
            else
            {
                StartPos_Y = this.gameObject.transform.position.y;
                NowTime_Y = 0.0f;
                MoveUp = !MoveUp;   // 向き反転
                Fugou_Y = CalculationScript.FugouChange(MoveUp);   // 符号反転
                MoveDirection_Y = BoolMoveDirectionChangeY(MoveUp); // 向き表示変化
                NowMove_Y = true;
            }
        }

        // プレイヤーの移動
        if (PlayerMoveFlag)
        {
            //Debug.Log("プレイヤーブロックの上移動中");
            //Debug.Log(this.gameObject.transform.position.x - OldPos.x);
            PlayerMain.instance.transform.position += 
                new Vector3(this.gameObject.transform.position.x - OldPos.x, this.gameObject.transform.position.y - OldPos.y, 0);

            //PlayerMainScript.addVel = new Vector3(this.gameObject.transform.position.x - OldPos.x, this.gameObject.transform.position.y - OldPos.y, 0) / Time.fixedDeltaTime;
        }

        // 錨オブジェクトの移動
        if (BulletMoveFlag)
        {
            if (BulletMain.instance.NowBulletState == EnumBulletState.BulletStop)
            {
                PlayerMain.instance.BulletScript.transform.position +=
                    new Vector3(this.gameObject.transform.position.x - OldPos.x, this.gameObject.transform.position.y - OldPos.y, 0);
            }
            else
            {
                BulletMoveFlag = false;
            }
        }
    }

    public override void Death()
    {

    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") &&
                PlayerMain.instance.getFootHit().collider != null &&
                PlayerMain.instance.getFootHit().collider.gameObject == this.gameObject)
        {
            PlayerMoveFlag = true;
        }
        if (collision.gameObject.CompareTag("Bullet"))
        {
            BulletMoveFlag = true;
        }
    }
    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (PlayerMain.instance.getFootHit().collider != null &&
                 PlayerMain.instance.getFootHit().collider.gameObject == this.gameObject)
            {
                PlayerMoveFlag = true;
            }
        }
        if (collision.gameObject.CompareTag("Bullet"))
        {
            BulletMoveFlag = true;
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMoveFlag = false;
        }
        //if (collision.gameObject.CompareTag("Bullet"))
        //{
        //    BulletMoveFlag = false;
        //}
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
