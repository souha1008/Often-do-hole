using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[System.Serializable]
public class Coin
{
    // �������p�R���X�g���N�^
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

        AllCoins = Allcoin1 + Allcoin2 + Allcoin3;
        AllGetCoins = 0;
    }

    // �R�s�[�p�R���X�g���N�^
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

        AllCoins = coin.AllCoins;
        AllGetCoins = coin.AllGetCoins;
    }

    // �擾�R�C��
    public bool[] NowGetCoin1;
    public bool[] NowGetCoin2;
    public bool[] NowGetCoin3;

    // ���v�R�C����
    public int AllCoin1;
    public int AllCoin2;
    public int AllCoin3;
    public int AllCoins; // 3�������v

    // ���v����R�C����
    public int AllGetCoin1;
    public int AllGetCoin2;
    public int AllGetCoin3;
    public int AllGetCoins; // 3�������v
}


public class CoinManager : SingletonMonoBehaviour<CoinManager>
{
    // ���݂̃R�C���擾��
    [HideInInspector] public Coin coin = new Coin(3, 3, 3);        // �Q�[�����ł̃R�C���擾�p
    [HideInInspector] public Coin SubCoin = new Coin(3, 3, 3);     // �Q�[�����ł̃R�C���擾�p(�`�F�b�N�|�C���g�ʉ߂��ĂȂ��Ƃ��p)

    // �R�C���I�u�W�F�N�g
    public GameObject Coins1;
    public GameObject Coins2;
    public GameObject Coins3;

    private bool CoinManagerStageFlag = false;  // �X�e�[�W�ɓ��������Ɉ��̂ݏ���������悤�t���O

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

        if (!CoinManagerStageFlag)
        {
            // ���Z�[�u�f�[�^����R�C���̎擾�f�[�^�����Ă���A�Ȃ��ꍇ������
            if (SaveDataManager.Instance.MainData != null && SaveDataManager.Instance.MainData.Stage[GameStateManager.GetNowStage()].Clear == true)
            {
                coin = SaveDataManager.Instance.MainData.Stage[GameStateManager.GetNowStage()].coin;
            }
            else
            {
                // �R�C��������
                coin = new Coin(
                    Coins1.transform.childCount,
                    Coins2.transform.childCount,
                    Coins3.transform.childCount
                    );
            }
        }        
        SubCoin = new Coin(coin);

        // �R�C���I�u�W�F�N�g�ɏ��Z�b�g
        SetCoinObjectInfo();

        CoinManagerStageFlag = true;
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
            Coins2.transform.GetChild(i).GetComponent<CoinObject>().SetCoinObjectInfo(new StageCoinInfo(1, i, coin.NowGetCoin2[i]));
        }
        for (int i = 0; i < Coins3.transform.childCount; i++)
        {
            Coins3.transform.GetChild(i).GetComponent<CoinObject>().SetCoinObjectInfo(new StageCoinInfo(2, i, coin.NowGetCoin3[i]));
        }
    }


    // �R�C���f�[�^�X�V(�R�C���I�u�W�F�N�g������Ăԗp)
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

        // �R�C�����v���萔�X�V
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
        SubCoin.AllGetCoins = SubCoin.AllGetCoin1 + SubCoin.AllGetCoin2 + SubCoin.AllGetCoin3;
    }


    // �`�F�b�N�|�C���g���S�[���ʂ������̃R�C���f�[�^�X�V�p
    public void SetCheckPointCoinData()
    {
        if (SubCoin != null)
        {
            coin = new Coin(SubCoin);
            //Debug.LogWarning("�R�C���X�V");
            //Debug.LogWarning(coin.AllGetCoins);
        }
    }


    // �X�e�[�W����o�����̃R�C���}�l�[�W���[���Z�b�g�p
    public void ResetCoin()
    {
        coin = new Coin(3, 3, 3);
        SubCoin = new Coin(3, 3, 3);
        CoinManagerStageFlag = false;
        //Debug.LogWarning("�R�C�����Z�b�g");
    }


    //�@�R�C���f�[�^�Z�[�u(�Z�[�u�f�[�^�ɏ������ޗp)
    public void SetCoinSaveData()
    {
        SaveDataManager.Instance.MainData.Stage[GameStateManager.GetNowStage()].coin = new Coin(coin);
        //Debug.LogWarning("�Z�[�u�f�[�^�ɃZ�[�u");
    }

    //private void Update()
    //{
    //    Debug.LogWarning("���C���R�C�����v�F" + coin.AllGetCoins);
    //    Debug.LogWarning("�T�u�R�C�����v�F" + SubCoin.AllGetCoins);
    //}
}