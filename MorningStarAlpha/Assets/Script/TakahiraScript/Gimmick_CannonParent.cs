using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

// Gimmick_CannonParent�N���X���g��
[CustomEditor(typeof(Gimmick_CannonParent))]

// �����I��L��
//[CanEditMultipleObjects]

public class Gimmick_CannonParentEditor : Editor
{
    // �V���A���C�Y�I�u�W�F�N�g
    SerializedProperty MoveInfo;
    SerializedProperty CannonChild, StartLength, ShootTime;
    SerializedProperty FixedRadFlag, ChaseFlag, Speed, LifeTime;
    SerializedProperty WayRight;

    void OnEnable()
    {
        // �V���A���C�Y�I�u�W�F�N�g���擾
        MoveInfo = serializedObject.FindProperty("MoveInfo");
        MoveInfo.isExpanded = false; // �h���b�v�_�E���ŏ�����߂Ƃ�

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
        // �����L���b�V������l�����[�h����
        serializedObject.Update();

        // PrayerMain�̈ړ����
        EditorGUILayout.PropertyField(MoveInfo, new GUIContent("���݂̈ړ����"));    // �ړ������\��
        EditorGUILayout.Space(5);


        // �C��̐ݒ�
        EditorGUILayout.LabelField("[�C��̐ݒ�]");
        EditorGUILayout.Space(5);

        FixedRadFlag.boolValue = EditorGUILayout.Toggle("�C��̊p�x�Œ�", FixedRadFlag.boolValue);
        CannonChild.objectReferenceValue = EditorGUILayout.ObjectField("�ł��o���e�I�u�W�F�N�g", CannonChild.objectReferenceValue, typeof(GameObject), true);
        StartLength.floatValue = EditorGUILayout.FloatField("�����o���v���C���[�Ƃ̋���", StartLength.floatValue);
        ShootTime.floatValue = EditorGUILayout.FloatField("�ł��o���Ԋu�̎���", ShootTime.floatValue);
        EditorGUILayout.Space(5);

        // �e�̐ݒ�
        EditorGUILayout.LabelField("[�e�̐ݒ�]");
        EditorGUILayout.Space(5);

        

        if (!FixedRadFlag.boolValue) // �C��Œ�łȂ��Ȃ�
            ChaseFlag.boolValue = EditorGUILayout.Toggle("�e���ǔ����邩", ChaseFlag.boolValue);
        else
            ChaseFlag.boolValue = false;

        Speed.floatValue = EditorGUILayout.FloatField("�e�̑��x", Speed.floatValue);
        LifeTime.floatValue = EditorGUILayout.FloatField("�e�̐�������", LifeTime.floatValue);
        EditorGUILayout.Space(5);

        // ����
        EditorGUILayout.LabelField("[����]");
        EditorGUILayout.Space(5);

        WayRight.boolValue = EditorGUILayout.Toggle("�E������", WayRight.boolValue);


        // �����L���b�V���ɒl��ۑ�����
        serializedObject.ApplyModifiedProperties();
    }
}
#endif

public class Gimmick_CannonParent : Gimmick_Main
{
    public GameObject CannonChild;  // �C�䂩��ł��o���e�I�u�W�F�N�g
    public float StartLength = 15;  // �C�䂪�����o���v���C���[�Ƃ̋���
    public float ShootTime = 5;     // �ł��o���Ԋu

    public bool FixedRadFlag;       // �p�x�̌Œ�
    public bool ChaseFlag;          // �e���ǔ����邩
    public float Speed = 5;         // �e�̑��x
    public float LifeTime = 5;      // �e�̐�������

    private const float Length = 7;        // �e�̏o���ʒu�����p

    public bool WayRight;           // �T���̌���

    private Animator animator;

    private float NowRotateZ;   // ��]Z
    private bool StartFlag;     // �N���t���O
    private float NowShootTime; // �o�ߎ���

    // �e�̃v�[��
    private static int BulletNum = 6;   // �T���̒e�ő��
    private Gimmick_CannonChild[] Bullet = new Gimmick_CannonChild[BulletNum];

    public override void Init()
    {
        // ������
        StartFlag = false;
        NowShootTime = ShootTime; // �ŏ���1��e����
        //NowShootTime = 0; // �ŏ���1��e���˂��Ȃ�

        // �R���C�_�[
        Cd.isTrigger = false;

        // ���W�b�h�{�f�B
        Rb.isKinematic = true;

        animator = GetComponent<Animator>();

        // ���������킹��
        if (WayRight)
        {
            //this.gameObject.transform.rotation = Quaternion.identity;
            NowRotateZ = 0;
        }
        else
        {
            //this.gameObject.transform.rotation = Quaternion.Euler(0, 180.0f, 0);
            NowRotateZ = 180;
        }
    }

    public override void UpdateMove()
    {
        // �v���C���[�̍��W
        Vector3 PlayerPos = PlayerMain.instance.transform.position;
        Vector3 ThisPos = this.gameObject.transform.position;


        // �v���C���[�Ƃ̋������m�F
        if (Vector2.Distance(PlayerPos, ThisPos) <= StartLength)
            StartFlag = true;
        else
            StartFlag = false;


        // �N�����̏���
        if (StartFlag)
        {
            if (!FixedRadFlag) // �C��Œ�łȂ��Ȃ�
            {
                float Rot = CalculationScript.UnityTwoPointAngle360(ThisPos, PlayerPos);
                if (WayRight)
                {
                    if ((Rot >= 0 && Rot <= 90) || (Rot >= 270 && Rot <= 360))
                    {
                        // �v���C���[�̕����Ɍ���
                        this.gameObject.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Rot);
                        NowRotateZ = Rot;
                    }
                }
                else
                {
                    if ((Rot >= 90 && Rot <= 270))
                    {
                        // �v���C���[�̕����Ɍ���
                        this.gameObject.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Rot + 180);
                        NowRotateZ = Rot;
                    }
                }
            }

            

            // �e���ˏ���
            if (NowShootTime >= ShootTime)
            {
                animator.Play("05_shark_motion_02", 0);  // �A�j���[�V�����Đ�
                //Shoot(); // �A�j���[�^�[���ŏ����𓮂����Ă���(�A�j���[�V�����C�x���g)
                NowShootTime = 0.0f; // �o�ߎ��ԃ��Z�b�g
            }
            else
                NowShootTime += Time.deltaTime; // �o�ߎ��ԉ��Z
        }
    }

    // �e����
    public void Shoot()
    {
        if (CannonChild != null)
        {
            VecQuaternion vecQuaternion;
            if (WayRight)
            {
                vecQuaternion =
                    CalculationScript.PointRotate(gameObject.transform.position, gameObject.transform.position + new Vector3(Length, -1.0f, 0), NowRotateZ, Vector3.forward);
            }
            else
            {
                vecQuaternion =
                    CalculationScript.PointRotate(gameObject.transform.position, gameObject.transform.position + new Vector3(Length, 1.0f, 0), NowRotateZ, Vector3.forward);
            }

            Gimmick_CannonChild Child = null;

            for (int i = 0; i < BulletNum; i++)
            {
                if (Bullet[i] == null)
                {
                    Bullet[i] = Instantiate(CannonChild, vecQuaternion.Pos, Quaternion.Euler(0, 0, NowRotateZ)).GetComponent<Gimmick_CannonChild>(); // �e����
                    Child = Bullet[i];
                    Debug.LogWarning("����");
                    break;
                }
                else if (!Bullet[i].gameObject.activeSelf)
                {
                    Child = Bullet[i];
                    Debug.LogWarning("�ė��p");
                    break;
                }
            }


            // �e�̍ő�ʃ`�F�b�N
            if (Child == null)
            {
                Debug.LogWarning("�T���̒e�̍ő�ʂ𒴂��܂����B�ő�ʂ���葽���ݒ肵�ĉ������B");
            }
            else
            {
                Child.SetCannonChild(Speed, LifeTime, ChaseFlag, vecQuaternion.Pos, Quaternion.Euler(0, 0, NowRotateZ)); // �e�̒l�Z�b�g
                Child.gameObject.SetActive(true);
            }          
        }
    }

    public override void Death()
    {
        // �������g������
        Destroy(this.gameObject);
    }

    public override void OnCollisionEnter(Collision collision)
    {
        // �v���C���[�ƐڐG���R�C���擾
        if (collision.gameObject.CompareTag("Player"))
        {
            // �q�b�g�X�g�b�v
            GameSpeedManager.Instance.StartHitStop(0.1f);
            // �v���C���[���m�b�N�o�b�N
            PlayerMain.instance.mode = new PlayerState_Knockback(this.transform.position, false);
        }
    }
}
