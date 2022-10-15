using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// 全ての子オブジェクトのコライダーを表示
/// </summary>
public class GenerateSASARANAI : MonoBehaviour
{
    const float GridSize = 3.2f;

    public enum Direction
    {
        UP,
        DOWN,
    }

    [Tooltip("複製したいオブジェクト")] public GameObject ground;
    [Tooltip("道路の角オブジェクト")] public GameObject corner;
    [Tooltip("道路の角オブジェクト")] public GameObject flat;

    public bool Road_Top = true;
    public bool Road_Bottom = true;
    public bool Road_Left = true;
    public bool Road_Light = true;

    [Tooltip("真ん中の白い部分の向き")] public Direction direction;

    [Range(-0.1f, 0.1f), Tooltip("サイズ調整用： 正の数（真ん中の白い部分が大きめに表示される）負の数（真ん中の白い部分が小さめに表示される）")] public float AdjustSizeX = -0.5f;
    [Range(-0.1f, 0.1f), Tooltip("サイズ調整用： 正の数（真ん中の白い部分が大きめに表示される）負の数（真ん中の白い部分が小さめに表示される）")] public float AdjustSizeY = -0.5f;
    [Range(0.5f, 1.0f), Tooltip("サイズ調整用： 正の数（真ん中の白い部分が大きめに表示される）負の数（真ん中の白い部分が小さめに表示される）")] public float AdjustSizeZ = 0.0f;

#if UNITY_EDITOR
    [CustomEditor(typeof(GenerateSASARANAI))]
    public class GeneratePlatformEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GenerateSASARANAI genPlat = target as GenerateSASARANAI;

            
            GUILayout.Space(20);
            if (GUILayout.Button("地形の生成", GUILayout.Height(72)))
            {
                //元あるものは削除
                DeleteChildObject();
                GenerateRoad();
                GenerateGround();

                genPlat.GetComponent<Renderer>().enabled = false;
            }
            if (GUILayout.Button("削除", GUILayout.Height(72)))
            {
                DeleteChildObject();
                genPlat.GetComponent<Renderer>().enabled = true;
            }



            void GenerateGround()
            {
                //真ん中の白い部分
                {
                    GameObject obj = Instantiate(genPlat.ground);
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

                    if(genPlat.direction == Direction.DOWN)
                    {
                        obj.transform.rotation = Quaternion.Euler(0, 0, 180);
                    }

                    obj.transform.parent = genPlat.transform;

                    ////ピボットズレによる最終位置調整
                    Vector3 adjustPos = obj.GetComponent<Renderer>().bounds.center - genPlat.GetComponent<Collider>().bounds.center;
                    obj.transform.position -= adjustPos;
                }
            }

            void GenerateRoad()
            {
                Bounds bounds = genPlat.GetComponent<Renderer>().bounds;

                //マスが縦横それぞれいくつ連続しているかを調べる
                int x_lne = Mathf.RoundToInt(Mathf.Abs(genPlat.transform.lossyScale.x) / GridSize);
                int y_lne = Mathf.RoundToInt(Mathf.Abs(genPlat.transform.lossyScale.y) / GridSize);

                Debug.Log("Xlne = " + x_lne + "  : Ylen = " + y_lne);

                //最大値と最小値を求める
                Vector3 maxPos = bounds.max;
                maxPos.z = bounds.center.z;

                Vector3 minPos = bounds.min;
                minPos.z = bounds.center.z;

                //角の生成
                {
                    //左上
                    if (genPlat.Road_Top || genPlat.Road_Left)
                    {
                        GameObject obj = InstantiateCorner();
                        obj.transform.position = new Vector3(minPos.x - 0.2f, maxPos.y, minPos.z);
                        obj.transform.rotation = Quaternion.Euler(0, 0, 90);
                        obj.transform.parent = genPlat.transform;
                    }

                    //右上
                    if (genPlat.Road_Top || genPlat.Road_Light)
                    {
                        GameObject obj = InstantiateCorner();
                        obj.transform.position = new Vector3(maxPos.x, maxPos.y + 0.2f, minPos.z); ;
                        obj.transform.parent = genPlat.transform;
                    }

                    //左下
                    if (genPlat.Road_Bottom || genPlat.Road_Left)
                    {
                        GameObject obj = InstantiateCorner();
                        obj.transform.position = new Vector3(minPos.x, minPos.y - 0.2f, minPos.z);
                        obj.transform.rotation = Quaternion.Euler(0, 0, 180);
                        obj.transform.parent = genPlat.transform;
                    }

                    //右下
                    if (genPlat.Road_Bottom || genPlat.Road_Light)
                    {
                        GameObject obj = InstantiateCorner();
                        obj.transform.position = new Vector3(maxPos.x + 0.2f, minPos.y, minPos.z);
                        obj.transform.rotation = Quaternion.Euler(0, 0, 270);
                        obj.transform.parent = genPlat.transform;
                    }
                }

                //角ではない部分作成
                {  
                    if (genPlat.Road_Top)
                    {
                        for (int i = 1; i < x_lne - 1; i++)
                        {
                            //上部分
                            GameObject obj = InstantiateFlat();
                            float xPos = minPos.x + GridSize * i + GridSize;
                            obj.transform.position = new Vector3(xPos, maxPos.y, minPos.z);
                            obj.transform.rotation = Quaternion.Euler(0, 0, 90);
                            obj.transform.parent = genPlat.transform;

                        }
                    }


                    if (genPlat.Road_Bottom)
                    {
                        for (int i = 1; i < x_lne - 1; i++)
                        {

                            //下部分
                            GameObject obj = InstantiateFlat();
                            float xPos = minPos.x + GridSize * i;
                            obj.transform.position = new Vector3(xPos, minPos.y, minPos.z);
                            obj.transform.rotation = Quaternion.Euler(0, 0, 270);
                            obj.transform.parent = genPlat.transform;
                        }
                    }

                    if (genPlat.Road_Left)
                    {
                        for (int i = 1; i < y_lne - 1; i++)
                        {
                            //左部分
                            GameObject obj = InstantiateFlat();
                            float YPos = minPos.y + GridSize * i + GridSize;
                            obj.transform.position = new Vector3(minPos.x, YPos, minPos.z);
                            obj.transform.rotation = Quaternion.Euler(0, 0, 180);
                            obj.transform.parent = genPlat.transform;
                        }
                    }

                    if (genPlat.Road_Light)
                    {
                        for (int i = 1; i < y_lne - 1; i++)
                        {
                            //右部分
                            GameObject obj = InstantiateFlat();
                            float YPos = minPos.y + GridSize * i;
                            obj.transform.position = new Vector3(maxPos.x, YPos, minPos.z);
                            obj.transform.parent = genPlat.transform;
                        }
                    }
                }
            }

            GameObject InstantiateCorner()
            {
                GameObject obj = Instantiate(genPlat.corner);
                AdjustGridSize(obj);

                //サイズ調整
                float cornerAdjust = 1.27f;
                obj.transform.localScale = new Vector3(obj.transform.localScale.x * cornerAdjust, obj.transform.localScale.y * cornerAdjust, obj.transform.localScale.z);

                return obj;
            }

            GameObject InstantiateFlat()
            {
                GameObject obj = Instantiate(genPlat.flat);
                AdjustGridSize(obj);

                //サイズ調整
                float cornerAdjust = 1.27f;
                obj.transform.localScale = new Vector3(obj.transform.localScale.x * cornerAdjust, obj.transform.localScale.y, obj.transform.localScale.z);

                return obj;
            }

            void AdjustGridSize(GameObject obj)
            {
                //サイズ合わせ
                {
                    Vector3 newScale = Vector3.one;

                    //もとのモデルデータに依存せずにサイズを1:1:1にする
                    newScale.x = 1.0f / obj.GetComponent<Renderer>().bounds.size.x;
                    newScale.y = 1.0f / obj.GetComponent<Renderer>().bounds.size.y;

                    //テスト用足場のサイズに合わせる
                    newScale.x *= GridSize;
                    newScale.y *= GridSize;

                    obj.transform.localScale = new Vector3(newScale.x, newScale.y, obj.transform.localScale.z);
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
