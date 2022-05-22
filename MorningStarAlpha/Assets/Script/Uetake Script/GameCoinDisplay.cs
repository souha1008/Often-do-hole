using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameCoinDisplay : MonoBehaviour
{
    private TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        TextUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        TextUpdate();
    }

    private void TextUpdate()
    {
        int coinNum = (int)CoinManager.Instance.SubCoin.AllGetCoin1 + (int)CoinManager.Instance.SubCoin.AllGetCoin2 + (int)CoinManager.Instance.SubCoin.AllGetCoin3;
        string displayTime;


        displayTime = coinNum.ToString() + " / 9";


        text.text = displayTime;
    }
}

