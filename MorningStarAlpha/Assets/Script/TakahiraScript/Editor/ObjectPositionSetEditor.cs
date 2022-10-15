using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ObjectPositionSet))]
[CanEditMultipleObjects]
public class ObjectPositionSetEditor : Editor
{
    private ObjectPositionSet[] instances;
    private Vector3 Center = Vector3.zero;

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        instances = targets.Cast<ObjectPositionSet>().ToArray();
    }

    /// <summary>
    /// �V�[���r���[��GUI
    /// </summary>
    private void OnSceneGUI()
    {
        Tools.current = Tool.None;

        Center = GetCenterOfInstances(instances);

        // �t���[�n���h��
        FreeHandle();

        // X��
        AxisHandle(Color.red, Vector2Int.right);

        // Y��
        AxisHandle(Color.green, Vector2Int.up);
    }

    /// <summary>
    /// �t���[�n���h���̕`��
    /// </summary>
    private void FreeHandle()
    {
        Handles.color = Color.magenta;

        // �t���[�ړ��n���h���̍쐬
        EditorGUI.BeginChangeCheck();
        var pos = Handles.FreeMoveHandle(Center, Quaternion.identity, 5f, Vector3.one, Handles.CircleHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            Debug.Log("������");
            MoveObject(pos - Center);
        }
    }

    /// <summary>
    /// �����̃C���X�^���X�̒��S��Ԃ�
    /// </summary>
    private static Vector3 GetCenterOfInstances(ObjectPositionSet[] instances)
    {
        float x = 0f, y = 0f;

        foreach (var ins in instances)
        {
            var position = ins.transform.position;
            x += position.x;
            y += position.y;
        }

        return new Vector3(x / instances.Length, y / instances.Length, 0);
    }

    /// <summary>
    /// ���n���h���̕`��
    /// </summary>
    private void AxisHandle(Color color, Vector2 direction)
    {
        // �n���h���̍쐬
        Handles.color = color;
        EditorGUI.BeginChangeCheck();
        var deltaMovement = Handles.Slider(Center, new Vector3(direction.x, direction.y, 0)) - Center;

        if (EditorGUI.EndChangeCheck())
        {
            var dot = Vector2.Dot(deltaMovement, direction);
            if (!(Mathf.Abs(dot) > Mathf.Epsilon)) return;

            MoveObject(dot * direction);
        }
    }

    /// <summary>
    /// �X�i�b�v���ăI�u�W�F�N�g�𓮂���
    /// </summary>
    private void MoveObject(Vector3 vec3)
    {
        var vec2 = new Vector2Int(Mathf.RoundToInt(vec3.x / ObjectPositionSet.GridSize), Mathf.RoundToInt(vec3.y / ObjectPositionSet.GridSize));

        if (vec2 == Vector2.zero) return;

        foreach (var ins in instances)
        {
            Object[] objects = { ins, ins.transform };
            Undo.RecordObjects(objects, "�I�u�W�F�N�g�̈ړ�");
            ins.Move(vec2);
        }
    }
}

#endif
