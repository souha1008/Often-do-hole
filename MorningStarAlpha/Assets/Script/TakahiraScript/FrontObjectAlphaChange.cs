using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// アルファ値を変えて前面のオブジェクトを透過させるスクリプト(トゥーンシェーダーのみ)
public class FrontObjectAlphaChange : MonoBehaviour
{
    private List <Material> Materials = new List<Material>();   // マテリアル格納用リスト

    private static float StartDisAppearDistance = 23;   // 消え始める距離(こっちの方が大きく)
    private static float EndDisAppearDistance = 18;     // 完全に消える距離(0以下にしないこと)

    private bool OnceFlag = false;

    public float wa = 0;

    void Start()
    {
        Renderer[] renderers = this.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] materials = renderers[i].materials;

            for (int j = 0; j < materials.Length; j++)
            {
                Materials.Add(materials[j]);
            }
        }
    }

    void Update()
    {
        // プレイヤーの座標
        Vector3 PlayerPos = PlayerMain.instance.transform.position;
        Vector3 ThisPos = this.gameObject.transform.position;
        float Dis = Vector2.Distance(PlayerPos, ThisPos);

        // プレイヤーとの距離を確認
        if (Dis <= StartDisAppearDistance)
        {
            if (Dis <= EndDisAppearDistance)
            {
                SetAlpha(-1.0f);
            }
            else
            {
                wa = (Dis - EndDisAppearDistance) / (StartDisAppearDistance - EndDisAppearDistance) - 1.0f;
                SetAlpha(wa);
            }
            OnceFlag = false;
        }
        else if (!OnceFlag)
        {
            OnceFlag = true;
            SetAlpha(0.0f);
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
}
