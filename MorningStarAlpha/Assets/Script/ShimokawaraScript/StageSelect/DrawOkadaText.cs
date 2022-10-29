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
        "伝説の海賊が眠りし地、カナタ島。\n伝説を夢見た少女が今、\n大雲海に飛び込んだ！",
        "少女の名はラティ。\nカナタ島の最奥にある\n秘宝めざして、\n巨大なイカリを\nふりまわす！",
        "ラティは次の\nお宝を求めて\nハグルマ島へ向かった。\nハグルマたちを利用して\nどんどん突き進め！",
        "西日を受けながら\nどんどん進むラティ。\nその先には、トゲトゲのクリスタルが\n待ち受けていた！",
        "不思議なトンネルを\n見つけたラティ。\n夕日の差し込む\nその洞窟には、\n不思議な空間が\n広がっていた！",
        "すっかり日も暮れた中、旧市街に\nお宝アリとのうわさ。\nでも旧市街には、\n不気味なうわさも\nあるようで......",
        "さらなるお宝を求めて\n真夜中の旧市街を\n探索していると、\nどこからともなく\n砲声が！\nどうやらサメ達が\n目を覚ましたようだ...",
        "旧市街の最後のお宝を\n手に入れたラティ。\nその直後、轟音とともに大きな影がラティに\n迫りくる！"
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
