using UnityEngine;
using UnityEngine.AddressableAssets;

// コインオブジェクト情報
public class StageCoinInfo
{
    public StageCoinInfo(int coinGroup, int index, bool getCoinFlag)
    {
        CoinGroup = coinGroup;      // コインの分割グループ
        Index = index;              // コインNo
        GetCoinFlag = getCoinFlag;  // 取得状況フラグ
    }
    public int CoinGroup;
    public int Index;
    public bool GetCoinFlag;
}

public class CoinObject : MonoBehaviour
{
    // コインオブジェクト情報
    private StageCoinInfo CoinInfo;
    
    [SerializeField, Label("透明コインオブジェクト")]
    public GameObject SkeletonCoin;     // 透明コイン

    public void Start()
    {
        // コライダー
        this.gameObject.GetComponent<Collider>().isTrigger = true;

        if (CoinInfo != null)
        {
            if (CoinInfo.GetCoinFlag)
            {
                Instantiate(SkeletonCoin, gameObject.transform.position, gameObject.transform.rotation); // 透明コイン生成
                Death();
            }              
        }
    }


    // コイン情報セット
    public void SetCoinObjectInfo(StageCoinInfo coinInfo)
    {
        CoinInfo = coinInfo;
    }

    public void Death()
    {
        Destroy(this.gameObject);
    }



    public void OnTriggerEnter(Collider collider)
    {
        // プレイヤーと接触時コイン取得
        if (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("Bullet"))
        {
            SoundManager.Instance.PlaySound("決定音");
            // ヒットストップ
            GameSpeedManager.Instance.StartHitStop(0.1f);
            CoinInfo.GetCoinFlag = true;
            CoinManager.Instance.SetCoinInfo(CoinInfo);
            Death();
        }
    }
}
