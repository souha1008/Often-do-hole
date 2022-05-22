using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class SpriteManager : SingletonMonoBehaviour<SpriteManager>
{
    public async Task<Sprite> LoadTexture(string SpriteName)
    {
        Sprite sprite = null;
        // �X�v���C�g�f�[�^�ǂݍ���
        var HandleSprites = await Addressables.LoadResourceLocationsAsync(SpriteName, typeof(Sprite)).Task;

        if (HandleSprites != default)
        {
            sprite = await Addressables.LoadAssetAsync<Sprite>(HandleSprites).Task;

            Debug.LogWarning("Sprite�ǂݍ��݊���");
        }
        else
        {
            Debug.LogWarning("Sprite�ǂݍ��ݎ��s");
        }

        Addressables.Release(HandleSprites);
        return sprite;
    }

    public async Task<Sprite> LoadTexture(AssetReferenceSprite ARSprite)
    {
        Sprite sprite = null;

        // �X�v���C�g�f�[�^�ǂݍ���
        var HandleSprites = await Addressables.LoadResourceLocationsAsync(ARSprite, typeof(Sprite)).Task;

        if (HandleSprites != default)
        {
            sprite = await Addressables.LoadAssetAsync<Sprite>(HandleSprites).Task;

            Debug.LogWarning("Sprite�ǂݍ��݊���");
            Debug.LogWarning(sprite);
        }
        else
        {
            Debug.LogWarning("Sprite�ǂݍ��ݎ��s");
        }

        Addressables.Release(HandleSprites);

        return sprite;
    }
}
