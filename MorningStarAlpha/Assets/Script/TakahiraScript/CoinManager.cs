using UnityEngine;

public class Coin
{
    public bool[] NowGetCoin1;
    public bool[] NowGetCoin2;
    public bool[] NowGetCoin3;
}


public class CoinManager : SingletonMonoBehaviour<CoinManager>
{
    // ���v�R�C����
    private int AllCoin1;
    private int AllCoin2;
    private int AllCoin3;

    // ���݂̃R�C���擾��
    public Coin coin;

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

        //DontDestroyOnLoad(this.gameObject); // �V�[�����ς���Ă����ȂȂ�



        // �R�C���擾�󋵏�����

        // ���Z�[�u�f�[�^����R�C���̎擾�f�[�^�����Ă���A�Ȃ��ꍇ������

        //if (SaveDataManager.Instance.MainData.Stage[].coin != null)
        //{
        //    coin = SaveDataManager.Instance.MainData.Stage[].coin;
        //    SetCoinObjectInfo();
        //}
        //else
        {
            // ���v�R�C�����擾
            AllCoin1 = Coins1.transform.childCount;
            AllCoin2 = Coins2.transform.childCount;
            AllCoin3 = Coins3.transform.childCount;

            // �R�C��������
            coin = new Coin();
            coin.NowGetCoin1 = new bool[AllCoin1];
            coin.NowGetCoin2 = new bool[AllCoin2];
            coin.NowGetCoin3 = new bool[AllCoin3];

            // �R�C���I�u�W�F�N�g�ɏ��Z�b�g
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


    //�@�R�C���f�[�^�Z�[�u(�Z�[�u�f�[�^�ɏ������ޗp)
    private void SetCoinSaveData()
    {
        //SaveDataManager.Instance.MainData.Stage[].coin = coin;
    }
}