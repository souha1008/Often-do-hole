using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// アルファ値を変えて前面のオブジェクトを透過させるスクリプト(トゥーンシェーダーのみ)
public class FrontObjectAlphaChange : MonoBehaviour
{
    private List <Material> Materials = new List<Material>();   // マテリアル格納用リスト

    [Label("距離による変動なしならチェック")] public bool NoUseDistanceFlag = false;     // 最初の透過率(0.0f～1.0f)
    [Label("消え始める距離")] public float StartDisAppearDistance = 23;   // 消え始める距離(こっちの方が大きく)
    [Label("完全に消える距離")] public float EndDisAppearDistance = 18;     // 完全に消える距離(0以下にしないこと)
    [Label("最初の透過率(0～1)")] public float StartAlpha = 1.0f;     // 最初の透過率(0.0f～1.0f)
    [Label("最後の透過率(0～1)")] public float EndAlpha = 0.0f;     // 最後の透過率(0.0f～1.0f)

    private bool OnceFlag = false;

    [ReadOnly] public float NowAlpha = 0;

    void Start()
    {
        CheckLength(); // 長さ,透過度の矛盾チェック

        Renderer[] renderers = this.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] materials = renderers[i].materials;

            for (int j = 0; j < materials.Length; j++)
            {
                Materials.Add(materials[j]);
            }
        }

        SetAlpha(StartAlpha - 1.0f); // アルファ値セット
        if (NoUseDistanceFlag) 
            Materials.Clear();  // 終了処理
    }

    void Update()
    {
        if (!NoUseDistanceFlag)
        {

#if UNITY_EDITOR
            CheckLength(); // 長さ,透過度の矛盾チェック
#endif

            // プレイヤーの座標
            Vector3 PlayerPos;
            if (PlayerMain.instance != null)
            {
                PlayerPos = PlayerMain.instance.transform.position;
            }
            else
            {
                NoUseDistanceFlag = true;
                return;
            }
            Vector3 ThisPos = this.gameObject.transform.position;
            float Dis = Vector2.Distance(PlayerPos, ThisPos);

            // プレイヤーとの距離を確認
            if (Dis <= StartDisAppearDistance)
            {
                if (Dis <= EndDisAppearDistance)
                {
                    NowAlpha = EndAlpha - 1.0f;
                    SetAlpha(NowAlpha);
                }
                else
                {
                    NowAlpha = ((Dis - EndDisAppearDistance) / (StartDisAppearDistance - EndDisAppearDistance)) * (StartAlpha - EndAlpha) + EndAlpha - 1.0f;
                    SetAlpha(NowAlpha);
                }
                OnceFlag = false;
            }
            else if (!OnceFlag)
            {
                OnceFlag = true;
                NowAlpha = StartAlpha - 1.0f;
                SetAlpha(NowAlpha);
            }
        }
    }

    // アルファ値変更処理
    private void SetAlpha(float Alpha)
    {
        for (int i = 0; i < Materials.Count; i++)
        {
            Materials[i].SetFloat("_Tweak_transparency", Alpha);
        }
    }

    private void OnDestroy()
    {
        Materials.Clear();
    }

    // 長さ,透過度の矛盾チェック
    private void CheckLength()
    {
        if (StartDisAppearDistance < EndDisAppearDistance)
        {
            StartDisAppearDistance = EndDisAppearDistance;
        }

        Mathf.Clamp(StartAlpha, 0.0f, 1.0f);
        Mathf.Clamp(EndAlpha, 0.0f, 1.0f);

        if (StartAlpha < EndAlpha)
        {
            StartAlpha = EndAlpha;
        }
    }
}
