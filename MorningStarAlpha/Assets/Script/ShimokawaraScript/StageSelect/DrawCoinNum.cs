using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            gameObject.GetComponent<UnityEngine.UI.Text>().text = "x " + SaveDataManager.Instance.GetStageData(SelectManager.instance.NowSelectStage).coin.AllGetCoins + " / 9";

        //Debug.Log(SelectManager.instance.CanStart);
        //Debug.Log(SaveDataManager.Instance.GetStageData(GameStateManager.GetNowStage()).coin.AllGetCoins);
        //Debug.Log(GameStateManager.GetNowStage());
    }
}
