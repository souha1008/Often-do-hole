using UnityEngine;
using UnityEngine.UI;
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
    public GameObject SkeletonCoin;   // 透明コイン

    [Label("自身のアニメーター")]
    public Animator CoinAnimator;     // コインアニメーター

    private bool OnceFlag = false;

    public void Start()
    {
        // コライダー
        this.gameObject.GetComponent<Collider>().isTrigger = true;

        if (CoinInfo != null)
        {
            if (CoinInfo.GetCoinFlag)
            {
                Instantiate(SkeletonCoin, gameObject.transform.position, gameObject.transform.rotation); // 透明コイン生成
                //Death();
                this.gameObject.SetActive(false);
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
        // コイン取得エフェクト
        EffectManager.Instance.CoinGetEffect(transform.position);
        this.gameObject.SetActive(false);
    }



    public void OnTriggerEnter(Collider collider)
    {
        // プレイヤーと接触時コイン取得
        if (!OnceFlag && (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("Bullet")))
        {
            OnceFlag = true;
            SoundManager.Instance.PlaySound("決定音");
            // ヒットストップ
            GameSpeedManager.Instance.StartHitStop(0.1f);

            // 振動
            VibrationManager.Instance.StartVibration(0.8f, 0.8f, 0.12f);

            CoinInfo.GetCoinFlag = true;
            CoinManager.Instance.SetCoinInfo(CoinInfo);
            
            //Death();
            CoinAnimator.SetBool("GetCoin", true);
        }
    }
}
