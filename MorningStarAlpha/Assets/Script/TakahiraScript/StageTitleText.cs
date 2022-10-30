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
        "伝説の始まり",
        "StageTitle2",
        "StageTitle3",
        "StageTitle4",
        "StageTitle5",
        "StageTitle6",
        "StageTitle7",
        "StageTitle8",
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
