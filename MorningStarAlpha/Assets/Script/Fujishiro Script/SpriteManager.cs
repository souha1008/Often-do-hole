using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class SpriteManager : SingletonMonoBehaviour<SpriteManager>
{
    public async Task<Sprite> LoadTexture(string SpriteName)
    {
        Sprite sprite = null;
        // スプライトデータ読み込み
        var HandleSprites = await Addressables.LoadResourceLocationsAsync(SpriteName, typeof(Sprite)).Task;

        if (HandleSprites != default)
        {
            sprite = await Addressables.LoadAssetAsync<Sprite>(HandleSprites).Task;

            Debug.LogWarning("Sprite読み込み完了");
        }
        else
        {
            Debug.LogWarning("Sprite読み込み失敗");
        }

        Addressables.Release(HandleSprites);
        return sprite;
    }

    public async Task<Sprite> LoadTexture(AssetReferenceSprite ARSprite)
    {
        Sprite sprite = null;

        // スプライトデータ読み込み
        var HandleSprites = await Addressables.LoadResourceLocationsAsync(ARSprite, typeof(Sprite)).Task;

        if (HandleSprites != default)
        {
            sprite = await Addressables.LoadAssetAsync<Sprite>(HandleSprites).Task;

            Debug.LogWarning("Sprite読み込み完了");
            Debug.LogWarning(sprite);
        }
        else
        {
            Debug.LogWarning("Sprite読み込み失敗");
        }

        Addressables.Release(HandleSprites);

        return sprite;
    }
}
