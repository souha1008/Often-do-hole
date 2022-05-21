using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

// �R�C���I�u�W�F�N�g���
public class StageCoinInfo
{
    public StageCoinInfo(int coinGroup, int index, bool getCoinFlag)
    {
        CoinGroup = coinGroup;      // �R�C���̕����O���[�v
        Index = index;              // �R�C��No
        GetCoinFlag = getCoinFlag;  // �擾�󋵃t���O
    }
    public int CoinGroup;
    public int Index;
    public bool GetCoinFlag;
}

public class CoinObject : MonoBehaviour
{
    // �R�C���I�u�W�F�N�g���
    private StageCoinInfo CoinInfo;
    
    [SerializeField, Label("�����R�C���I�u�W�F�N�g")]
    public GameObject SkeletonCoin;   // �����R�C��

    [Label("���g�̃A�j���[�^�[")]
    public Animator CoinAnimator;     // �R�C���A�j���[�^�[

    private bool OnceFlag = false;

    public void Start()
    {
        // �R���C�_�[
        this.gameObject.GetComponent<Collider>().isTrigger = true;

        if (CoinInfo != null)
        {
            if (CoinInfo.GetCoinFlag)
            {
                Instantiate(SkeletonCoin, gameObject.transform.position, gameObject.transform.rotation); // �����R�C������
                //Death();
                this.gameObject.SetActive(false);
            }              
        }
    }


    // �R�C�����Z�b�g
    public void SetCoinObjectInfo(StageCoinInfo coinInfo)
    {
        CoinInfo = coinInfo;
    }

    public void Death()
    {
        // �R�C���擾�G�t�F�N�g
        EffectManager.Instance.CoinGetEffect(transform.position);
        this.gameObject.SetActive(false);
    }



    public void OnTriggerEnter(Collider collider)
    {
        // �v���C���[�ƐڐG���R�C���擾
        if (!OnceFlag && (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("Bullet")))
        {
            OnceFlag = true;
            SoundManager.Instance.PlaySound("���艹");
            // �q�b�g�X�g�b�v
            GameSpeedManager.Instance.StartHitStop(0.1f);

            // �U��
            VibrationManager.Instance.StartVibration(0.8f, 0.8f, 0.12f);

            CoinInfo.GetCoinFlag = true;
            CoinManager.Instance.SetCoinInfo(CoinInfo);
            
            //Death();
            CoinAnimator.SetBool("GetCoin", true);
        }
    }
}
