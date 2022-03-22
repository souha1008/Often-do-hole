using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


// Gimmick_Enemy�N���X���g��
[CustomEditor(typeof(Gimmick_Enemy))]

// �����I��L��
[CanEditMultipleObjects]

public class Gimmick_EnemyEditor : Editor
{
    // �V���A���C�Y�I�u�W�F�N�g
    SerializedProperty MoveInfo;
    SerializedProperty Move_X, Move_Y;
    SerializedProperty MoveDirection_X, MoveDirection_Y;
    SerializedProperty MoveLength_X, MoveLength_Y;
    SerializedProperty MoveTime_X1, MoveTime_X2, MoveTime_Y1, MoveTime_Y2;
    SerializedProperty StartTime_X, StartTime_Y;
    SerializedProperty EasingType_X1, EasingType_X2, EasingType_Y1, EasingType_Y2;

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
        MoveTime_X1 = serializedObject.FindProperty("MoveTime_X1");
        MoveTime_Y1 = serializedObject.FindProperty("MoveTime_Y1");
        MoveTime_X2 = serializedObject.FindProperty("MoveTime_X2");
        MoveTime_Y2 = serializedObject.FindProperty("MoveTime_Y2");
        StartTime_X = serializedObject.FindProperty("StartTime_X");
        StartTime_Y = serializedObject.FindProperty("StartTime_Y");
        EasingType_X1 = serializedObject.FindProperty("EasingType_X1");
        EasingType_Y1 = serializedObject.FindProperty("EasingType_Y1");
        EasingType_X2 = serializedObject.FindProperty("EasingType_X2");
        EasingType_Y2 = serializedObject.FindProperty("EasingType_Y2");
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
            MoveTime_X1.floatValue = EditorGUILayout.FloatField("���b�����Ĉړ����邩[�s��]", MoveTime_X1.floatValue);
            MoveTime_X2.floatValue = EditorGUILayout.FloatField("���b�����Ĉړ����邩[�A��]", MoveTime_X2.floatValue);
            StartTime_X.floatValue = EditorGUILayout.Slider("�����o�ߎ���", StartTime_X.floatValue, 0, MoveTime_X1.floatValue);    // �X���C�_�[��\���i�����́u�����l,�ŏ��l,�ő�l�v�j
            EasingType_X1.enumValueIndex =
                EditorGUILayout.Popup("�C�[�W���O(������)[�s��]", EasingType_X1.enumValueIndex, 
                new string[] 
                {   "LINEAR",
                    "QUAD_IN", "QUAD_OUT", "QUAD_INOUT", 
                    "CUBIC_IN", "CUBIC_OUT", "CUBIC_INOUT", 
                    "QUART_IN", "QUART_OUT", "QUART_INOUT",
                    "QUINT_IN", "QUINT_OUT", "QUINT_INOUT",
                    "SINE_IN", "SINE_OUT", "SINE_INOUT", 
                    "EXP_IN", "EXP_OUT", "EXP_INOUT",
                    "CIRC_IN", "CIRC_OUT", "CIRC_INOUT",
                    "ELASTIC_IN", "ELASTIC_OUT", "ELASTIC_INOUT",
                    "BACK_IN", "BACK_OUT", "BACK_INOUT",
                    "BOUNCE_IN", "BOUNCE_OUT", "BOUNCE_INOUT"
                });
            EasingType_X2.enumValueIndex =
                EditorGUILayout.Popup("�C�[�W���O(������)[�A��]", EasingType_X2.enumValueIndex,
                new string[]
                {   "LINEAR",
                    "QUAD_IN", "QUAD_OUT", "QUAD_INOUT",
                    "CUBIC_IN", "CUBIC_OUT", "CUBIC_INOUT",
                    "QUART_IN", "QUART_OUT", "QUART_INOUT",
                    "QUINT_IN", "QUINT_OUT", "QUINT_INOUT",
                    "SINE_IN", "SINE_OUT", "SINE_INOUT",
                    "EXP_IN", "EXP_OUT", "EXP_INOUT",
                    "CIRC_IN", "CIRC_OUT", "CIRC_INOUT",
                    "ELASTIC_IN", "ELASTIC_OUT", "ELASTIC_INOUT",
                    "BACK_IN", "BACK_OUT", "BACK_INOUT",
                    "BOUNCE_IN", "BOUNCE_OUT", "BOUNCE_INOUT"
                });
        }
        EditorGUILayout.EndToggleGroup();   // �O���[�v��


        Move_Y.boolValue = EditorGUILayout.BeginToggleGroup("Y�����ړ�", Move_Y.boolValue); // �O���[�v���n�܂�
        if (Move_Y.boolValue)
        {
            MoveDirection_Y.enumValueIndex =
            EditorGUILayout.Popup("�ړ�����", MoveDirection_Y.enumValueIndex, new string[] { "��ړ�", "���ړ�" });
            MoveLength_Y.floatValue = EditorGUILayout.FloatField("�ړ�����", MoveLength_Y.floatValue);
            MoveTime_Y1.floatValue = EditorGUILayout.FloatField("���b�����Ĉړ����邩[�s��]", MoveTime_Y1.floatValue);
            MoveTime_Y2.floatValue = EditorGUILayout.FloatField("���b�����Ĉړ����邩[�A��]", MoveTime_Y2.floatValue);
            StartTime_Y.floatValue = EditorGUILayout.Slider("�����o�ߎ���", StartTime_Y.floatValue, 0, MoveTime_Y1.floatValue);    // �X���C�_�[��\���i�����́u�����l,�ŏ��l,�ő�l�v�j
            EasingType_Y1.enumValueIndex =
                EditorGUILayout.Popup("�C�[�W���O(������)[�s��]", EasingType_Y1.enumValueIndex,
                new string[]
                {   "LINEAR",
                    "QUAD_IN", "QUAD_OUT", "QUAD_INOUT",
                    "CUBIC_IN", "CUBIC_OUT", "CUBIC_INOUT",
                    "QUART_IN", "QUART_OUT", "QUART_INOUT",
                    "QUINT_IN", "QUINT_OUT", "QUINT_INOUT",
                    "SINE_IN", "SINE_OUT", "SINE_INOUT",
                    "EXP_IN", "EXP_OUT", "EXP_INOUT",
                    "CIRC_IN", "CIRC_OUT", "CIRC_INOUT",
                    "ELASTIC_IN", "ELASTIC_OUT", "ELASTIC_INOUT",
                    "BACK_IN", "BACK_OUT", "BACK_INOUT",
                    "BOUNCE_IN", "BOUNCE_OUT", "BOUNCE_INOUT"
                });
            EasingType_Y2.enumValueIndex =
                EditorGUILayout.Popup("�C�[�W���O(������)[�A��]", EasingType_Y2.enumValueIndex,
                new string[]
                {   "LINEAR",
                    "QUAD_IN", "QUAD_OUT", "QUAD_INOUT",
                    "CUBIC_IN", "CUBIC_OUT", "CUBIC_INOUT",
                    "QUART_IN", "QUART_OUT", "QUART_INOUT",
                    "QUINT_IN", "QUINT_OUT", "QUINT_INOUT",
                    "SINE_IN", "SINE_OUT", "SINE_INOUT",
                    "EXP_IN", "EXP_OUT", "EXP_INOUT",
                    "CIRC_IN", "CIRC_OUT", "CIRC_INOUT",
                    "ELASTIC_IN", "ELASTIC_OUT", "ELASTIC_INOUT",
                    "BACK_IN", "BACK_OUT", "BACK_INOUT",
                    "BOUNCE_IN", "BOUNCE_OUT", "BOUNCE_INOUT"
                });
        }
        EditorGUILayout.EndToggleGroup();   // �O���[�v��

        // �����L���b�V���ɒl��ۑ�����
        serializedObject.ApplyModifiedProperties();
    }
}
#endif

public class Gimmick_Enemy : Gimmick_Main
{
    // �ϐ�
    public EASING_TYPE EasingType_X1, EasingType_X2;        // �C�[�W���O�^�C�v
    public EASING_TYPE EasingType_Y1, EasingType_Y2;        // �C�[�W���O�^�C�v

    public float MoveLength_X = 15.0f;          // ��������
    public float MoveLength_Y = 15.0f;          // ��������
    public float MoveTime_X1, MoveTime_X2 = 2.0f; // ���b�����Ĉړ����邩
    public float MoveTime_Y1, MoveTime_Y2 = 2.0f; // ���b�����Ĉړ����邩
    public float StartTime_X;                   // �����o�ߎ���
    public float StartTime_Y;                   // �����o�ߎ���
    public MOVE_DIRECTION_X MoveDirection_X;    // �ړ�����X
    public MOVE_DIRECTION_Y MoveDirection_Y;    // �ړ�����Y

    public bool Move_X;                         // X�����ړ����g���Ă��邩
    public bool Move_Y;                         // Y�����ړ����g���Ă��邩

    private bool MoveRight, MoveUp;             // �ړ�����(�E)(��)
    private bool StartMoveRight, StartMoveUp;   // �����ړ�����(�E)(��)
    private bool NowMove_X, NowMove_Y;          // �ړ�����
    private float NowTime_X, NowTime_Y;         // �o�ߎ���
    private float StartPos_X, StartPos_Y;       // �������W
    private float Fugou_X, Fugou_Y;             // ����

    private Vector3 OldPos;                     // �ЂƂO�̍��W�m�F�p

    // �X�^�[�g����
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
        StartMoveRight = MoveRight;
        StartMoveUp = MoveUp;
        OldPos = this.gameObject.transform.position;
    }

    // �G�̓�������
    public override void FixedMove()
    {
        OldPos = this.gameObject.transform.position;
        if (Move_X) // X�����ړ����g�p����Ă�����
        {
            if (NowMove_X)
            {
                // �ړ�����
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

                NowTime_X += Time.fixedDeltaTime; // ���ԉ��Z

                // �ړ��I��
                if (NowTime_X > MoveTimeX)
                    NowMove_X = false;
            }
            else
            {
                StartPos_X = this.gameObject.transform.position.x;  // �������W�Z�b�g
                NowTime_X = 0.0f; // ���ԃ��Z�b�g
                MoveRight = !MoveRight; // �������]
                Fugou_X = CalculationScript.FugouChange(MoveRight);   // �������]
                MoveDirection_X = BoolMoveDirectionChangeX(MoveRight); // �����\���ω�
                NowMove_X = true; // ����
            }
        }

        if (Move_Y) // Y�����ړ����g�p����Ă�����
        {
            if (NowMove_Y)
            {
                // �ړ�����
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

                NowTime_Y += Time.fixedDeltaTime; // ���ԉ��Z

                // �ړ��I��
                if (NowTime_Y > MoveTimeY)
                    NowMove_Y = false;
            }
            else
            {
                StartPos_Y = this.gameObject.transform.position.y; // �������W�Z�b�g
                NowTime_Y = 0.0f; // ���ԃ��Z�b�g
                MoveUp = !MoveUp; // �������]
                Fugou_Y = CalculationScript.FugouChange(MoveUp);   // �������]
                MoveDirection_Y = BoolMoveDirectionChangeY(MoveUp); // �����\���ω�
                NowMove_Y = true; // ����
            }
        }

        Rad += new Vector3(0.0f, 0.0f, 10.0f);  // ��]
    }

    // �G���S����
    public override void Death()
    {
        // �����S�G�t�F�N�g

        // ���g������
        Destroy(this.gameObject);
    }

    // �����ƏՓˏ���(�g���K�[)
    public override void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Bullet")
        {
            // �q�b�g�X�g�b�v
            GameSpeedManager.Instance.StartHitStop();
            // �����������G�t�F�N�g

            Death(); // ���S����   
        }

        if (collider.gameObject.tag == "Player")
        {
            // �v���C���[�m�b�N�o�b�N�X�e�[�g�Ɉڍs

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
