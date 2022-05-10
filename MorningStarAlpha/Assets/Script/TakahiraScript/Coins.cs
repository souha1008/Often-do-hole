using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
    private void Awake()
    {
        CoinManager.Instance.SetCoins(this);
    }
}
