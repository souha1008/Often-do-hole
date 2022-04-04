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

    void OnEnable()
    {
        // �V���A���C�Y�I�u�W�F�N�g���擾
        MoveInfo = serializedObject.FindProperty("MoveInfo");
        MoveInfo.isExpanded = false; // �h���b�v�_�E���ŏ�����߂Ƃ�
    }
    public override void OnInspectorGUI()
    {
        // �����L���b�V������l�����[�h����
        serializedObject.Update();
  

        // �C�[�W���O�^�C�v�p�̃e�L�X�g
        string[] EasingTypeText = new string[]
                {
                    "�����ړ�",
                    "����(�Ŏ�)_IN", "����(�Ŏ�)_OUT", "����(�Ŏ�)_INOUT",
                    "����(��)_IN", "����(��)_OUT", "����(��)_INOUT",
                    "����(��)_IN", "����(��)_OUT", "����(��)_INOUT",
                    "����(��)_IN", "����(��)_OUT", "����(��)_INOUT",
                    "���ӂ�_IN", "���ӂ�_OUT", "���ӂ�_INOUT",
                    "�L��������_IN", "�L��������_OUT", "�L��������_INOUT",
                    "�������_IN", "�������_OUT", "�������_INOUT",
                    "�S���ړ�_IN", "�S���ړ�_OUT", "�S���ړ�_INOUT",
                    "�����s���߂�_IN", "�����s���߂�_OUT", "�����s���߂�_INOUT",
                    "�o�E���h_IN", "�o�E���h_OUT", "�o�E���h_INOUT"
                };

        Gimmick_Enemy Enemy = target as Gimmick_Enemy;

        // �G�f�B�^�[�̕ύX�m�F
        EditorGUI.BeginChangeCheck(); 

        // PrayerMain�̈ړ����
        EditorGUILayout.PropertyField(MoveInfo, new GUIContent("���݂̈ړ����"));    // �ړ������\��


        // X�EY�����ړ�
        Enemy.Move_X = EditorGUILayout.BeginToggleGroup("X�����ړ�", Enemy.Move_X); // �O���[�v���n�܂�
        if (Enemy.Move_X)
        {
            Enemy.MoveDirection_X=
                (MOVE_DIRECTION_X)EditorGUILayout.Popup("�ړ�����", (int)Enemy.MoveDirection_X, new string[] { "�E�ړ�", "���ړ�" });
            Enemy.MoveLength_X = EditorGUILayout.FloatField("�ړ�����", Enemy.MoveLength_X);
            Enemy.MoveTime_X1 = EditorGUILayout.FloatField("���b�����Ĉړ����邩[�s��]", Enemy.MoveTime_X1);
            Enemy.MoveTime_X2 = EditorGUILayout.FloatField("���b�����Ĉړ����邩[�A��]", Enemy.MoveTime_X2);
            Enemy.StartTime_X = EditorGUILayout.Slider("�����o�ߎ���", Enemy.StartTime_X, 0, Enemy.MoveTime_X1);    // �X���C�_�[��\���i�����́u�����l,�ŏ��l,�ő�l�v�j
            Enemy.EasingType_X1 =
                (EASING_TYPE)EditorGUILayout.Popup("�C�[�W���O(������)[�s��]", (int)Enemy.EasingType_X1, EasingTypeText);
            Enemy.EasingType_X2 =
                (EASING_TYPE)EditorGUILayout.Popup("�C�[�W���O(������)[�A��]", (int)Enemy.EasingType_X2, EasingTypeText);
        }
        EditorGUILayout.EndToggleGroup();   // �O���[�v��


        Enemy.Move_Y = EditorGUILayout.BeginToggleGroup("Y�����ړ�", Enemy.Move_Y); // �O���[�v���n�܂�
        if (Enemy.Move_Y)
        {
            Enemy.MoveDirection_Y =
                (MOVE_DIRECTION_Y)EditorGUILayout.Popup("�ړ�����", (int)Enemy.MoveDirection_Y, new string[] { "��ړ�", "���ړ�" });
            Enemy.MoveLength_Y = EditorGUILayout.FloatField("�ړ�����", Enemy.MoveLength_Y);
            Enemy.MoveTime_Y1 = EditorGUILayout.FloatField("���b�����Ĉړ����邩[�s��]", Enemy.MoveTime_Y1);
            Enemy.MoveTime_Y2 = EditorGUILayout.FloatField("���b�����Ĉړ����邩[�A��]", Enemy.MoveTime_Y2);
            Enemy.StartTime_Y = EditorGUILayout.Slider("�����o�ߎ���", Enemy.StartTime_Y, 0, Enemy.MoveTime_Y1);    // �X���C�_�[��\���i�����́u�����l,�ŏ��l,�ő�l�v�j
            Enemy.EasingType_Y1 =
                (EASING_TYPE)EditorGUILayout.Popup("�C�[�W���O(������)[�s��]", (int)Enemy.EasingType_Y1, EasingTypeText);
            Enemy.EasingType_Y2 =
                (EASING_TYPE)EditorGUILayout.Popup("�C�[�W���O(������)[�A��]", (int)Enemy.EasingType_Y2, EasingTypeText);
        }
        EditorGUILayout.EndToggleGroup();   // �O���[�v��

        // �G�f�B�^�[�̕ύX�m�F
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target); // �I���I�u�W�F�N�g�X�V

            //�I������Ă���S�Ă��̂ɂ��ď���
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

        // �����L���b�V���ɒl��ۑ�����
        //serializedObject.ApplyModifiedProperties();
    }
}
#endif

public class Gimmick_Enemy : Gimmick_Main
{
    // �ϐ�
    public EASING_TYPE EasingType_X1, EasingType_X2;        // �C�[�W���O�^�C�v
    public EASING_TYPE EasingType_Y1, EasingType_Y2;        // �C�[�W���O�^�C�v

    public float MoveLength_X = 8.0f;          // ��������
    public float MoveLength_Y = 8.0f;          // ��������
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

        transform.rotation *= Quaternion.Euler(0, 0, 10);  // ��]
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
