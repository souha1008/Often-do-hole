using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SpriteTest : MonoBehaviour
{
    [SerializeField] private AssetReferenceSprite Sprite;
    Sprite sprite1;

    // Start is called before the first frame update
    async void Start()
    {
        sprite1 = await SpriteManager.Instance.LoadTexture(Sprite);

        Debug.LogWarning(sprite1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
