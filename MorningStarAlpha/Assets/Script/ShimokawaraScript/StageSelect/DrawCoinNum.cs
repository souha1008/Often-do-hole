using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DrawCoinNum : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (SelectManager.instance.CanStart)
        {
            if(SelectManager.instance.NowSelectStage == 0)
            {
                gameObject.GetComponent<TextMeshProUGUI>().text = SaveDataManager.Instance.GetStageData(SelectManager.instance.NowSelectStage).coin.AllGetCoins + " / 3";
            }
            else
            {
                gameObject.GetComponent<TextMeshProUGUI>().text = SaveDataManager.Instance.GetStageData(SelectManager.instance.NowSelectStage).coin.AllGetCoins + " / 9";
            }
        }

        //Debug.Log(SelectManager.instance.CanStart);
        //Debug.Log(SaveDataManager.Instance.GetStageData(GameStateManager.GetNowStage()).coin.AllGetCoins);
        //Debug.Log(GameStateManager.GetNowStage());
    }
}
