using UnityEngine;

[System.Serializable]
public class Coin
{
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
    private Coin SubCoin = null;    // ゲーム内でのコイン取得用(チェックポイント通過してないとき用)

    // コインオブジェクト
    public GameObject Coins1;
    public GameObject Coins2;
    public GameObject Coins3;


    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject); // シーンが変わっても死なない
    }

    public void SetCoins(Coins Coins)
    {
        Coins1 = Coins.transform.GetChild(0).gameObject;
        Coins2 = Coins.transform.GetChild(1).gameObject;
        Coins3 = Coins.transform.GetChild(2).gameObject;

        // コイン取得状況初期化

        // ※セーブデータからコインの取得データ持ってくる、ない場合初期化

        if (coin == null)
        {
            if (SaveDataManager.Instance.MainData.Stage[GameStateManager.GetNowStage()].Clear == true)
            {
                coin = SaveDataManager.Instance.MainData.Stage[GameStateManager.GetNowStage()].coin;
            }
            else
            {
                // コイン初期化
                coin = new Coin();
                // 合計コイン数取得
                coin.AllCoin1 = Coins1.transform.childCount;
                coin.AllCoin2 = Coins2.transform.childCount;
                coin.AllCoin3 = Coins3.transform.childCount;
                // コインの配列初期化
                coin.NowGetCoin1 = new bool[coin.AllCoin1];
                coin.NowGetCoin2 = new bool[coin.AllCoin2];
                coin.NowGetCoin3 = new bool[coin.AllCoin3];

                // コインの合計取得数
                coin.AllGetCoin1 = 0;
                coin.AllGetCoin2 = 0;
                coin.AllGetCoin3 = 0;

                //Debug.LogWarning(coin.AllCoin1);
            }
        }
        SubCoin = coin;

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
            Coins2.transform.GetChild(i).GetComponent<CoinObject>().SetCoinObjectInfo(new StageCoinInfo(0, i, coin.NowGetCoin2[i]));
        }
        for (int i = 0; i < Coins3.transform.childCount; i++)
        {
            Coins3.transform.GetChild(i).GetComponent<CoinObject>().SetCoinObjectInfo(new StageCoinInfo(0, i, coin.NowGetCoin3[i]));
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
        coin = SubCoin;
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
        SaveDataManager.Instance.MainData.Stage[GameStateManager.GetNowStage()].coin = coin;
        Debug.LogWarning("セーブデータにセーブ");
    }
}