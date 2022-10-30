using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif

//#if UNITY_EDITOR
//[CustomEditor(typeof(MapConverter))]
//public class MapConverterEXEditor : Editor
//{
//    private MapConverter _target;
//    private void OnEnable()
//    {
//        _target = (MapConverter)target;
//    }

//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();

//        GUILayout.Space(20);

//        if (GUILayout.Button("タイルマップ->地形　変換", GUILayout.Height(72)))
//        {
//            _target.ConvertMapTile();
//        }

//        if (GUILayout.Button("生成した地形の削除", GUILayout.Height(72)))
//        {
//            _target.DeleteMap();
//        }
//    }
//}
//#endif