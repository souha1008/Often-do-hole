using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameCoinDisplay : MonoBehaviour
{
    private TextMeshProUGUI text;
    private string coinMax = "9";
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();

        if(GameStateManager.GetNowStage() == 0)
        {
            coinMax = "3";
        }
        else
        {
            coinMax = "9";
        }
        
        TextUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        if (FadeManager.GetNowState() == FADE_STATE.FADE_NONE)
        {
            TextUpdate();
        }
    }

    private void TextUpdate()
    {
        int coinNum = (int)CoinManager.Instance.SubCoin.AllGetCoins;
        string displayTime;


        displayTime = coinNum.ToString() + " / " + coinMax;


        text.text = displayTime;
    }
}

