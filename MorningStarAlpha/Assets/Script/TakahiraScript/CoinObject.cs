using UnityEngine;

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

    public void Start()
    {
        // �R���C�_�[
        this.gameObject.GetComponent<Collider>().isTrigger = true;

        if (CoinInfo != null)
        {
            if (CoinInfo.GetCoinFlag) Death();
        }
    }


    // �R�C�����Z�b�g
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
        // �v���C���[�ƐڐG���R�C���擾
        if (collider.gameObject.CompareTag("Player"))
        {
            CoinInfo.GetCoinFlag = true;
            CoinManager.Instance.SetCoinInfo(CoinInfo);
            Death();
        }
    }
}
