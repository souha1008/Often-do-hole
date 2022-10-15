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
    /// シーンビューのGUI
    /// </summary>
    private void OnSceneGUI()
    {
        Tools.current = Tool.None;

        Center = GetCenterOfInstances(instances);

        // フリーハンドル
        FreeHandle();

        // X軸
        AxisHandle(Color.red, Vector2Int.right);

        // Y軸
        AxisHandle(Color.green, Vector2Int.up);
    }

    /// <summary>
    /// フリーハンドルの描画
    /// </summary>
    private void FreeHandle()
    {
        Handles.color = Color.magenta;

        // フリー移動ハンドルの作成
        EditorGUI.BeginChangeCheck();
        var pos = Handles.FreeMoveHandle(Center, Quaternion.identity, 5f, Vector3.one, Handles.CircleHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            Debug.Log("動いた");
            MoveObject(pos - Center);
        }
    }

    /// <summary>
    /// 複数のインスタンスの中心を返す
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
    /// 軸ハンドルの描画
    /// </summary>
    private void AxisHandle(Color color, Vector2 direction)
    {
        // ハンドルの作成
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
    /// スナップしてオブジェクトを動かす
    /// </summary>
    private void MoveObject(Vector3 vec3)
    {
        var vec2 = new Vector2Int(Mathf.RoundToInt(vec3.x / ObjectPositionSet.GridSize), Mathf.RoundToInt(vec3.y / ObjectPositionSet.GridSize));

        if (vec2 == Vector2.zero) return;

        foreach (var ins in instances)
        {
            Object[] objects = { ins, ins.transform };
            Undo.RecordObjects(objects, "オブジェクトの移動");
            ins.Move(vec2);
        }
    }
}

#endif
