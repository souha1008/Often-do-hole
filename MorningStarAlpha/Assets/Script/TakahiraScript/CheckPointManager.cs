using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CheckPointManager : SingletonMonoBehaviour<CheckPointManager>
{
    [Label("���݂̃��X�|�[���I�u�W�F�N�g")]
    public GameObject RespawnObject = null;
    [Label("���݂̃��X�|�[�����W")]    
    public Vector3 NowRespawnPos;       // ���݂̃��X�|�[�����W(���邾��)

    private static Vector3 RespawnPos = Vector3.zero;   // ���X�|�[�����W
    public bool RespawnFlag = false;   // �`�F�b�N�|�C���g�Z�b�g�m�F�t���O

    private List<Vector3> OldCheckPoint = new List<Vector3>();    // �ʉߍς݃`�F�b�N�|�C���g

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject); // �V�[�����ς���Ă����ȂȂ�
    }


    // �`�F�b�N�|�C���g�̃Z�b�g
    public void SetCheckPoint(CheckPoint checkpoint)
    {
        // �����`�F�b�N�|�C���g���ƍX�V�s��if��
        bool SetFlag = true;

        for (int i = 0; i < OldCheckPoint.Count; i++)
        {
            if (OldCheckPoint[i] == checkpoint.RespawnPointObject.transform.position)
            {
                SetFlag = false;
            }
        }
        if (SetFlag)
        {
            RespawnPos = checkpoint.RespawnPointObject.transform.position;
            RespawnObject = checkpoint.GetComponentInParent<MeshOnOff>().gameObject; // ���b�V���؂�ւ��̕t�����e�I�u�W�F�N�g�擾
            NowRespawnPos = GetCheckPointPos(); // ���݂̃��X�|�[�����W�X�V

            CoinManager.Instance.SetCheckPointCoinData(); // �R�C���̏������

            OldCheckPoint.Add(RespawnPos);
        }
        RespawnFlag = true;
    }

    // ���݂̃`�F�b�N�|�C���g�̍��W�Q�b�g
    public Vector3 GetCheckPointPos()
    {
        //Debug.LogWarning(RespawnPos);
        return RespawnPos;
    }

    // �`�F�b�N�|�C���g���Z�b�g
    public void ResetCheckPoint()
    {
        RespawnPos = Vector3.zero;
        NowRespawnPos = GetCheckPointPos();
        RespawnObject = null;
        RespawnFlag = false;
        OldCheckPoint.Clear();
    }
}