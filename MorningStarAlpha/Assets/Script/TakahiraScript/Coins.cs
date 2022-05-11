using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : SingletonMonoBehaviour<Coins>
{
    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        CoinManager.Instance.SetCoins(this);
    }
}
