using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// アルファ値を変えて前面のオブジェクトを透過させるスクリプト(トゥーンシェーダーのみ)
public class FrontObjectAlphaChange : MonoBehaviour
{
    private List <Material> Materials = new List<Material>();   // マテリアル格納用リスト

    //[Label("消え始める距離")] public float StartDisAppearDistance = 23;   // 消え始める距離(こっちの方が大きく)
    //[Label("完全に消える距離")] public float EndDisAppearDistance = 18;     // 完全に消える距離(0以下にしないこと)
    [Label("最初の透過率(0～1)")] public float StartAlpha = 1.0f;     // 最初の透過率(0.0f～1.0f)

    //private bool OnceFlag = false;

    //[ReadOnly] public float NowAlpha = 0;

    void Start()
    {
        //if (StartDisAppearDistance < EndDisAppearDistance)
        //{
        //    StartDisAppearDistance = EndDisAppearDistance;
        //}

        if (StartAlpha < 0.0f)
        {
            StartAlpha = 0.0f;
        }
        else if (StartAlpha > 1.0f)
        {
            StartAlpha = 1.0f;
        }

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
        Materials.Clear();  // 終了処理
    }

    void Update()
    {
//#if UNITY_EDITOR
//        if (StartAlpha < 0.0f)
//        {
//            StartAlpha = 0.0f;
//        }
//        else if (StartAlpha > 1.0f)
//        {
//            StartAlpha = 1.0f;
//        }
//#endif


//        // プレイヤーの座標
//        Vector3 PlayerPos = PlayerMain.instance.transform.position;
//        Vector3 ThisPos = this.gameObject.transform.position;
//        float Dis = Vector2.Distance(PlayerPos, ThisPos);

//        // プレイヤーとの距離を確認
//        if (Dis <= StartDisAppearDistance)
//        {
//            if (Dis <= EndDisAppearDistance)
//            {
//                NowAlpha = -1.0f;
//                SetAlpha(NowAlpha);
//            }
//            else
//            {
//                NowAlpha = ((Dis - EndDisAppearDistance) / (StartDisAppearDistance - EndDisAppearDistance)) * StartAlpha - 1.0f;
//                SetAlpha(NowAlpha);
//            }
//            OnceFlag = false;
//        }
//        else if (!OnceFlag)
//        {
//            OnceFlag = true;
//            NowAlpha = StartAlpha - 1.0f;
//            SetAlpha(NowAlpha);
//        }
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
