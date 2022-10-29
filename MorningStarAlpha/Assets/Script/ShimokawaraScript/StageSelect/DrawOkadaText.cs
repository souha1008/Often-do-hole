using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DrawOkadaText : MonoBehaviour
{
    private string[] StageNames = {
#if false
        "Stage1",
        "Stage2",
        "Stage3",
        "Stage4",
        "Stage5",
        "Stage6",
        "Stage7",
        "Stage8",
        "Stage9",
        "Stage10",
        "Stage11"
#endif
        "「伝説のおわり、夢のはじまり」\n伝説の海賊が眠りし地、カナタ島。\n伝説を夢見た少女が今、大雲海に飛び込んだ！",
        "「空色の眼の少女」\n少女の名はラティ。カナタ島の最奥にある秘宝めざして、\n巨大なイカリをふりまわす！",
        "「機械仕掛けの街」\nラティは次のお宝を求めてハグルマ島へ向かった。\nハグルマたちを利用して、どんどん突き進め！",
        "「乱反射するスイショウ谷」\n西日を受けながらどんどん進むラティ。\nその先には、トゲトゲのクリスタルが待ち受けていた！",
        "「スイショウ谷はもう一つ」\n不思議なトンネルを見つけたラティ。\n夕日の差し込むその洞窟には、不思議な空間が広がっていた！",
        "「月が落ちる街」\nすっかり日も暮れた中、旧市街にお宝アリとのうわさ。\nでも旧市街には、不気味なうわさもあるようで......",
        "「旧市街のガンマンたち」\nさらなるお宝を求めて真夜中の旧市街を探索していると、\nどこからともなく砲声が！どうやらサメ達が目を覚ましたようだ......",
        "「迫りくるは巨大な」\n旧市街の最後のお宝を手に入れたラティ。\nその直後、轟音とともに大きな影がラティに迫りくる！"
    };

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (SelectManager.instance.CanStart)
            gameObject.GetComponent<TextMeshProUGUI>().text = StageNames[SelectManager.instance.NowSelectStage];
    }
}
