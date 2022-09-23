using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

//Inspectorで編集しやすくするためclassを追加
[System.Serializable]
class ConvertTile
{
    [SerializeField]
    TileBase tile = null;

    [SerializeField]
    GameObject prefab = null;

    public TileBase GetTile()
    {
        return tile;
    }
    public GameObject GetPrefab()
    {
        return prefab;
    }
}



public class MapConverter : MonoBehaviour
{
    //変換元のTilemap
    [SerializeField]
    private Tilemap setTileMap;

    //Tileの変換対応List
    [SerializeField]
    private List<ConvertTile> convertTiles = new List<ConvertTile>();

    //変換後Object1マスあたりのサイズ
    private float nodeSizeX = 3.2f;
    private float nodeSizeY = 3.2f;
    //ルートの名前
    const string ParentName = "PLATFORMS";


    //Inspectorのボタンから呼び出し
    public void ConvertMapTile()
    {
        //変換前にもとのデータがあったら削除
        DeleteMap();

        //タイルマップのサイズ
        int sizeX = setTileMap.size.x;
        int sizeY = setTileMap.size.y;
        //タイル座標がマイナスの値を取りうるのでとりあえず検索範囲を倍にして対応
        int scanX = sizeX * 2;
        int scanY = sizeY * 2;
        Vector3 offset = new Vector3(nodeSizeX * 0.5f, nodeSizeY * 0.5f, 0.0f);
        //新しいGameObjectのルート
        GameObject newParent = new GameObject(ParentName);
        Undo.RegisterCreatedObjectUndo(newParent, "Create New GameObject");

        //Tilemapを検索してObject生成
        for (int cy = 0; cy < scanY; cy++)
        {
            for (int cx = 0; cx < scanX; cx++)
            {
                Vector3Int cellPosition = new Vector3Int(cx - sizeX, cy - sizeY, 0);
                TileBase getTile = setTileMap.GetTile(cellPosition);

                if (getTile != null)
                {
                    var target = FindTile(getTile);
                    if ((target != null) && (target.GetPrefab() != null))
                    {
                        GameObject newGameObject = (GameObject)Instantiate(target.GetPrefab());

                        //サイズ合わせ
                        {
                            Vector3 newScale = Vector3.zero;

                            //もとのモデルデータに依存せずにサイズを1:1:1にする
                            newScale.x = 1.0f / newGameObject.GetComponent<Renderer>().bounds.size.x;
                            newScale.y = 1.0f / newGameObject.GetComponent<Renderer>().bounds.size.y;
                            newScale.z = 1.0f / newGameObject.GetComponent<Renderer>().bounds.size.z;

                            //テスト用足場のサイズに合わせる
                            newScale.x *= nodeSizeX;
                            newScale.y *= nodeSizeY;
                            newScale.z *= 10.0f;

                            //newScale.x += genPlat.AdjustSizeX;
                            //newScale.y += genPlat.AdjustSizeY;
                            //newScale.z += genPlat.AdjustSizeZ;

                            newGameObject.transform.localScale = newScale;
                        }

                        Vector3 GeneratePos = new Vector3((cx - sizeX) * nodeSizeX, (cy - sizeY) * nodeSizeY, 0.0f);
                        GeneratePos += offset;
                        newGameObject.transform.position = GeneratePos;


                        newGameObject.transform.parent = newParent.transform;
                        Undo.RegisterCreatedObjectUndo(newGameObject, "Create New GameObject");
                    }
                }
            }
        }
    }

    //マップの消去
    public void DeleteMap()
    {
        GameObject parent = GameObject.Find(ParentName);
        Object.DestroyImmediate(parent);
    }

    //変換対応Listを検索
    ConvertTile FindTile(TileBase searchTile)
    {
        return convertTiles.Find(tile => tile.GetTile() == searchTile);
    }
}