using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapConverter))]
public class MapConverterEXEditor : Editor
{
    private MapConverter _target;
    private void OnEnable()
    {
        _target = (MapConverter)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(20);

        if (GUILayout.Button("�^�C���}�b�v->�n�`�@�ϊ�", GUILayout.Height(72)))
        {
            _target.ConvertMapTile();
        }

        if (GUILayout.Button("���������n�`�̍폜", GUILayout.Height(72)))
        {
            _target.DeleteMap();
        }
    }
}