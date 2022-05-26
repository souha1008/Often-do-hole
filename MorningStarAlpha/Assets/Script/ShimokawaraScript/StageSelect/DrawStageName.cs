using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DrawStageName : MonoBehaviour
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
        "Tutorial",
        "Stage1",
        "Stage2",
        "Stage3",
        "Stage4",
        "Stage5",
        "Stage6",
        "BOSS"
    };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(SelectManager.instance.CanStart)
        gameObject.GetComponent<TextMeshProUGUI>().text = StageNames[SelectManager.instance.NowSelectStage];
    }
}
