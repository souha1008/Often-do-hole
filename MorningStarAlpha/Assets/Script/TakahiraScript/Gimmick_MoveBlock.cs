using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


// Gimmick_MoveBlock�N���X���g��
[CustomEditor(typeof(Gimmick_MoveBlock))]

// �����I��L��
[CanEditMultipleObjects]

public class Gimmick_MoveBlockEditor : Editor
{
    // �V���A���C�Y�I�u�W�F�N�g
    SerializedProperty MoveInfo;
    SerializedProperty Move_X, Move_Y;
    SerializedProperty MoveDirection_X, MoveDirection_Y;
    SerializedProperty MoveLength_X, MoveLength_Y;
    SerializedProperty MoveTime_X, MoveTime_Y;
    SerializedProperty StartTime_X, StartTime_Y;

    void OnEnable()
    {
        // �V���A���C�Y�I�u�W�F�N�g���擾
        MoveInfo = serializedObject.FindProperty("MoveInfo");
        MoveInfo.isExpanded = false; // �h���b�v�_�E���ŏ�����߂Ƃ�

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
        // �����L���b�V������l�����[�h����
        serializedObject.Update();

        // PrayerMain�̈ړ����
        EditorGUILayout.PropertyField(MoveInfo, new GUIContent("���݂̈ړ����"));    // �ړ������\��


        // X�EY�����ړ�
        Move_X.boolValue = EditorGUILayout.BeginToggleGroup("X�����ړ�", Move_X.boolValue); // �O���[�v���n�܂�
        if (Move_X.boolValue)
        {
            MoveDirection_X.enumValueIndex =
            EditorGUILayout.Popup("�ړ�����", MoveDirection_X.enumValueIndex, new string[] { "�E�ړ�", "���ړ�" });
            MoveLength_X.floatValue = EditorGUILayout.FloatField("�ړ�����", MoveLength_X.floatValue);
            MoveTime_X.floatValue = EditorGUILayout.FloatField("���b�����Ĉړ����邩", MoveTime_X.floatValue);
            StartTime_X.floatValue = EditorGUILayout.Slider("�����o�ߎ���", StartTime_X.floatValue, 0, MoveTime_X.floatValue);    // �X���C�_�[��\���i�����́u�����l,�ŏ��l,�ő�l�v�j
        }     
        EditorGUILayout.EndToggleGroup();   // �O���[�v��


        Move_Y.boolValue = EditorGUILayout.BeginToggleGroup("Y�����ړ�", Move_Y.boolValue); // �O���[�v���n�܂�
        if (Move_Y.boolValue)
        {
            MoveDirection_Y.enumValueIndex =
            EditorGUILayout.Popup("�ړ�����", MoveDirection_Y.enumValueIndex, new string[] { "��ړ�", "���ړ�" });
            MoveLength_Y.floatValue = EditorGUILayout.FloatField("�ړ�����", MoveLength_Y.floatValue);
            MoveTime_Y.floatValue = EditorGUILayout.FloatField("���b�����Ĉړ����邩", MoveTime_Y.floatValue);
            StartTime_Y.floatValue = EditorGUILayout.Slider("�����o�ߎ���", StartTime_Y.floatValue, 0, MoveTime_Y.floatValue);    // �X���C�_�[��\���i�����́u�����l,�ŏ��l,�ő�l�v�j
        }
        EditorGUILayout.EndToggleGroup();   // �O���[�v��

        // �����L���b�V���ɒl��ۑ�����
        serializedObject.ApplyModifiedProperties();
    }
}
#endif

public enum MOVE_DIRECTION_X
{
    [EnumCName("�E����")]
    MoveRight,
    [EnumCName("������")]
    MoveLeft
}
public enum MOVE_DIRECTION_Y
{
    [EnumCName("�����")]
    MoveUp,
    [EnumCName("������")]
    MoveDown
}

public class Gimmick_MoveBlock : Gimmick_Main
{
    // �ϐ�
    public float MoveLength_X = 15.0f;    // ��������
    public float MoveLength_Y = 15.0f;    // ��������
    public float MoveTime_X = 3.0f;       // ���b�����Ĉړ����邩
    public float MoveTime_Y = 3.0f;       // ���b�����Ĉړ����邩
    public float StartTime_X;             // �����o�ߎ���
    public float StartTime_Y;             // �����o�ߎ���
    public MOVE_DIRECTION_X MoveDirection_X; // �ړ�����
    public MOVE_DIRECTION_Y MoveDirection_Y; // �ړ�����

    public bool Move_X;                 // X�����ړ����g���Ă��邩
    public bool Move_Y;                 // Y�����ړ����g���Ă��邩

    private bool MoveRight, MoveUp;             // �ړ�����(�E)(��)
    private bool NowMove_X, NowMove_Y;          // �ړ�����
    private float NowTime_X, NowTime_Y;         // �o�ߎ���
    private float StartPos_X, StartPos_Y;       // �������W
    private float Fugou_X, Fugou_Y;             // ����

    private bool PlayerMoveFlag;
    private bool BulletMoveFlag;

    private Vector3 OldPos;

    public override void Init()
    {
        // ������
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

        // �R���W����
        this.gameObject.GetComponent<Collider>().isTrigger = false;  // �g���K�[�I�t

        // ���W�b�h�{�f�B
        Rb.isKinematic = true;
    }

    public override void FixedMove()
    {
        OldPos = this.gameObject.transform.position;
        if (Move_X) // X�����ړ����g�p����Ă�����
        {
            if (NowMove_X)
            {
                this.gameObject.transform.position =
                    new Vector3(Easing.QuadInOut(NowTime_X, MoveTime_X, StartPos_X, StartPos_X + MoveLength_X * Fugou_X), gameObject.transform.position.y, gameObject.transform.position.z);
                NowTime_X += Time.fixedDeltaTime;

                // �ړ��I��
                if (NowTime_X > MoveTime_X)
                    NowMove_X = false;
            }
            else
            {
                StartPos_X = this.gameObject.transform.position.x;
                NowTime_X = 0.0f;
                MoveRight = !MoveRight;   // �������]
                Fugou_X = CalculationScript.FugouChange(MoveRight);   // �������]
                MoveDirection_X = BoolMoveDirectionChangeX(MoveRight); // �����\���ω�
                NowMove_X = true;
            }
        }

        if (Move_Y) // Y�����ړ����g�p����Ă�����
        {
            if (NowMove_Y)
            {
                this.gameObject.transform.position =
                    new Vector3(gameObject.transform.position.x, Easing.QuadInOut(NowTime_Y, MoveTime_Y, StartPos_Y, StartPos_Y + MoveLength_Y * Fugou_Y), gameObject.transform.position.z);
                NowTime_Y += Time.fixedDeltaTime;

                // �ړ��I��
                if (NowTime_Y > MoveTime_Y)
                    NowMove_Y = false;
            }
            else
            {
                StartPos_Y = this.gameObject.transform.position.y;
                NowTime_Y = 0.0f;
                MoveUp = !MoveUp;   // �������]
                Fugou_Y = CalculationScript.FugouChange(MoveUp);   // �������]
                MoveDirection_Y = BoolMoveDirectionChangeY(MoveUp); // �����\���ω�
                NowMove_Y = true;
            }
        }

        // �v���C���[�̈ړ�
        if (PlayerMoveFlag)
        {
            //Debug.Log("�v���C���[�u���b�N�̏�ړ���");
            //Debug.Log(this.gameObject.transform.position.x - OldPos.x);
            PlayerMain.instance.transform.position += 
                new Vector3(this.gameObject.transform.position.x - OldPos.x, this.gameObject.transform.position.y - OldPos.y, 0);

            //PlayerMainScript.addVel = new Vector3(this.gameObject.transform.position.x - OldPos.x, this.gameObject.transform.position.y - OldPos.y, 0) / Time.fixedDeltaTime;
        }

        // �d�I�u�W�F�N�g�̈ړ�
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
