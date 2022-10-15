using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

//Inspector�ŕҏW���₷�����邽��class��ǉ�
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
    //�ϊ�����Tilemap
    [SerializeField]
    private Tilemap setTileMap;

    //Tile�̕ϊ��Ή�List
    [SerializeField]
    private List<ConvertTile> convertTiles = new List<ConvertTile>();

    //�ϊ���Object1�}�X������̃T�C�Y
    private float nodeSizeX = 3.2f;
    private float nodeSizeY = 3.2f;
    //���[�g�̖��O
    const string ParentName = "PLATFORMS";

    private GameObject newParent;
    private TileBase StartedTile;

    int sizeX;
    int sizeY;
    int scanX;
    int scanY;

    //Inspector�̃{�^������Ăяo��
    public void ConvertMapTile()
    {
        //�ϊ��O�ɂ��Ƃ̃f�[�^����������폜
        DeleteMap();

        //�^�C���}�b�v�̃T�C�Y
        sizeX = setTileMap.size.x;
        sizeY = setTileMap.size.y;
        //�^�C�����W���}�C�i�X�̒l����肤��̂łƂ肠���������͈͂�{�ɂ��đΉ�
        scanX = sizeX * 2;
        scanY = sizeY * 2;
        //Vector3 offset = new Vector3(nodeSizeX * 0.5f, nodeSizeY * 0.5f, 0.0f);


        //���łɒn�`�쐬���Ă��邩�ǂ���
        int[,] array = new int[scanY, scanX];

        //�V����GameObject�̃��[�g
        newParent = new GameObject(ParentName);
        Undo.RegisterCreatedObjectUndo(newParent, "Create New GameObject");

        //Tilemap����������Object����
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
                        StartedTile = getTile;

                        SearchRect(cx, cy, getTile);
                        Vector3 size = Vector3.one;
                        Vector3 pos = Vector3.one;
                        GeneratePlatform(target.GetPrefab(), pos, size);
                    }
                }
            }
        }
    }

    //�}�b�v�̏���
    public void DeleteMap()
    {
        GameObject parent = GameObject.Find(ParentName);
        Object.DestroyImmediate(parent);
    }

    //�ϊ��Ή�List������
    ConvertTile FindTile(TileBase searchTile)
    {
        return convertTiles.Find(tile => tile.GetTile() == searchTile);
    }

    private void SearchRect(int startX, int startY, TileBase tileType)
    {
        //�������ɘA������u���b�N�̌�������
        int Y_len = 0;
        int X_len = 0;

        while (true)
        {
            int nx = startX + X_len;
            int ny = startY + Y_len;

            Vector3Int cellPosition = new Vector3Int(nx - sizeX, ny - sizeY, 0);
            TileBase getTile = setTileMap.GetTile(cellPosition);

            if (getTile == tileType)
            {
                Y_len++;
                if(Y_len >= scanY)
                {

                }
            }
            else
            {
                break;
            }
          
        }

    }

    //�n�`�̐���
    private void GeneratePlatform(GameObject Prefab, Vector3 pos, Vector3 size)
    {
        GameObject newGameObject = Instantiate(Prefab);
        Vector3 newScale = Vector3.one;

        //���Ƃ̃��f���f�[�^�Ɉˑ������ɃT�C�Y��1:1:1�ɂ���
        newScale.x = 1.0f / newGameObject.GetComponent<Renderer>().bounds.size.x;
        newScale.y = 1.0f / newGameObject.GetComponent<Renderer>().bounds.size.y;
        newScale.z = 1.0f / newGameObject.GetComponent<Renderer>().bounds.size.z;

        //�e�X�g�p����̃T�C�Y�ɍ��킹��
        newScale.x *= size.x;
        newScale.y *= size.y;
        newScale.z *= 10.0f;

        //newScale.x += genPlat.AdjustSizeX;
        //newScale.y += genPlat.AdjustSizeY;
        //newScale.z += genPlat.AdjustSizeZ;

        newGameObject.transform.localScale = newScale;


        Vector3 GeneratePos = pos;
        newGameObject.transform.position = GeneratePos;


        newGameObject.transform.parent = newParent.transform;
        Undo.RegisterCreatedObjectUndo(newGameObject, "Create New GameObject");
    }
}


