using UnityEngine;

public class Coin
{
    public bool[] NowGetCoin1;
    public bool[] NowGetCoin2;
    public bool[] NowGetCoin3;
}


public class CoinManager : SingletonMonoBehaviour<CoinManager>
{
    // 合計コイン数
    private int AllCoin1;
    private int AllCoin2;
    private int AllCoin3;

    // 現在のコイン取得状況
    public Coin coin;

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

        //DontDestroyOnLoad(this.gameObject); // シーンが変わっても死なない



        // コイン取得状況初期化

        // ※セーブデータからコインの取得データ持ってくる、ない場合初期化

        //if (SaveDataManager.Instance.MainData.Stage[].coin != null)
        //{
        //    coin = SaveDataManager.Instance.MainData.Stage[].coin;
        //    SetCoinObjectInfo();
        //}
        //else
        {
            // 合計コイン数取得
            AllCoin1 = Coins1.transform.childCount;
            AllCoin2 = Coins2.transform.childCount;
            AllCoin3 = Coins3.transform.childCount;

            // コイン初期化
            coin = new Coin();
            coin.NowGetCoin1 = new bool[AllCoin1];
            coin.NowGetCoin2 = new bool[AllCoin2];
            coin.NowGetCoin3 = new bool[AllCoin3];

            // コインオブジェクトに情報セット
            SetCoinObjectInfo();
        }
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
                coin.NowGetCoin1[stageCoinInfo.Index] = stageCoinInfo.GetCoinFlag;
                break;
            case 1:
                coin.NowGetCoin2[stageCoinInfo.Index] = stageCoinInfo.GetCoinFlag;
                break;
            case 2:
                coin.NowGetCoin3[stageCoinInfo.Index] = stageCoinInfo.GetCoinFlag;
                break;
        }
    }


    //　コインデータセーブ(セーブデータに書き込む用)
    private void SetCoinSaveData()
    {
        //SaveDataManager.Instance.MainData.Stage[].coin = coin;
    }
}