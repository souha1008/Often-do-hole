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
    const float GridSize = 3.2f;
    public enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
    }

    [Tooltip("複製したいオブジェクト")] public GameObject platform;
    [Tooltip("向き")] public Direction direction;

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


                GenerateSasaru();

                genPlat.GetComponent<Renderer>().enabled = false;
            }

            if (GUILayout.Button("地形の削除", GUILayout.Height(72)))
            {
                DeleteChildObject();

                genPlat.GetComponent<Renderer>().enabled = true;
            }

            void GenerateSasaru()
            {
                Bounds bounds = genPlat.GetComponent<Renderer>().bounds;

                //マスが縦横それぞれいくつ連続しているかを調べる
                int x_lne = Mathf.RoundToInt(Mathf.Abs(genPlat.transform.lossyScale.x) / GridSize);
                int y_lne = Mathf.RoundToInt(Mathf.Abs(genPlat.transform.lossyScale.y) / GridSize);

                Debug.Log("Xlne = " + x_lne + "  : Ylen = " + y_lne);

                Vector3 minPos = bounds.min;
                minPos.z = bounds.center.z;
                minPos.x += GridSize * 0.5f;
                minPos.y += GridSize * 0.5f;

                for (int x = 0; x < x_lne; x++)
                {
                    for (int y = 0; y < y_lne; y++)
                    {
                        GameObject obj = Instantiate(genPlat.platform);

                        float randSizeZ = 1.0f;

                        //向き調整
                        obj.transform.rotation = Quaternion.Euler(0, 90, 0);


                        //サイズ拡大
                        if (x == 0 || x == x_lne - 1)
                        {
                            if (!(y == 0 || y == y_lne - 1))
                            {
                                randSizeZ = Random.Range(1.0f, 2.0f);
                                //ランダム回転
                                float randRotate = Random.Range(-5.0f, 5.0f);
                                obj.transform.rotation = Quaternion.Euler(0, 0, randRotate) * obj.transform.rotation;
                            }
                        }
                        if (y == 0 || y == y_lne - 1)
                        {
                            if (!(x == 0 || x == x_lne - 1))
                            {
                                randSizeZ = Random.Range(1.0f, 2.0f);
                                //ランダム回転
                                float randRotate = Random.Range(-5.0f, 5.0f);
                                obj.transform.rotation = Quaternion.Euler(0, 0, randRotate) * obj.transform.rotation;
                            }
                        }

                        float randSizeX = Random.Range(1.7f, 2.2f);
                        float randSizeY = Random.Range(1.0f, 1.3f);
                        obj.transform.localScale = new Vector3(obj.transform.localScale.x * randSizeX, obj.transform.localScale.y * randSizeY, obj.transform.localScale.z * randSizeZ);

                        Vector3 SpawnPos = new Vector3(minPos.x + GridSize * x, minPos.y + GridSize * y, minPos.z);
                        obj.transform.position = SpawnPos;

                       
                        //回転
                        switch (genPlat.direction)
                        {
                            case Direction.UP:
                                obj.transform.rotation *= Quaternion.Euler(0, 0, 180);
                            break;

                            case Direction.LEFT:
                                obj.transform.rotation = Quaternion.Euler(0, 0, 270) * obj.transform.rotation;
                                break;

                            case Direction.RIGHT:
                                obj.transform.rotation = Quaternion.Euler(0, 0, 90) * obj.transform.rotation;
                                break;

                            default:
                                break;
                        }


                        obj.transform.parent = genPlat.transform;
                    }
                }
            }

            void DeleteChildObject()
            {
                for (int i = genPlat.transform.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(genPlat.transform.GetChild(i).gameObject);    
                }
            }

            void SizeConvert(GameObject obj)
            {
                //サイズ合わせ
                {
                    Vector3 newScale = Vector3.one;

                    //もとのモデルデータに依存せずにサイズを1:1:1にする
                    newScale.x = 1.0f / obj.GetComponent<Renderer>().bounds.size.x;
                    newScale.y = 1.0f / obj.GetComponent<Renderer>().bounds.size.y;
                    newScale.y = 1.0f / obj.GetComponent<Renderer>().bounds.size.z;

                    //テスト用足場のサイズに合わせる
                    float randSize = Random.Range(3.0f, 5.0f);
                    newScale.x *= randSize;
                    newScale.y *= GridSize;
                    newScale.z *= GridSize;


                    obj.transform.localScale = newScale;
                }
            }
        }
    }
#endif

}
