using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

// Gimmick_CannonParent�N���X���g��
[CustomEditor(typeof(Gimmick_CannonParent))]

// �����I��L��
[CanEditMultipleObjects]

public class Gimmick_CannonParentEditor : Editor
{
    // �V���A���C�Y�I�u�W�F�N�g
    SerializedProperty MoveInfo;
    SerializedProperty CannonChild, StartLength, ShootTime;
    SerializedProperty FixedRadFlag, ChaseFlag, Speed, LifeTime;

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


        // �����L���b�V���ɒl��ۑ�����
        serializedObject.ApplyModifiedProperties();
    }
}
#endif

public class Gimmick_CannonParent : Gimmick_Main
{
    [Header("[�C��̐ݒ�]")]
    [Label("�ł��o���e�I�u�W�F�N�g")]
    public GameObject CannonChild;  // �C�䂩��ł��o���e�I�u�W�F�N�g
    [Label("�����o���v���C���[�Ƃ̋���")]
    public float StartLength = 15;  // �C�䂪�����o���v���C���[�Ƃ̋���
    [Label("�ł��o���Ԋu�̎���")]
    public float ShootTime = 5;     // �ł��o���Ԋu

    [Header("[�e�̐ݒ�]")]
    [Label("�C��̊p�x�Œ�")]
    public bool FixedRadFlag;       // �p�x�̌Œ�
    [Label("�e���ǔ����邩")]
    public bool ChaseFlag;          // �e���ǔ����邩
    [Label("�e�̑��x")]
    public float Speed = 5;         // �e�̑��x
    [Label("�e�̐�������")]
    public float LifeTime = 5;      // �e�̐�������



    private bool StartFlag;     // �N���t���O
    private float NowShootTime; // �o�ߎ���

    [HideInInspector] public GameObject PlayerObject; // �v���C���[�I�u�W�F�N�g
    

    public override void Init()
    {
        // ������
        StartFlag = false;
        NowShootTime = ShootTime; // �ŏ���1��e����

        // �v���C���[�I�u�W�F�N�g�擾
        PlayerObject = GameObject.Find("Player");
    }

    public override void UpdateMove()
    {
        // �v���C���[�̍��W
        Vector3 PlayerPos = PlayerObject.transform.position;
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
                // �v���C���[�̕����Ɍ���
                Rad.z = CalculationScript.UnityTwoPointAngle360(ThisPos, PlayerPos);
            }

            // �e���ˏ���
            if (NowShootTime >= ShootTime)
            {
                Shoot();
                NowShootTime = 0.0f; // �o�ߎ��ԃ��Z�b�g
            }

            NowShootTime += Time.deltaTime; // �o�ߎ��ԉ��Z
        }
    }

    // �e����
    public void Shoot()
    {
        if (CannonChild != null)
        {
            GameObject Child = Instantiate(CannonChild, gameObject.transform.position, Quaternion.Euler(Rad)); // �e����

            Child.GetComponent<Gimmick_CannonChild>().SetCannonChild(PlayerObject, Speed, LifeTime, ChaseFlag); // �e�̒l�Z�b�g
        }
    }

    public override void Death()
    {
        // �������g������
        Destroy(this.gameObject);
    }
}
