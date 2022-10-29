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
        "伝説の海賊が眠りし地、カナタ島。\n伝説を夢見た少女が今、大雲海に飛び込んだ！",
        "少女の名はラティ。カナタ島の最奥にある秘宝めざして、\n巨大なイカリをふりまわす！",
        "ラティは次のお宝を求めてハグルマ島へ向かった。\nハグルマたちを利用して、どんどん突き進め！",
        "西日を受けながらどんどん進むラティ。\nその先には、トゲトゲのクリスタルが待ち受けていた！",
        "不思議なトンネルを見つけたラティ。\n夕日の差し込むその洞窟には、不思議な空間が広がっていた！",
        "すっかり日も暮れた中、旧市街にお宝アリとのうわさ。\nでも旧市街には、不気味なうわさもあるようで......",
        "さらなるお宝を求めて真夜中の旧市街を探索していると、\nどこからともなく砲声が！どうやらサメ達が目を覚ましたようだ......",
        "旧市街の最後のお宝を手に入れたラティ。\nその直後、轟音とともに大きな影がラティに迫りくる！"
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
