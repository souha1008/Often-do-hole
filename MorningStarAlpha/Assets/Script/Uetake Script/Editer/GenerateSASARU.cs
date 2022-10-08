using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// 全ての子オブジェクトのコライダーを表示
/// </summary>
public class GenerateSASARU : MonoBehaviour
{
    [Tooltip("複製したいオブジェクト")] public GameObject platform;
    [Range(-0.1f, 0.1f), Tooltip("サイズ調整用： 正の数（見た目用オブジェクトが大きめに表示される）負の数（見た目用オブジェクトが小さめに表示される）")] public float AdjustSizeX = 0.0f;
    [Range(-0.1f, 0.1f), Tooltip("サイズ調整用： 正の数（見た目用オブジェクトが大きめに表示される）負の数（見た目用オブジェクトが小さめに表示される）")] public float AdjustSizeY = 0.0f;
    [Range(-0.1f, 1.0f), Tooltip("サイズ調整用： 正の数（見た目用オブジェクトが大きめに表示される）負の数（見た目用オブジェクトが小さめに表示される）")] public float AdjustSizeZ = 0.0f;

#if UNITY_EDITOR
    [CustomEditor(typeof(GenerateSASARU))]
    public class GeneratePlatformEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GenerateSASARU genPlat = target as GenerateSASARU;

            GUILayout.Space(20);
            if (GUILayout.Button("地形の生成", GUILayout.Height(72)))
            {
                //元あるものは削除
                DeleteChildObject();

                   
                GenerateGround();
            }

            if (GUILayout.Button("地形の削除", GUILayout.Height(72)))
            {
                DeleteChildObject();
            }

            void GenerateGround()
            {
                //真ん中の白い部分
                {
                    GameObject obj = Instantiate(genPlat.platform);
                    //位置合わせ
                    obj.transform.position = genPlat.transform.position;

                    //サイズ合わせ
                    {
                        Vector3 newScale = Vector3.zero;

                        //もとのモデルデータに依存せずにサイズを1:1:1にする
                        newScale.x = 1.0f / obj.GetComponent<Renderer>().bounds.size.x;
                        newScale.y = 1.0f / obj.GetComponent<Renderer>().bounds.size.y;
                        newScale.z = 1.0f / obj.GetComponent<Renderer>().bounds.size.z;

                        //テスト用足場のサイズに合わせる
                        newScale.x *= genPlat.transform.localScale.x;
                        newScale.y *= genPlat.transform.localScale.y;
                        newScale.z *= genPlat.transform.localScale.z;

                        newScale.x += genPlat.AdjustSizeX;
                        newScale.y += genPlat.AdjustSizeY;
                        newScale.z += genPlat.AdjustSizeZ;

                        obj.transform.localScale = newScale;
                    }

                    obj.transform.parent = genPlat.transform;

                    ////ピボットズレによる最終位置調整
                    Vector3 adjustPos = obj.GetComponent<Renderer>().bounds.center - genPlat.GetComponent<Collider>().bounds.center;
                    obj.transform.position -= adjustPos;
                }
            }

            void DeleteChildObject()
            {
                Debug.Log(genPlat.transform.childCount);

                for (int i = genPlat.transform.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(genPlat.transform.GetChild(i).gameObject);    
                }
            }
        }
    }
#endif

}
