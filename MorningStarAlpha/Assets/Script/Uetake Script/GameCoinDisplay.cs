using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCoinDisplay : MonoBehaviour
{
    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        string displayTime;
        displayTime = CoinManager.Instance.coin.AllGetCoins + " / 9";


        text.text = displayTime;
    }
}

