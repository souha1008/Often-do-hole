using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class StageTitleText : MonoBehaviour
{
    private TextMeshProUGUI TMProObj;   // テキストオブジェクト
    //private Image ImageObj;   // イメージオブジェクト
    //[SerializeField] private Sprite[] StageTitleSprites; // ステージタイトルイメージ


    private string[] TitleText =
    {
        "夢のはじまり",
        "空色の眼の少女",
        "機械仕掛けの街",
        "乱反射するスイショウ谷",
        "スイショウ谷はもう一つ",
        "月が落ちる街",
        "旧市街のガンマンたち",
        "迫りくるは巨大な...",
        "StageTitle9"
    };

    // Start is called before the first frame update
    void Start()
    {
        // 取得
        TMProObj = GetComponent<TextMeshProUGUI>();
        //ImageObj = GetComponent<Image>();

        if (TMProObj != null)
        {
            TMProObj.text = TitleText[GameStateManager.GetNowStage()];  // 文字変更
        }
    }
}
