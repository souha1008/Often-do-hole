using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SpriteManager : SingletonMonoBehaviour<SpriteManager>
{
    public static List<Sprite> spritesList = new List<Sprite>();

    static List<string> spriteName = new List<string>();

    bool SpriteLoadFlag = true;

    private IEnumerator SetSpriteData()
    {
        IList<Sprite> sprites;

        // �X�v���C�g�f�[�^�ǂݍ���
        var handleSprite = Addressables.LoadAssetsAsync<Sprite>("Sprite", null);

        do
        {
            yield return null;
        } while (handleSprite.Status != AsyncOperationStatus.Succeeded && handleSprite.Status != AsyncOperationStatus.Failed);

        if(handleSprite.Result != null)
        {
            sprites = handleSprite.Result;

            for(int i = 0; i < sprites.Count; i++)
            {
                spritesList.Add(sprites[i]);
                spriteName.Add(sprites[i].name);
            }
            sprites.Clear();

            Debug.Log("Sprite�ǂݍ��݊���");
        }
        else
        {
            Debug.Log("Sprite�ǂݍ��ݎ��s");
        }
        SpriteLoadFlag = false;
    }

    public Sprite LoadTexture(string SpriteName)
    {
        Sprite sprite_ = spritesList[1];

        for(int i = 0; i < spritesList.Count; i++)
        {
            if(spriteName[i] == SpriteName)
            {
                sprite_ = spritesList[i];
            }
        }

        return sprite_;
    }

    //private IEnumerator LoadTextureIE(string SpriteName, Sprite sprite)
    //{


    //}
        
    void Awake()
    {
        StartCoroutine(SetSpriteData());
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
