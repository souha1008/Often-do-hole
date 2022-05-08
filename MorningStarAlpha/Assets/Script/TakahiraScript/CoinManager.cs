using UnityEngine;

[System.Serializable]
public class Coin
{
    public bool[] NowGetCoin1;
    public bool[] NowGetCoin2;
    public bool[] NowGetCoin3;

    // ���v�R�C����
    public int AllCoin1;
    public int AllCoin2;
    public int AllCoin3;
}


public class CoinManager : SingletonMonoBehaviour<CoinManager>
{
    // ���݂̃R�C���擾��
    public Coin coin = null;        // �Q�[�����ł̃R�C���擾�p
    private Coin OldCoin = null;    // �Q�[�����ł̃R�C���擾�p(�`�F�b�N�|�C���g�ʉ߂��ĂȂ��Ƃ��p)

    // �R�C���I�u�W�F�N�g
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

        DontDestroyOnLoad(this.gameObject); // �V�[�����ς���Ă����ȂȂ�
    }

    public void SetCoins(Coins Coins)
    {
        Coins1 = Coins.transform.GetChild(0).gameObject;
        Coins2 = Coins.transform.GetChild(1).gameObject;
        Coins3 = Coins.transform.GetChild(2).gameObject;

        // �R�C���擾�󋵏�����

        // ���Z�[�u�f�[�^����R�C���̎擾�f�[�^�����Ă���A�Ȃ��ꍇ������

        if (coin == null)
        {
            if (SaveDataManager.Instance.MainData.Stage[GameStateManager.GetNowStage()].Clear == true)
            {
                coin = SaveDataManager.Instance.MainData.Stage[GameStateManager.GetNowStage()].coin;
            }
            else
            {
                // �R�C��������
                coin = new Coin();
                // ���v�R�C�����擾
                coin.AllCoin1 = Coins1.transform.childCount;
                coin.AllCoin2 = Coins2.transform.childCount;
                coin.AllCoin3 = Coins3.transform.childCount;
                // �R�C���̔z�񏉊���
                coin.NowGetCoin1 = new bool[coin.AllCoin1];
                coin.NowGetCoin2 = new bool[coin.AllCoin2];
                coin.NowGetCoin3 = new bool[coin.AllCoin3];
                //Debug.LogWarning(coin.AllCoin1);
            }
        }
        OldCoin = coin;

        // �R�C���I�u�W�F�N�g�ɏ��Z�b�g
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


    // �R�C���I�u�W�F�N�g�Ƀf�[�^�Z�b�g
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


    // �R�C���f�[�^�X�V(�R�C���I�u�W�F�N�g������Ăԗp)
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

    // �`�F�b�N�|�C���g���S�[���ʂ������̃R�C���f�[�^�X�V�p
    public void SetCheckPointCoinData()
    {
        coin = OldCoin;
    }


    // �X�e�[�W����o�����̃R�C���}�l�[�W���[���Z�b�g�p
    public void ResetCoin()
    {
        coin = null;
        OldCoin = null;
    }


    //�@�R�C���f�[�^�Z�[�u(�Z�[�u�f�[�^�ɏ������ޗp)
    public void SetCoinSaveData()
    {
        SaveDataManager.Instance.MainData.Stage[GameStateManager.GetNowStage()].coin = coin;
        Debug.LogWarning("�Z�[�u�f�[�^�ɃZ�[�u");
    }
}