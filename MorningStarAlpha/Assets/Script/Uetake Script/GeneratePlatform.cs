using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// 全ての子オブジェクトのコライダーを表示
/// </summary>
public class GeneratePlatform : MonoBehaviour
{
    [Tooltip("複製したいオブジェクト")] public GameObject platform;
    [Range(-0.1f, 0.1f), Tooltip("サイズ調整用： 正の数（見た目用オブジェクトが大きめに表示される）負の数（見た目用オブジェクトが小さめに表示される）")] public float AdjustSizeX = 0.0f;
    [Range(-0.1f, 0.1f), Tooltip("サイズ調整用： 正の数（見た目用オブジェクトが大きめに表示される）負の数（見た目用オブジェクトが小さめに表示される）")] public float AdjustSizeY = 0.0f;
    [Range(-0.1f, 0.1f), Tooltip("サイズ調整用： 正の数（見た目用オブジェクトが大きめに表示される）負の数（見た目用オブジェクトが小さめに表示される）")] public float AdjustSizeZ = 0.0f;

#if UNITY_EDITOR
    [CustomEditor(typeof(GeneratePlatform))]
    public class GeneratePlatformEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GeneratePlatform genPlat = target as GeneratePlatform;

            GUILayout.Space(20);
            if (GUILayout.Button("地形の生成", GUILayout.Height(72)))
            {
                GameObject parent = new GameObject(genPlat.name + "_MODELS");
                Collider[] cols = genPlat.GetComponentsInChildren<Collider>();

                foreach (var col in cols)
                {
                    if(col.gameObject == genPlat.gameObject)
                    {
                        continue;
                    }
                    GameObject TestPlat = col.gameObject;

                    //オブジェクト生成と名前変更
                    GameObject obj = Instantiate(genPlat.platform);
                    obj.name = "Model_" + TestPlat.name;
                    //位置合わせ
                    obj.transform.position = TestPlat.transform.position;

                    //サイズ合わせ
                    {
                        Vector3 newScale = Vector3.zero;

                        //もとのモデルデータに依存せずにサイズを1:1:1にする
                        newScale.x = 1.0f / obj.GetComponent<Renderer>().bounds.size.x;
                        newScale.y = 1.0f / obj.GetComponent<Renderer>().bounds.size.y;
                        newScale.z = 1.0f / obj.GetComponent<Renderer>().bounds.size.z;

                        //テスト用足場のサイズに合わせる
                        newScale.x *= TestPlat.transform.localScale.x;
                        newScale.y *= TestPlat.transform.localScale.y;
                        newScale.z *= TestPlat.transform.localScale.z;

                        newScale.x += genPlat.AdjustSizeX;
                        newScale.y += genPlat.AdjustSizeY;
                        newScale.z += genPlat.AdjustSizeZ;

                        obj.transform.localScale = newScale;
                    }

                    ////ピボットズレによる最終位置調整
                    Vector3 adjustPos = obj.GetComponent<Renderer>().bounds.center - col.bounds.center;
                    obj.transform.position -= adjustPos;

                    obj.transform.parent = parent.gameObject.transform;
                }
            }
        }
    }
#endif

}
