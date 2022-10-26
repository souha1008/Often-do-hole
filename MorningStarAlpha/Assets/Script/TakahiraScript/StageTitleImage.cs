using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageTitleImage : MonoBehaviour
{
    private TextMeshProUGUI TMProObj;   // テキストオブジェクト

    // Start is called before the first frame update
    void Start()
    {
        // 取得
        TMProObj = GetComponent<TextMeshProUGUI>();

        if (TMProObj != null)
        {
            TMProObj.text = GameStateManager.GetNowStage().ToString();  // 文字変更
        }    
    }
}
