using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public enum GismoMode { None = 0, AlwaysVisible = 10, ShowOnlySelected = 20  }

public class Duplicator : MonoBehaviour
{

    [Tooltip("ギズモの表示を切り替えます。")]             public GismoMode g_mode = GismoMode.ShowOnlySelected;
    [Tooltip("シーンロード時に実行します。")]             public bool duplicateOnStart = true;
    [Tooltip("非同期（コルーチン）します。")]             public bool coroutineDuplicate = false;
    [Tooltip("どこからどこまでの指定をするポイントです。")]  public Transform pointA;
    [Tooltip("どこからどこまでの指定をするポイントです。")]  public Transform pointB;

    [Header("Object")]
    [Tooltip("複製するオブジェクトです。")]         public GameObject original;
    [Tooltip("複製するObjのサイズです。")]         public Vector3 objectSize = Vector3.one;

    [Header("Corner")]
    [Tooltip("コーナーのみオブジェクトを生成します")]    public bool onlyCorner;
    [Tooltip("コーナーオブジェクトです。")]            public GameObject original_corner;
    [Tooltip("X軸にてコーナーを使用します。")]          public bool cornerCheckX;
    [Tooltip("Y軸にてコーナーを使用します。")]          public bool cornerCheckY;
    [Tooltip("Z軸にてコーナーを使用します。")]          public bool cornerCheckZ;



    void Start()
    {
        if (duplicateOnStart)
        {
            if (coroutineDuplicate) StartCoroutine(Duplicate());
            else Duplicate();
        }
    }

    public IEnumerator Duplicate()
    {
        // pointA, pointBから、(ベクトルの計算のために)最小・最大のV3に変換し、
        // Scene上のあいまいな位置のポイントをFloorでグリッドに添わせる

        Vector3 pMin = new Vector3(
            (Mathf.Min(pointA.position.x, pointB.position.x)),
            (Mathf.Min(pointA.position.y, pointB.position.y)),
            (Mathf.Min(pointA.position.z, pointB.position.z))
            );
        Vector3 pMax = new Vector3(
            (Mathf.Max(pointA.position.x, pointB.position.x)),
            (Mathf.Max(pointA.position.y, pointB.position.y)),
            (Mathf.Max(pointA.position.z, pointB.position.z))
            );

        pMin = new Vector3(
            (Mathf.Floor(pMin.x * (1 / objectSize.x))) * objectSize.x,
            (Mathf.Floor(pMin.y * (1 / objectSize.y))) * objectSize.y,
            (Mathf.Floor(pMin.z * (1 / objectSize.z))) * objectSize.z
            );
        pMax = new Vector3(
            Mathf.Floor(pMax.x * (1 / objectSize.x)) * objectSize.x,
            Mathf.Floor(pMax.y * (1 / objectSize.y)) * objectSize.y,
            Mathf.Floor(pMax.z * (1 / objectSize.z)) * objectSize.z
            );

        // 何度回すか計算します。
        int countX = (int)(((pMax.x - pMin.x) / objectSize.x));
        int countY = (int)(((pMax.y - pMin.y) / objectSize.y));
        int countZ = (int)(((pMax.z - pMin.z) / objectSize.z));


        for (int z = 0; z <= countZ; z++)
            for (int y = 0; y <= countY; y++)
                for (int x = 0; x <= countX; x++)
                {
                    Vector3 ofs = (new Vector3(objectSize.x * x, objectSize.y * y, objectSize.z * z));
                    Vector3 _pos = (pMin + objectSize / 2 + ofs);


                    if (!(onlyCorner || cornerCheckX || cornerCheckY || cornerCheckZ))
                    {
                        Inst(_pos, original);
                    }
                    else
                    {
                        if (cornerCheckX && (x == 0 || x == countX))
                        {
                            Inst(_pos, original_corner);
                        }
                        else if (cornerCheckY && (y == 0 || y == countY))
                        {
                            Inst(_pos, original_corner);
                        }
                        else if (cornerCheckZ && (z == 0 || z == countZ))
                        {
                            Inst(_pos, original_corner);
                        }
                        else if(!onlyCorner)
                        {
                            Inst(_pos, original);
                        }
                    }

                    if(coroutineDuplicate) yield return null;
                }
        yield return null;
    }

    private void Inst(Vector3 _pos, GameObject _go)
    {

        var go = Instantiate(_go);
        go.transform.position = _pos;
        go.transform.parent = this.gameObject.transform;
        go.SetActive(true);
    }

    void ShowGizmos()
    {
        Vector3 pMin = new Vector3(
            (Mathf.Min(pointA.position.x, pointB.position.x)),
            (Mathf.Min(pointA.position.y, pointB.position.y)),
            (Mathf.Min(pointA.position.z, pointB.position.z))
            );
        Vector3 pMax = new Vector3(
            (Mathf.Max(pointA.position.x, pointB.position.x)),
            (Mathf.Max(pointA.position.y, pointB.position.y)),
            (Mathf.Max(pointA.position.z, pointB.position.z))
            );

        pMin = new Vector3(
            (Mathf.Floor(pMin.x * (1 / objectSize.x))) * objectSize.x,
            (Mathf.Floor(pMin.y * (1 / objectSize.y))) * objectSize.y,
            (Mathf.Floor(pMin.z * (1 / objectSize.z))) * objectSize.z
            );
        pMax = new Vector3(
            Mathf.Floor(pMax.x * (1 / objectSize.x)) * objectSize.x,
            Mathf.Floor(pMax.y * (1 / objectSize.y)) * objectSize.y,
            Mathf.Floor(pMax.z * (1 / objectSize.z)) * objectSize.z
            );  

        int countX = (int)(((pMax.x - pMin.x) / objectSize.x));
        int countY = (int)(((pMax.y - pMin.y) / objectSize.y));
        int countZ = (int)(((pMax.z - pMin.z) / objectSize.z));

        for (int z = 0; z <= countZ; z++)
            for (int y = 0; y <= countY; y++)
                for (int x = 0; x <= countX; x++)
                {

                    Gizmos.color = Color.white;

                    if (cornerCheckX && ( x == 0 || x == countX )) Gizmos.color = Color.red;
                    if (cornerCheckY && ( y == 0 || y == countY )) Gizmos.color = Color.red;
                    if (cornerCheckZ && ( z == 0 || z == countZ )) Gizmos.color = Color.red;

                    if (!onlyCorner)
                    {
                        Vector3 ofs = (new Vector3(objectSize.x * x, objectSize.y * y, objectSize.z * z));
                        Gizmos.DrawWireCube(pMin + objectSize / 2 + ofs, objectSize);
                    }else if (onlyCorner)
                    {
                        if (cornerCheckX && (x == 0 || x == countX))
                        {
                            Vector3 ofs = (new Vector3(objectSize.x * x, objectSize.y * y, objectSize.z * z));
                            Gizmos.DrawWireCube(pMin + objectSize / 2 + ofs, objectSize);
                        }
                        if (cornerCheckY && (y == 0 || y == countY))
                        {
                            Vector3 ofs = (new Vector3(objectSize.x * x, objectSize.y * y, objectSize.z * z));
                            Gizmos.DrawWireCube(pMin + objectSize / 2 + ofs, objectSize);
                        }
                        if (cornerCheckZ && (z == 0 || z == countZ))
                        {
                            Vector3 ofs = (new Vector3(objectSize.x * x, objectSize.y * y, objectSize.z * z));
                            Gizmos.DrawWireCube(pMin + objectSize / 2 + ofs, objectSize);
                        }
                    }
                }
    }

    private void OnDrawGizmosSelected()
    {
        if (pointA == null || pointB == null) InitComponent();

        if (g_mode != GismoMode.None) ShowGizmos();
        transform.position = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        if(g_mode == GismoMode.AlwaysVisible)
        {
            ShowGizmos();
        }

    }

    [ContextMenu("InitComponent")]
    public void InitComponent()
    {
        if (pointA == null)
        {
            GameObject pA = new GameObject("pointA");
            pA.transform.parent = this.transform;
            pointA = pA.transform;
        }
        if (pointB == null)
        {
            GameObject pB = new GameObject("pointB");
            pB.transform.parent = this.transform;
            pointB = pB.transform;
        }

        g_mode = GismoMode.AlwaysVisible;
        transform.position = Vector3.zero;

        foreach (Transform child in gameObject.transform)
        {
            if(!(child.gameObject.name == "pointA" || child.gameObject.name == "pointB"))
                DestroyImmediate(child.gameObject);
        }

        coroutineDuplicate = false;
        objectSize = Vector3.one;
        onlyCorner = false;
        cornerCheckX = false;
        cornerCheckY = false;
        cornerCheckZ = false;

        ResetPoints();

    }

    public void ResetPoints()
    {
        pointA.transform.localPosition = new Vector3(-1, 0, 0);
        pointB.transform.localPosition = new Vector3(1, 0, 0);
        AdjustPointsToGrid();
    }

    public void AdjustPointsToGrid()
    {
        pointA.position = new Vector3(
            (Mathf.Floor(pointA.position.x * (1 / objectSize.x))) * objectSize.x + (objectSize.x / 2),
            (Mathf.Floor(pointA.position.y * (1 / objectSize.y))) * objectSize.y + (objectSize.y / 2),
            (Mathf.Floor(pointA.position.z * (1 / objectSize.z))) * objectSize.z + (objectSize.z / 2)
            );
        pointB.position = new Vector3(
            Mathf.Floor(pointB.position.x * (1 / objectSize.x)) * objectSize.x + (objectSize.x / 2),
            Mathf.Floor(pointB.position.y * (1 / objectSize.y)) * objectSize.y + (objectSize.y / 2),
            Mathf.Floor(pointB.position.z * (1 / objectSize.z)) * objectSize.z + (objectSize.z / 2)
            );
    }

    public void ResetObjects()
    {
        foreach (Transform child in gameObject.transform)
        {
            if (!(child.gameObject.name == "pointA" || child.gameObject.name == "pointB"))
                DestroyImmediate(child.gameObject);
        }
    }


}


// 以下UnityEditor
#if UNITY_EDITOR
[CustomEditor(typeof(Duplicator))]
public class DuplicatorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Duplicator dup = target as Duplicator;

        GUILayout.Space(20);
        if (GUILayout.Button("オブジェクトの配置", GUILayout.Height(72)))
        {
            bool oldCD = dup.coroutineDuplicate;
            dup.coroutineDuplicate = false;
            dup.ResetObjects();
            dup.StartCoroutine(dup.Duplicate());
            dup.g_mode = GismoMode.None;
            dup.duplicateOnStart = false;
            dup.coroutineDuplicate = oldCD;
        }
        if (GUILayout.Button("オブジェクトリセット(たまに残るので数回クリック)", GUILayout.Height(32)))
        {
            dup.duplicateOnStart = true;
            dup.g_mode = GismoMode.AlwaysVisible;
            for (int i = 0; i < 30; i++)
            {
                dup.ResetObjects();
            }
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ポイントをグリッドに添わせる", GUILayout.Height(32)))
        {
            dup.AdjustPointsToGrid();
        }
        if (GUILayout.Button("ポイントリセット", GUILayout.Height(32)))
        {
            dup.ResetPoints();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("コンポーネントの初期化"))
        {
            dup.InitComponent();
        }

    }

}
#endif