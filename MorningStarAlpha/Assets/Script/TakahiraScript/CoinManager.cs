using System;
using UnityEngine;

[System.Serializable]
public class Coin
{
    // 初期化用コンストラクタ
    public Coin (int Allcoin1, int Allcoin2, int Allcoin3)
    {
        AllCoin1 = Allcoin1;
        AllCoin2 = Allcoin2;
        AllCoin3 = Allcoin3;

        NowGetCoin1 = new bool[Allcoin1];
        NowGetCoin2 = new bool[Allcoin2];
        NowGetCoin3 = new bool[Allcoin3];

        for (int i = 0; i < AllCoin1; i++)
        {
            NowGetCoin1[i] = false;
        }
        for (int i = 0; i < AllCoin2; i++)
        {
            NowGetCoin2[i] = false;
        }
        for (int i = 0; i < AllCoin3; i++)
        {
            NowGetCoin3[i] = false;
        }

        AllGetCoin1 = 0;
        AllGetCoin2 = 0;
        AllGetCoin3 = 0;
    }

    // コピー用コンストラクタ
    public Coin(Coin coin)
    {
        AllCoin1 = coin.AllCoin1;
        AllCoin2 = coin.AllCoin2;
        AllCoin3 = coin.AllCoin3;

        NowGetCoin1 = new bool[AllCoin1];
        NowGetCoin2 = new bool[AllCoin2];
        NowGetCoin3 = new bool[AllCoin3];

        Array.Copy(coin.NowGetCoin1, NowGetCoin1, coin.NowGetCoin1.Length);
        Array.Copy(coin.NowGetCoin2, NowGetCoin2, coin.NowGetCoin2.Length);
        Array.Copy(coin.NowGetCoin3, NowGetCoin3, coin.NowGetCoin3.Length);

        AllGetCoin1 = coin.AllGetCoin1;
        AllGetCoin2 = coin.AllGetCoin2;
        AllGetCoin3 = coin.AllGetCoin3;
    }

    // 取得コイン
    public bool[] NowGetCoin1;
    public bool[] NowGetCoin2;
    public bool[] NowGetCoin3;

    // 合計コイン数
    public int AllCoin1;
    public int AllCoin2;
    public int AllCoin3;

    // 合計入手コイン数
    public int AllGetCoin1;
    public int AllGetCoin2;
    public int AllGetCoin3;
}


public class CoinManager : SingletonMonoBehaviour<CoinManager>
{
    // 現在のコイン取得状況
    public Coin coin = null;        // ゲーム内でのコイン取得用
    public Coin SubCoin = null;     // ゲーム内でのコイン取得用(チェックポイント通過してないとき用)

    // コインオブジェクト
    public GameObject Coins1;
    public GameObject Coins2;
    public GameObject Coins3;

    // コインマネージャー初期化フラグ
    private bool CoinManagerInitFlag = false;


    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject); // シーンが変わっても死なない

        // コイン初期化
        if (!CoinManagerInitFlag)
        {
            ResetCoin();
            CoinManagerInitFlag = true;
        }
            
    }

    public void SetCoins(Coins Coins)
    {
        Coins1 = Coins.transform.GetChild(0).gameObject;
        Coins2 = Coins.transform.GetChild(1).gameObject;
        Coins3 = Coins.transform.GetChild(2).gameObject;

        // コイン初期化
        if (!CoinManagerInitFlag)
        {
            ResetCoin();
            CoinManagerInitFlag = true;
        }


        // ※セーブデータからコインの取得データ持ってくる、ない場合初期化
        if (coin == null)
        {
            if (SaveDataManager.Instance.MainData != null && SaveDataManager.Instance.MainData.Stage[GameStateManager.GetNowStage()].Clear == true)
            {
                coin = SaveDataManager.Instance.MainData.Stage[GameStateManager.GetNowStage()].coin;
            }
            else
            {
                // コイン初期化
                coin = new Coin(
                    Coins1.transform.childCount, 
                    Coins2.transform.childCount, 
                    Coins3.transform.childCount
                    );
            }
        }
        SubCoin = new Coin(coin);

        // コインオブジェクトに情報セット
        SetCoinObjectInfo();
    }


    //private void Start()
    //{
    //    //Debug.LogWarning(coin.NowGetCoin1.Length);
    //}

    //public void FixedUpdate()
    //{
    //    //Debug.LogWarning(coin.NowGetCoin1[0]);
    //    //Debug.LogWarning(coin.NowGetCoin1[1]);
    //    //Debug.LogWarning(coin.NowGetCoin1[2]);
    //}


    // コインオブジェクトにデータセット
    private void SetCoinObjectInfo()
    {
        for (int i = 0; i < Coins1.transform.childCount; i++)
        {
            Coins1.transform.GetChild(i).GetComponent<CoinObject>().SetCoinObjectInfo(new StageCoinInfo(0, i, coin.NowGetCoin1[i]));
        }
        for (int i = 0; i < Coins2.transform.childCount; i++)
        {
            Coins2.transform.GetChild(i).GetComponent<CoinObject>().SetCoinObjectInfo(new StageCoinInfo(1, i, coin.NowGetCoin2[i]));
        }
        for (int i = 0; i < Coins3.transform.childCount; i++)
        {
            Coins3.transform.GetChild(i).GetComponent<CoinObject>().SetCoinObjectInfo(new StageCoinInfo(2, i, coin.NowGetCoin3[i]));
        }
    }


    // コインデータ更新(コインオブジェクト側から呼ぶ用)
    public void SetCoinInfo(StageCoinInfo stageCoinInfo)
    {
        switch (stageCoinInfo.CoinGroup)
        {
            case 0:
                SubCoin.NowGetCoin1[stageCoinInfo.Index] = stageCoinInfo.GetCoinFlag;
                break;
            case 1:
                SubCoin.NowGetCoin2[stageCoinInfo.Index] = stageCoinInfo.GetCoinFlag;
                break;
            case 2:
                SubCoin.NowGetCoin3[stageCoinInfo.Index] = stageCoinInfo.GetCoinFlag;
                break;
        }

        // コイン合計入手数更新
        SetAllGetCoinNum();
    }

    private void SetAllGetCoinNum()
    {
        SubCoin.AllGetCoin1 = SubCoin.AllGetCoin2 = SubCoin.AllGetCoin3 = 0;
        for (int i = 0; i < SubCoin.AllCoin1; i++)
        {
            if (SubCoin.NowGetCoin1[i])
            {
                SubCoin.AllGetCoin1 += 1;
            }
        }
        for (int i = 0; i < SubCoin.AllCoin2; i++)
        {
            if (SubCoin.NowGetCoin2[i])
            {
                SubCoin.AllGetCoin2 += 1;
            }
        }
        for (int i = 0; i < SubCoin.AllCoin3; i++)
        {
            if (SubCoin.NowGetCoin3[i])
            {
                SubCoin.AllGetCoin3 += 1;
            }
        }
    }


    // チェックポイント＆ゴール通った時のコインデータ更新用
    public void SetCheckPointCoinData()
    {
        coin = new Coin(SubCoin);
        //Debug.LogWarning("コイン更新");
    }


    // ステージから出た時のコインマネージャーリセット用
    public void ResetCoin()
    {
        coin = null;
        SubCoin = null;
    }


    //　コインデータセーブ(セーブデータに書き込む用)
    public void SetCoinSaveData()
    {
        SaveDataManager.Instance.MainData.Stage[GameStateManager.GetNowStage()].coin = new Coin(coin);
        //Debug.LogWarning("セーブデータにセーブ");
    }
}